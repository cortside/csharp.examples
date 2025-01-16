using Asp.Versioning.ApiExplorer;
using Cortside.AspNetCore;
using Cortside.AspNetCore.Auditable;
using Cortside.AspNetCore.Auditable.Entities;
using Cortside.AspNetCore.Builder;
using Cortside.AspNetCore.Common;
using Cortside.AspNetCore.Filters;
using Cortside.AspNetCore.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace WeatherForecast.WebApi {
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup : IWebApiStartup {
        /// <summary>
        /// Startup
        /// </summary>
        /// <param name="configuration"></param>
        [ActivatorUtilitiesConstructor]
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        /// <summary>
        /// Startup
        /// </summary>
        public Startup() {
        }

        /// <summary>
        /// Config
        /// </summary>
        private IConfiguration Configuration { get; set; }

        /// <summary>
        /// Sets the Configuration to be used
        /// </summary>
        /// <param name="config"></param>
        public void UseConfiguration(IConfiguration config) {
            Configuration = config;
        }

        /// <summary>
        /// Configure Services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services) {
            // setup global default json serializer settings
            JsonConvert.DefaultSettings = JsonNetUtility.GlobalDefaultSettings;

            // add controllers and all of the api defaults
            services.AddApiDefaults(InternalDateTimeHandling.Utc, options => {
                options.Filters.Add<MessageExceptionResponseFilter>();
            });

            // add SubjectPrincipal for auditing
            services.AddSubjectPrincipal();
            services.AddTransient<ISubjectFactory<Subject>, DefaultSubjectFactory>();

            // Add swagger with versioning and OpenID Connect configuration using Newtonsoft
            services.AddSwagger(Configuration, "Acme.ShoppingCart API", "Acme.ShoppingCart API", ["v1"]);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="provider"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider) {
            app.UseApiDefaults(Configuration);
            app.UseSwagger("WebApi", provider);

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            // order of the following matters
            app.UseAuthentication();
            app.UseSubjectPrincipal(); // intentionally set after UseAuthentication
            app.UseRouting();
            app.UseAuthorization(); // intentionally set after UseRouting
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
