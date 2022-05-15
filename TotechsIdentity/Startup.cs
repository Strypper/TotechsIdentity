using AutoMapper;
using Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using TotechsIdentity.AppSettings;
using TotechsIdentity.Constants;
using TotechsIdentity.DataObjects;
using TotechsIdentity.Services;
using TotechsIdentity.Services.IService;

namespace TotechsIdentity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtTokenConfig>(Configuration.GetSection("JwtTokenConfig"));
            services.Configure<EmailConfig>(Configuration.GetSection("EmailConfig"));

            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddDbContextPool<IdentityContext>(options => options.UseSqlServer(Configuration.GetConnectionString("IdentityContext")));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                                        { 
                                          Title   = SwaggerConstants.Title, 
                                          Version = SwaggerConstants.OpenAPIVersion 
                                        });
                c.AddSecurityDefinition(SwaggerConstants.SecurityDefinitionName, new OpenApiSecurityScheme
                {
                    Name        = SwaggerConstants.SecurityDefinitionName,
                    Description = SwaggerConstants.Description,
                    Scheme      = SwaggerConstants.Scheme,
                    In          = ParameterLocation.Header,
                    Type        = SecuritySchemeType.ApiKey,
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });

            services.AddIdentity<User, Role>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 1;

                //options.User.RequireUniqueEmail = true; //default false
                //options.SignIn.RequireConfirmedEmail = true;
            })
                    .AddEntityFrameworkStores<IdentityContext>()
                    .AddUserManager<UserManager>()
                    .AddDefaultTokenProviders();

            services.AddScoped<IEmailService, SMTPEmailService>();
            services.AddScoped<ITokenService, JWTTokenService>();
            //Create SMTP Client
            services.AddScoped(provider =>
            {
                var config = provider.GetRequiredService<IOptionsMonitor<EmailConfig>>().CurrentValue;
                SmtpClient client = new(config.Host, config.Port)
                {
                    EnableSsl = config.EnableSsl,
                    UseDefaultCredentials = config.UseDefaultCredentials,
                    Credentials = new NetworkCredential(config.UserName, config.AppPassword)
                };

                return client;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
              .AddCookie(cfg => cfg.SlidingExpiration = true)
              .AddJwtBearer(cfg =>
              {
                  cfg.RequireHttpsMetadata = false;
                  cfg.SaveToken = true;

                  cfg.TokenValidationParameters = new TokenValidationParameters()
                  {
                      ValidAudience = Configuration["JwtTokenConfig:Issuer"],
                      ValidateIssuer = false,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtTokenConfig:Key"]))
                  };

              });

            services.AddAuthorization(options =>
            {
                //options.AddPolicy(Constants.Policies.CompanyOnly, policy => policy.RequireClaim("CompanyType", "Company"));
            });

            services.AddSingleton(new MapperConfiguration(mc =>
            {
                //mc.AddCollectionMappers(); //outdated version
                mc.AddProfile(new MappingProfile());
            }).CreateMapper());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint(SwaggerConstants.Url, SwaggerConstants.SwaggerEndPointName));
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
