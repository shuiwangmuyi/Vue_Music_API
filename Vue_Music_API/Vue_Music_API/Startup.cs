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
                    Description = "这是一个基于.net core的web API==接口文档",
                    Contact =new OpenApiContact() { Name ="WY",Email="1753993763qq.com"},
                    Version = "v1"
                });      
            });

            #region 添加Jwtoken验证
            // 设置验证方式为 Bearer Token
            // 你也可以添加 using Microsoft.AspNetCore.Authentication.JwtBearer;
            // 使用 JwtBearerDefaults.AuthenticationScheme 代替 字符串 "Brearer"
            services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abcdABCD1234abcdABCD1234")),    // 加密解密Token的密钥

                        // 是否验证发布者
                        ValidateIssuer = true,
                        // 发布者名称
                        ValidIssuer = "server",

                        // 是否验证订阅者
                        // 订阅者名称
                        ValidateAudience = true,
                        ValidAudience = "client007",

                        // 是否验证令牌有效期
                        ValidateLifetime = true,
                        // 每次颁发令牌，令牌有效时间
                        ClockSkew = TimeSpan.FromMinutes(120)
                    };
                });
            #endregion

            #region 跨域         
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
