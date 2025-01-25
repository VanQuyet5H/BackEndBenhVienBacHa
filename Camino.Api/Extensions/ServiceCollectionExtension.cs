using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Auth;
using Camino.Api.Extensions.ModelProvider;
using Camino.Api.Models.Error;
using Camino.Core.Caching;
using Camino.Core.Configuration;
using Camino.Core.DependencyInjection;
using Camino.Data;
using Camino.Services.Helpers;
using FirebaseAdmin;
using FluentValidation.AspNetCore;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Wkhtmltopdf.NetCore;

namespace Camino.Api.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            var jwtConfig = services.ConfigureStartupConfig<JwtConfig>(configuration.GetSection("Jwt"));
            services.ConfigureCors();
            services.AddDbContext<CaminoObjectContext>(c => c.UseSqlServer(configuration.GetConnectionString("MainConnection"), 
                sqlServerOptions => {
                    sqlServerOptions.EnableRetryOnFailure(5,TimeSpan.FromSeconds(10),null);
                    sqlServerOptions.CommandTimeout(80);
            }));

            services.AddHangfire(hangfireConfiguration => hangfireConfiguration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(1),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(1),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                }));
            services.AddHangfireServer();

            services.ConfigureAuth(jwtConfig);
            services.ConfigureAutoMapper();
            services.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>().HttpContext?.User);

            services.ConfigureStartupConfig<SmsConfig>(configuration.GetSection("Sms"));
            services.ConfigureStartupConfig<EmailConfig>(configuration.GetSection("Email"));
            services.ConfigureStartupConfig<S3UploadConfig>(configuration.GetSection("S3Upload"));
            services.ConfigureStartupConfig<FileTypeConfig>(configuration.GetSection("FileType"));
            services.ConfigureStartupConfig<LISConfig>(configuration.GetSection("LIS"));
            services.ConfigureStartupConfig<ExportPdfApiConfig>(configuration.GetSection("ExportPdfApi"));
            services.AddDependencyInjection(nameof(Camino));

            services.AddSingleton<IStaticCacheManager, MemoryCacheManager>();

            // Initialize the Firebase Admin SDK
            if (hostingEnvironment.IsDevelopment())
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(@"hoaiduc-firebase.json")
                });
            }
            else
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(@"hoaiduc-firebase.production.json")
                });
            }
            services.AddMvc(option =>
                {
                    option.OutputFormatters.RemoveType<TextOutputFormatter>();
                    option.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
                    option.StringTrimmingProvider();
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new
                        ForWebApiTrimmingConverter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddFluentValidation(fv => {
                    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = true;
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "BV HoaiDuc API", Version = "v1" });
                c.AddSecurityDefinition("Bearer",
                    new ApiKeyScheme
                    {
                        In = "header",
                        Description = "Please enter into field the word 'Bearer' following by space and JWT",
                        Name = "Authorization",
                        Type = "apiKey"
                    });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    { "Bearer", Enumerable.Empty<string>() },
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            //services.ConfigureFluentValidation();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (context) => throw new ValidationApiException(context.ModelState);
            });
            services.AddWkhtmltopdf("Resource\\wkhtmltopdf");
        }
        public static void StringTrimmingProvider(this MvcOptions option)
        {
            var binderToFind = option.ModelBinderProviders
                .FirstOrDefault(x => x.GetType() == typeof(SimpleTypeModelBinderProvider));
            if (binderToFind == null)
            {
                return;
            }
            var index = option.ModelBinderProviders.IndexOf(binderToFind);
            option.ModelBinderProviders.Insert(index, new CustomeTrimmingModelBinderProvider());
        }

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
        }

        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            var typeMappingProfiles = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => type.BaseType == typeof(Profile));
            var mappingConfig = new MapperConfiguration(mc =>
            {
                foreach (var t in typeMappingProfiles)
                {
                    mc.AddProfile(t);
                }
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            AutoMapperExtension.Mapper = mapper;
        }

        public static void ConfigureAuth(this IServiceCollection services, JwtConfig jwtConfig)
        {
            var signingKey =
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.SecretKey));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.InternalIssuer = jwtConfig.InternalIssuer;
                options.PortalIssuer = jwtConfig.PortalIssuer;
                options.Audience = jwtConfig.Audience;
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuers = new []{ jwtConfig.InternalIssuer, jwtConfig.PortalIssuer },

                ValidateAudience = true,
                ValidAudience = jwtConfig.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtConfig.InternalIssuer;
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
                configureOptions.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }

        public static TConfig ConfigureStartupConfig<TConfig>(this IServiceCollection services, IConfiguration configuration) where TConfig : class, new()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            //create instance of config
            var config = new TConfig();

            //bind it to the appropriate section of configuration
            configuration.Bind(config);

            //and register it as a service
            services.AddSingleton(config);

            return config;
        }
        
    }
}
