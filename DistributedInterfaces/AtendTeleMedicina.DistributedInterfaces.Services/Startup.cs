using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.IO.Compression;
using System.Linq;
using System.Text;
using AtendTeleMedicina.DistributedInterfaces.Services.Configuration;
using AtendTeleMedicina.DistributedInterfaces.Services.Configuration.Swagger;
using AtendTeleMedicina.Infra.CrossCutting.Ioc;
using KissLog;
using KissLog.AspNetCore;
using KissLog.CloudListeners.RequestLogsListener;
using System.Diagnostics;
using System.IO;
using AtendTeleMedicina.DistributedInterfaces.Services.Hubs;
using AtendTeleMedicina.Application.Services.Nucleo;

namespace AtendTeleMedicina.DistributedInterfaces.Services
{
    public class Startup
    {
        readonly string AllowedOrigins = "_allowedOrigins";
        private IConfiguration Configuration { get; }
        private IHostingEnvironment _environment;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(env.ContentRootPath)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
              .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
            _environment = env;
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });

            services.AddAutoMapper(typeof(Startup));

            services.AddSignalR();

            #region KissLog
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ILogger>((context) =>
            {
                return Logger.Factory.Get();
            });

            services.AddLogging(logging =>
            {
                logging.AddKissLog();
            });
            #endregion

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            //services.AddHealthChecks()
            //    .AddDiskStorageHealthCheck(s => s.AddDrive("C:\\", 1024))
            //    .AddPrivateMemoryHealthCheck(512)
            //    .AddVirtualMemorySizeHealthCheck(512)
            //    .AddMySql(connectionString: Configuration.GetConnectionString("DefaultConnection"), name: "BancoMySql");

            //services.AddHealthChecksUI();

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddCors(options =>
            {
                options.AddPolicy(AllowedOrigins,
                builder =>
                {
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                    builder.WithExposedHeaders("Pagination");
                    builder.AllowCredentials();
                    //builder.AllowAnyOrigin();
                    builder.WithOrigins("http://atendtelemedicina.esusatendsaude.com.br",
                        "https://atendtelemedicina.esusatendsaude.com.br",
                        "http://localhost:5173",
                        "https://localhost:5173",
                        "http://localhost:5174",
                        "https://localhost:5174",
                        "https://web-sergipe.meeds.com.br",
                        "https://doctor-sergipe.meeds.com.br"
                        );
                });
            });

            services.Configure<GzipCompressionProviderOptions>(
                options => options.Level = CompressionLevel.Optimal);

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
            });

            BootStrapper.Register(services);

            services.AddMvcCore()
                .AddAuthorization()
                .AddApiExplorer()
                .AddJsonFormatters();

            #region Custom 401 Error
            //// Só para erro 401, versão futura do JwtBearerEvents pretende implementar para o 403 em 27/08/2017
            var jwtCustomBearerEvents = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.Response.StatusCode = 401;

                    context.Response.Headers.Append(
                      HeaderNames.WWWAuthenticate,
                      context.Options.Challenge);

                    if (!string.IsNullOrEmpty(context.Error))
                    {
                        await context.Response.WriteAsync(context.Error + ". " + context.ErrorDescription).ConfigureAwait(false);
                    }
                    context.HandleResponse();
                }
            };
            #endregion

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;

                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = Configuration["Tokens:Issuer"],
                        ValidAudience = Configuration["Tokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"])),

                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                    cfg.Events = jwtCustomBearerEvents;
                    cfg.IncludeErrorDetails = true;

                });

            services.AddSwaggerConfig();

            services.ResolveDependencies();

        }

        public void Configure(IApplicationBuilder app)
        {
            // Erros 401 e 403 Customizados
            app.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Request.Path.StartsWithSegments("/api"))
                    if (context.HttpContext.Response.StatusCode == 401) await context.HttpContext.Response.WriteAsync("Não autorizado.").ConfigureAwait(true);
                if (context.HttpContext.Response.StatusCode == 403) await context.HttpContext.Response.WriteAsync("Sem permissão para essa operação.").ConfigureAwait(true);
            });

            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    HotModuleReplacementEndpoint = "/dist/__webpack_hmr"
                }); ;
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
            }

            app.UseCors(AllowedOrigins);

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseSignalR(routes =>
            {
                routes.MapHub<HubProvider>("/Hub");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API AtendTeleMedicina V1");
            });

            app.UseAuthentication();

            //app.UseHealthChecks("/hc", new HealthCheckOptions()
            //{
            //    Predicate = _ => true,
            //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            //});
            //app.UseHealthChecksUI(
            //    options =>
            //    {
            //        options.UIPath = "/hc-ui";
            //        options.AddCustomStylesheet(Path.Combine(_environment.WebRootPath, "assets/css/hc-dotnet.css"));
            //    }
            //    );

            app.UseResponseCompression();

            app.UseKissLogMiddleware(options =>
            {
                ConfigureKissLog(options);
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.MapWhen(x => !x.Request.Path.Value.StartsWith("/v1/api"), builder =>
            {
                builder.UseMvc(routes =>
                {
                    routes.MapSpaFallbackRoute(
                      name: "spa-fallback",
                      defaults: new { controller = "Home", action = "Index" });
                });
            });
        }

        private void ConfigureKissLog(IOptionsBuilder options)
        {
            // optional KissLog configuration
            options.Options
                .AppendExceptionDetails((Exception ex) =>
                {
                    StringBuilder sb = new StringBuilder();

                    if (ex is System.NullReferenceException nullRefException)
                    {
                        sb.AppendLine("Important: check for null references");
                    }

                    return sb.ToString();
                });

            // KissLog internal logs
            options.InternalLog = (message) =>
            {
                Debug.WriteLine(message);
            };

            KissLogConfiguration.Listeners.Add(new RequestLogsApiListener(new KissLog.CloudListeners.Auth.Application(
                Configuration["KissLog.OrganizationId"],
                Configuration["KissLog.ApplicationId"])
            )
            {
                ApiUrl = Configuration["KissLog.ApiUrl"]
            });
        }

    }
}
