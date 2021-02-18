using Com.Unit.API;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vue_Music_API.PolicyPermission;

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
                    Contact = new OpenApiContact() 
                    { 
                        Url = new Uri("http://localhost:5000/swagger/index.html"), 
                        Name = "WY", 
                        Email = "1753993763qq.com" 
                        },
                    Version = "v1",
                    
                });
                // 接口排序
                m.OrderActionsBy(o => o.RelativePath);

                #region 在header中添加token，传递到后台  
                m.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT授权(数据将在请求头中进行传递)直接在下面框中输入Bearer {token}(注意两者之间是一个空格)：Bearer Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                //添加全局安全条件
                m.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

                #endregion
            });

            #region 添加Jwtoken验证
            services.AddAuthorization(o =>
            {
                o.AddPolicy("AdminOrUser", o => {
                    o.RequireRole("Admin").Build();
                });

                o.AddPolicy("MustAdmin", o =>
                {
                    o.Requirements.Add(new MustAdminRequirement());
                });

                o.AddPolicy("MustAdmin2", o =>
                {
                    o.RequireClaim("Permission", "CanViewPage", "CanViewAnything");
                });
            });
            services.AddSingleton<IAuthorizationHandler, MustAdminHandler>();

            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("EF1DA5B4-C7FA-4240-B997-7D1701BF9BE2"));
            services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = securityKey,    // 加密解密Token的密钥

                        // 是否验证发布者
                        ValidateIssuer = true,
                        // 发布者名称
                        ValidIssuer = "issuer",

                        // 是否验证订阅者
                        // 订阅者名称
                        ValidateAudience = true,
                        ValidAudience = "audience",

                        // 是否验证令牌有效期
                        //RequireExpirationTime = true,
                        ValidateLifetime = true,
                        // 每次颁发令牌，令牌有效时间
                         ClockSkew = TimeSpan.FromMinutes(120)
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            var result = new
                            {
                                status = 401,
                                msg = "无效的Token",
                                err = "无效的Token"
                            };
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                            return Task.CompletedTask;
                        }
                    };
                });
            #endregion


            #region 跨域         
            services.AddCors(a => a.AddPolicy("any", o =>o
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()));
            
            #endregion
            services.AddControllers();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

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
