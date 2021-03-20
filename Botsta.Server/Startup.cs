using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Botsta.DataStorage.Models;
using Botsta.Server.Configuration;
using GraphQL.Types;
using GraphQL.Server;
using GraphQL.NewtonsoftJson;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GraphQL.Server.Ui.Playground;
using Botsta.Server.GraphQL;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.OpenApi.Models;
using Botsta.Server.Extentions;
using Botsta.Server.Middelware;

namespace Botsta.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            AppConfig = configuration
                .GetSection("AppConfig")
                .Get<AppConfig>();
        }

        public IConfiguration Configuration { get; }

        public AppConfig AppConfig { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<BotstaDbContext>(
                options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("Botsta.DataStorage")));

            services.AddScoped<IBotstaDbRepository, BotstaDbRepository>();

            services.AddSingleton(AppConfig);
            services.AddTransient<IIdentityService, IdentityService>();

            ConfigureGraphQL(services);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    var serverSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfig.JwtSecret));
                    options.TokenValidationParameters = new TokenValidationParameters {
                        IssuerSigningKey = serverSecret,
                        ValidIssuer = AppConfig.JwtIssuer,
                        ValidAudience = AppConfig.JwtAudience,
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "Botsta API",
                    Description = "Botsta API",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Jakob Bauer",
                        Url = new Uri("https://www.bauer-jakob.de"),
                        Email = "info@bauer-jakob.de"
                    }
                });
            });
        }

        private void ConfigureGraphQL(IServiceCollection services)
        {
            services.AddScoped<BotstaQuery>();
            services.AddScoped<BotstaMutation>();
            services.AddScoped<ISchema, BotstaSchema>();

            services.AddGraphQL((options, provider) =>
            {
                options.EnableMetrics = true;
                var logger = provider.GetRequiredService<ILogger<Startup>>();
                options.UnhandledExceptionDelegate = ctx => logger.LogError("{Error} occurred", ctx.OriginalException.Message);
            })
            .AddGraphQLAuthorization(options =>
                {
                    options.AddPolicy(PoliciesExtentions.User, p => p.UserPolicy());
                }
            )
           .AddErrorInfoProvider(opt => opt.ExposeExceptionStackTrace = true)
           .AddGraphTypes(typeof(BotstaSchema))
           .AddNewtonsoftJson();

            //services.AddSingleton<IAuthorizationEvaluator, AuthorizationEvaluator>()
            //    .AddTransient<IValidationRule, AuthorizationValidationRule>()
            //    //.AddTransient<AuthorizationSettings>()
            //    //.AddAuthorization(config =>
            //    //{
            //    //    config.AddPolicy(Policies.User, p => Policies.UserPolicy(p));
            //    //});
            //    .AddTransient(s =>
            //    {
            //        var authSettings = new AuthorizationSettings();
            //        authSettings.AddPolicy(Policies.User, p => p.RequireClaim("role", "User"));
            //        return authSettings;
            //    });


            services.Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true);
            services.Configure<IISServerOptions>(options => options.AllowSynchronousIO = true);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseGraphQLPlayground(new GraphQLPlaygroundOptions
                {
                    EditorTheme = EditorTheme.Dark,
                    PrettierTabWidth = 4
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseGraphQL<ISchema>();
                

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Botsta API");
            });
        }
    }
}
