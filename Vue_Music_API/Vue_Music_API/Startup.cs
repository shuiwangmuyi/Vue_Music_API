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
                    Description = "����һ������.net core��web API==�ӿ��ĵ�",
                    Contact = new OpenApiContact() 
                    { 
                        Url = new Uri("http://localhost:5000/swagger/index.html"), 
                        Name = "WY", 
                        Email = "1753993763qq.com" 
                        },
                    Version = "v1",
                    
                });
                // �ӿ�����
                m.OrderActionsBy(o => o.RelativePath);

                #region ��header�����token�����ݵ���̨  
                m.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���)ֱ���������������Bearer {token}(ע������֮����һ���ո�)��Bearer Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                //���ȫ�ְ�ȫ����
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

            #region ���Jwtoken��֤
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
                        IssuerSigningKey = securityKey,    // ���ܽ���Token����Կ

                        // �Ƿ���֤������
                        ValidateIssuer = true,
                        // ����������
                        ValidIssuer = "issuer",

                        // �Ƿ���֤������
                        // ����������
                        ValidateAudience = true,
                        ValidAudience = "audience",

                        // �Ƿ���֤������Ч��
                        //RequireExpirationTime = true,
                        ValidateLifetime = true,
                        // ÿ�ΰ䷢���ƣ�������Чʱ��
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
                                msg = "��Ч��Token",
                                err = "��Ч��Token"
                            };
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                            return Task.CompletedTask;
                        }
                    };
                });
            #endregion


            #region ����         
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
