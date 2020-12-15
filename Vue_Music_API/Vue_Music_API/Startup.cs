using Com.Unit.API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
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
                    Description = "����һ������.net core��web API==�ӿ��ĵ�",
                    Contact =new OpenApiContact() { Name ="WY",Email="1753993763qq.com"},
                    Version = "v1"
                });      
            });

            #region ���Jwtoken��֤
            // ������֤��ʽΪ Bearer Token
            // ��Ҳ������� using Microsoft.AspNetCore.Authentication.JwtBearer;
            // ʹ�� JwtBearerDefaults.AuthenticationScheme ���� �ַ��� "Brearer"
            services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abcdABCD1234abcdABCD1234")),    // ���ܽ���Token����Կ

                        // �Ƿ���֤������
                        ValidateIssuer = true,
                        // ����������
                        ValidIssuer = "server",

                        // �Ƿ���֤������
                        // ����������
                        ValidateAudience = true,
                        ValidAudience = "client007",

                        // �Ƿ���֤������Ч��
                        ValidateLifetime = true,
                        // ÿ�ΰ䷢���ƣ�������Чʱ��
                        ClockSkew = TimeSpan.FromMinutes(120)
                    };
                });
            #endregion

            #region ����         
            services.AddCors(a => a.AddPolicy("any", o => o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
            services.AddControllers();
            #endregion

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();            
            app.UseSwagger();
            app.UseCors();
            app.UseSwaggerUI(
                m => {
                    m.SwaggerEndpoint("/swagger/v1/swagger.json", "swaggerTest");
                });

       

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
