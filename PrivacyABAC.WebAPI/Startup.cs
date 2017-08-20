using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PrivacyABAC.DbInterfaces.Repository;
using PrivacyABAC.MongoDb.Repository;
using PrivacyABAC.Core.Service;
using PrivacyABAC.MongoDb;
using NLog.Extensions.Logging;
using NLog.Web;
using Microsoft.AspNetCore.Cors.Infrastructure;
using PrivacyABAC.Functions;
using PrivacyABAC.Domains;
using PrivacyABAC.Domains.Common;

namespace PrivacyABAC.WebAPI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            env.ConfigureNLog("nlog.config");
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            //services.Configure<MongoDbContextProvider>(Configuration);
            services.AddSingleton(new MongoDbContextProvider()
            {
                ConnectionString = Configuration["MongoDbContextProvider:ConnectionString"],
                PolicyDatabaseName = Configuration["MongoDbContextProvider:PolicyDatabaseName"],
                UserCollectionName = Configuration["MongoDbContextProvider:UserCollectionName"],
                UserDatabaseName = Configuration["MongoDbContextProvider:UserDatabaseName"]
            });

            MongoDbContextProvider.Setup();

            services.AddScoped(typeof(IAccessControlPolicyRepository), typeof(AccessControlPolicyMongoDbRepository));
            services.AddScoped(typeof(IPrivacyPolicyRepository), typeof(PrivacyPolicyMongoDbRepository));
            services.AddScoped(typeof(IPolicyCombiningRepository), typeof(PolicyCombiningMongoDbRepository));
            services.AddScoped(typeof(IPrivacyDomainRepository), typeof(PrivacyDomainMongoDbRepository));
            services.AddScoped(typeof(IResourceRepository), typeof(ResourceMongoDbRepository));
            services.AddScoped(typeof(ISubjectRepository), typeof(SubjectMongoDbRepository));

            services.AddSingleton(typeof(AccessControlService));
            services.AddSingleton(typeof(PrivacyService));
            services.AddSingleton(typeof(SecurityService));
            services.AddSingleton(typeof(ConditionalExpressionService));

            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin(); // For anyone access.
            corsBuilder.AllowCredentials();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", corsBuilder.Build());
            });

            var pluginFunctionFactory = UserDefinedFunctionFactory.GetInstance();
            pluginFunctionFactory.RegisterDefaultFunctions();

            var domainFactory = PrivacyDomainPluginFactory.GetInstance();
            domainFactory.RegisterDefaultPlugin();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();

            //add NLog.Web
            app.AddNLogWeb();

            app.UseMvc();
            app.UseCors("CorsPolicy");
        }
    }
}
