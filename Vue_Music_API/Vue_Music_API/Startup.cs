using Com.Unit.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vue_Music_API
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
            DBSugar.conStr = Configuration["DB_ORMName"].Trim().ToString();
            services.AddSwaggerGen(m => {
                m.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Vue_Music_API",
                    Description="API For WY",
                    Contact=new OpenApiContact() { Name ="WY",Email="1753993763qq.com"},
                    Version = "v1"
                });
            });
            services.AddCors(a => a.AddPolicy("any", o => o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
            services.AddControllers();
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

            app.UseAuthorization();

            app.UseSwagger();
            app.UseCors();
            app.UseSwaggerUI(
                m => {
                    m.SwaggerEndpoint("/swagger/v1/swagger.json", "Vue_Music_API");
                });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
