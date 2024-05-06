using Domain.Models.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net;
using WebService.Filters;
using WebService.Models.AppResponse;
using DataLayer.RelationDB.Interfaces;
using DataLayer.RelationDB;
using Domain.Defines;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Security;
using Domain.UnitOfWork;
using CommonLibrary;
using WebService.Models.Defines;
using WebService.Filters.Swagger;
using WebService.Services;
using WebService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Models.Context;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Npgsql;
using System.Data;
using CommonLibrary.Util;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using DataLayer.Repository;
using DataLayer.Repository.Interfaces;
using WebService.Utility;
using CommonLibrary.Filter;
using System.IO;
using System.Reflection;

namespace WebService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private readonly WebServiceSetting Setting = new WebServiceSetting();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            SetAppsettingsInfo(services);
            SetJSONFormatInfo(services);
            SetSwaggerInfo(services);
            SetNpgsqlConnectionInfo(services);
            SetJwtAuthenticationInfo(services);
            SetCorsPolicyInfo(services);
            SetPluginDIInfo(services);
            SetFilterInfo(services);
            SetAttributeInfo(services);
            // 設定HttpClientFactory
            SetHttpClientFactory(services);

            ServicePointManager.DefaultConnectionLimit = short.MaxValue;
            ServicePointManager.ServerCertificateValidationCallback +=
              (sender, cert, chain, sslPolicyErrors) =>
              {
                  if (sslPolicyErrors == SslPolicyErrors.None)
                  {
                      return true;
                  }
                  var request = sender as HttpWebRequest;
                  if (request != null)
                  {
                      var result = request.RequestUri.Host == Setting.HttpWebRequestHost;

                      return result;
                  }
                  return false;
              };

            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
            // If using IIS:
            services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });
        }

        private void SetAppsettingsInfo(IServiceCollection services)
        {
            Configuration.Bind(Setting);
            services.AddSingleton(Setting);
        }

        private void SetJSONFormatInfo(IServiceCollection services)
        {
            services.AddControllers();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                NullValueHandling = NullValueHandling.Include,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        private void SetCorsPolicyInfo(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed(x => true);
                });
            });
        }

        private void SetSwaggerInfo(IServiceCollection services)
        {
            if (Setting.EnableSwagger)
            {
                services.AddSwaggerGen(c =>
                {
                    var apiInfo = new OpenApiInfo
                    {
                        Title = "DIR MONITOR WebService",
                        Version = "v1",
                        Description = "DIR MONITOR WebService",
                    };
                    c.SwaggerDoc("v1", apiInfo);

                    #region Bearer token authentication
                    OpenApiSecurityScheme securityDefinition = new OpenApiSecurityScheme()
                    {
                        Description = "No need to put the `bearer` keyword in front of token",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT"
                    };
                    c.AddSecurityDefinition("jwt_auth", securityDefinition);
                    c.OperationFilter<AuthorizationOperationFilter>();

                    // Set the comments path for the Swagger JSON and UI.
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                    // model comments 
                    var modelXml = "Models.xml";
                    var modelXmlPath = Path.Combine(AppContext.BaseDirectory, modelXml);
                    c.IncludeXmlComments(modelXmlPath);
                    c.SchemaFilter<SchemaFilter>();
                    #endregion

                    c.IncludeXmlComments("./Swagger.xml");
                    c.SchemaFilter<EnumSchemaFilter>();
                    c.OperationFilter<ApiHeaderManagerFilter>();
                });
            }
        }

        private void SetNpgsqlConnectionInfo(IServiceCollection services)
        {
            services.AddTransient<IDbConnection>(db =>
                new NpgsqlConnection(Setting.RelationDB.DIR_ConnectionString));
        }

        private void SetJwtAuthenticationInfo(IServiceCollection services)
        {
            services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
                options.IncludeErrorDetails = true; // 預設值為 true，有時會特別關閉

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // 透過這項宣告，就可以從 "sub" 取值並設定給 User.Identity.Name
                    NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                    // 透過這項宣告，就可以從 "roles" 取值，並可讓 [Authorize] 判斷角色
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

                    // 一般我們都會驗證 Issuer
                    ValidateIssuer = true,
                    ValidIssuer = Setting.JwtSettings.Issuer,

                    // 通常不太需要驗證 Audience
                    ValidateAudience = false,
                    //ValidAudience = "JwtAuthDemo", // 不驗證就不需要填寫

                    // 一般我們都會驗證 Token 的有效期間
                    ValidateLifetime = true,

                    // 如果 Token 中包含 key 才需要驗證，一般都只有簽章而已
                    ValidateIssuerSigningKey = false,

                    // 從 IConfiguration 取得
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Setting.JwtSettings.SignKey))
                };
            });
        }

        private void SetPluginDIInfo(IServiceCollection services)
        {
            // Db context
            services.AddDbContext<DIRContext>(options =>
    options.UseNpgsql(
        Setting.RelationDB.DIR_ConnectionString),
        ServiceLifetime.Scoped);
            services.AddScoped<DIRContext>(provider => {
                var webServiceSetting = provider.GetRequiredService<IOptions<WebServiceSetting>>().Value;
                var options = provider.GetRequiredService<DbContextOptions<DIRContext>>();
                return new DIRContext(options, webServiceSetting);
            });

            // repository
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Common Service
            services.AddSingleton<JwtHelper>();

            // Service
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<ISessionService, SessionService>();

            // DB 
            services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddScoped<ISessionDBAccess, SessionDBAccess>();
            services.AddScoped<ILoginDBAccess, LoginDBAccess>();
        }

        private void SetFilterInfo(IServiceCollection services)
        {
            services.AddMvcCore(config =>
            {
                config.Filters.Add(new ExceptionFilter(Setting));
                config.Filters.Add(new ResourceFilter(Setting));
                config.Filters.Add(new ResultFilter(Setting));
            });
        }

        private void SetAttributeInfo(IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressInferBindingSourcesForParameters = true;
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                                    .Where(p => p.Value.Errors.Count > 0)
                                    .ToList();
                    var keys = string.Join(Environment.NewLine, errors.Select(x => x.Key));

                    var errMsgs = errors.ToDictionary(
                        x => x.Key,
                        x => x.Value
                            .Errors
                            .Select(e => e.ErrorMessage)
                            .ToList());

                    var errorMsg = string.Empty;
                    var errorDefine = ErrorDefine.GetErrorDefine(ErrorCodeEnum.PARAMETER_ERR_CODE);
                    if (errMsgs != null && errMsgs.Any())
                    {
                        foreach (var errMsg in errMsgs)
                        {
                            errorMsg += string.Join(", ", errMsg.Value) + ", ";
                        }

                        errorMsg = errorMsg.Substring(0, errorMsg.Length - 2);
                    }
                    else
                    {
                        errorMsg = errorDefine.ErrorMsg;
                    }

                    var errResponse = new BaseResponseModel<string>
                    {
                        RtnCode = ((int)errorDefine.ErrorCode).ToString(),
                        RtnMsg = errorMsg
                    };

                    var response = new BadRequestObjectResult(errResponse)
                    {
                        StatusCode = 200
                    };

                    return response;
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (Setting.EnableSwagger)
            {
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "swagger/{documentName}/swagger.json";
                });
                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = string.Empty;
                    c.SwaggerEndpoint("swagger/v1/swagger.json", "ver 1");
                });
            }
        }

        private IServiceCollection SetHttpClientFactory(IServiceCollection services)
        {
            services.AddHttpClient(ExternalServiceEnum.SendSMS.ToString(), x =>
            {
                x.BaseAddress = new Uri(Setting.SMSSetting.Url);
                x.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                x.Timeout = TimeSpan.FromSeconds(10);
            });

            return services;
        }

        public class AuthorizationOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var actionMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
                var isAuthorized = actionMetadata.Any(metadataItem => metadataItem is AuthorizeAttribute);
                var allowAnonymous = actionMetadata.Any(metadataItem => metadataItem is AllowAnonymousAttribute);
                if (!isAuthorized || allowAnonymous)
                {
                    return;
                }

                operation.Parameters ??= new List<OpenApiParameter>();
                operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "jwt_auth",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            };
            }
        }
    }
}
