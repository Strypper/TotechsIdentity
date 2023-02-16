using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using AutoMapper;
using Azure.Storage;
using Azure.Storage.Blobs;
using Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories;
using TotechsIdentity.AppSettings;
using TotechsIdentity.Constants;
using TotechsIdentity.DataObjects;
using TotechsIdentity.Services;
using TotechsIdentity.Services.IService;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<JwtTokenConfig>(builder.Configuration.GetSection("JwtTokenConfig"));
builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("EmailConfig"));
builder.Services.Configure<AzureStorageConfig>(builder.Configuration.GetSection("AzureStorageConfig"));

builder.Services.AddHttpClient();
builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddDbContextPool<IdentityContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityContext")));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = SwaggerConstants.Title,
        Version = SwaggerConstants.OpenAPIVersion
    });
    c.AddSecurityDefinition(SwaggerConstants.SecurityDefinitionName, new OpenApiSecurityScheme
    {
        Name = SwaggerConstants.SecurityDefinitionName,
        Description = SwaggerConstants.Description,
        Scheme = SwaggerConstants.Scheme,
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
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

builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 1;

    options.User.RequireUniqueEmail = true; //default false
                                            //options.SignIn.RequireConfirmedEmail = true;
})
        .AddEntityFrameworkStores<IdentityContext>()
        .AddUserManager<UserManager>()
        .AddDefaultTokenProviders();

builder.Services.AddScoped<IEmailService, SMTPEmailService>();
builder.Services.AddScoped<ITokenService, JWTTokenService>();
builder.Services.AddScoped<IMediaService, AzureBlobStorageMediaService>();
builder.Services.AddSingleton((provider) =>
{
    var config = provider.GetRequiredService<IOptionsMonitor<AzureStorageConfig>>().CurrentValue;
    return new StorageSharedKeyCredential(config.AccountName, config.AccountKey);
});

//Create SMTP Client
builder.Services.AddScoped(provider =>
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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
  .AddJwtBearer(cfg =>
  {
      cfg.RequireHttpsMetadata = false;
      cfg.SaveToken = true;

      cfg.TokenValidationParameters = new TokenValidationParameters()
      {
          ValidAudience = builder.Configuration["JwtTokenConfig:Issuer"],
          ValidateIssuer = false,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtTokenConfig:Key"]))
      };

  });

builder.Services.AddSingleton(new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
}).CreateMapper());

builder.Services.AddSingleton<BlobContainerClient>(provider =>
{
    var config = provider.GetRequiredService<IOptionsMonitor<AzureStorageConfig>>().CurrentValue;
    return new BlobContainerClient(config.BlobConnectionString, config.ImageContainer);
});



var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCors(opt => opt.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

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

app.Run();