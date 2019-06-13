using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArgoManager.Lib;
using Docker.DotNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace ArgoManager
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors();
            // docker stuff
            var dockerClient = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();
            var dockerManager = new DockerManager(dockerClient);
            services.AddSingleton<IDockerManager>(dockerManager);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { 
                        Title = "Argo Manager", 
                        Version = "v1",
                        Description = "A tool to manage Argo Tunnels",
                        Contact = new Contact { Name = "Daan Boerlage", Url = "https://github.com/runebaas" } ,
                        License = new License { Name = "None" } 
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Argo Manager V1");
            });

            app.UseHttpsRedirection();
            app.UseCors();
            app.UseMvc();
        }
    }
}
