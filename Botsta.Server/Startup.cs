using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Botsta.DataStorage.Entities;
using Botsta.Core.Configuration;
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
using Botsta.Core.Extentions;
using Botsta.Core;
using Botsta.Server.Services;
using Microsoft.Extensions.Primitives;
using System.IO;
using GraphQL.Server.Transports.AspNetCore.NewtonsoftJson;
using System.Runtime.Serialization.Formatters.Binary;
using Botsta.DataStorage;
using Botsta.Server.GraphQL.Types;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;
using Botsta.Core.Services;

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

        public TokenValidationParameters TokenValidationParameters
        {
            get
            {
                var serverSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfig.JwtSecret));
                return new TokenValidationParameters
                {
                    IssuerSigningKey = serverSecret,
                    ValidIssuer = AppConfig.JwtIssuer,
                    ValidAudience = AppConfig.JwtAudience,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                };
            }
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<BotstaDbContext>(
                options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("Botsta.DataStorage")));

            services.AddScoped<IBotstaDbRepository, BotstaDbRepository>();

            services.AddSingleton(TokenValidationParameters);
            services.AddSingleton(AppConfig);
            services.AddScoped<IIdentityService, IdentityService>();

            ConfigureGraphQL(services);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = TokenValidationParameters;
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
            services.AddScoped<BotstaSubscription>();
            services.AddScoped<GraphChatroomType>();
            services.AddScoped<GraphMessageType>();
            services.AddScoped<ChatPracticantGraphType>();
            services.AddScoped<BotGraphType>();
            services.AddScoped<ISchema, BotstaSchema>();

            services.AddScoped<ISessionController, SessionController>();
            services.AddSingleton<IChatNotifier, ChatNotifier>();

            services.AddGraphQL((options, provider) =>
            {
                options.EnableMetrics = true;
                //options.MaxParallelExecutionCount = 20;
                var logger = provider.GetRequiredService<ILogger<Startup>>();
                options.UnhandledExceptionDelegate = ctx => logger.LogError(ctx.OriginalException, string.Empty);
            })
            .AddGraphQLAuthorization(options =>
                {
                    options.AddPolicy(PoliciesExtentions.User, p => p.UserPolicy());
                    options.AddPolicy(PoliciesExtentions.Bot, p => p.BotPolicy());
                    options.AddPolicy(PoliciesExtentions.RefreshToken, p => p.RefreshTokenPolicy());
                }
            )
           .AddErrorInfoProvider(opt => opt.ExposeExceptionStackTrace = true)
           .AddNewtonsoftJson()
           .AddWebSockets()
           .AddDataLoader()
           .AddGraphTypes(typeof(BotstaSchema));

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

            var autoMigrate = Configuration.GetValue<bool>("AutoMigrate", false);
            if (autoMigrate)
            {
                MigrateDatabase(app);
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseWebSockets(
                new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(3) }
                );
            app.UseGraphQLWebSockets<ISchema>();
            app.UseGraphQL<ISchema>();

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Botsta API");
            });
        }


        private void MigrateDatabase(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<BotstaDbContext>();

            if (db.Database.GetPendingMigrations().Any())
            {
                db.Database.Migrate();
            }

        }
    }
}
