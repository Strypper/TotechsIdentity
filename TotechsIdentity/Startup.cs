using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories;
using System.Text;
using TotechsIdentity.DataObjects;

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

            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddDbContextPool<IdentityContext>(options => options.UseSqlServer(Configuration.GetConnectionString("IdentityContext")));
            services.AddIdentity<User, Role>()
                    .AddEntityFrameworkStores<IdentityContext>()
                    .AddUserManager<UserManager>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "jwt";
                options.DefaultChallengeScheme = "jwt";
            })
              .AddCookie(cfg => cfg.SlidingExpiration = true)
              .AddJwtBearer("jwt", cfg =>
              {
                  cfg.RequireHttpsMetadata = false;
                  cfg.SaveToken = true;

                  cfg.TokenValidationParameters = new TokenValidationParameters()
                  {
                      ValidIssuer = Configuration["JwtTokens:Issuer"],
                      ValidAudience = Configuration["JwtTokens:Issuer"],
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtTokens:Key"]))
                  };

              });

            services.AddAuthorization(options =>
            {
                //options.AddPolicy(Constants.Policies.CompanyOnly, policy => policy.RequireClaim("CompanyType", "Company"));
            });

            services.AddSingleton(new MapperConfiguration(mc =>
            {
                mc.AddCollectionMappers();
                mc.AddProfile(new MappingProfile());
            }).CreateMapper());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TotechsIdentity", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TotechsIdentity v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
