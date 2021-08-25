using BLL.Helpers;
using BLL.Interfaces;
using BLL.Services;
using DAL.EF;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using Training.AutoMapper;
using Training.Middleware;

namespace Training
{
    public class Startup
    {
        readonly string myAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            //var builder = new ConfigurationBuilder()
            //                 .AddEnvironmentVariables()
            //                .AddConfiguration(configuration);
            //// create config
            //Configuration = builder.Build();

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //https://developer.mozilla.org/ru/docs/Web/HTTP/CORS
            //https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1
            services.AddCors(options =>
            {
                options.AddPolicy(name: myAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("https://localhost:5001")
                                      .WithMethods("PUT", "PATÑH");
                                      //.WithHeaders("x-custom-header");
                                  });
            });
            string connection = Configuration.GetConnectionString("DefaultConnection");
            //services.AddMvcCore().AddApiExplorer().AddAuthorization();
            services.AddControllers();
            services.AddDbContext<AppDBContext>(options => options.
                UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 26))), ServiceLifetime.Transient);

            services.AddAutoMapper(typeof(AutoMapperProfile));

            #region Config
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            var mailAddresConfigSection = Configuration.GetSection("EmailConfiguration");
            //services.Configure<SmtpConfig>(mailAddresConfigSection);
            services.AddSingleton<IEmailConfiguration>(Configuration.GetSection("EmailConfiguration")
                .Get<EmailConfiguration>());
            var smtpConfig = mailAddresConfigSection.Get<EmailConfiguration>();
            #endregion

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path.Combine(
                Directory.GetCurrentDirectory(), appSettings.DirectoryForFireBaseConfig,
                appSettings.FireBaseConfig["FileName"]));

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPythonLibService, PythonLibService>();
            services.AddScoped<IWebParseService, WebParseService>();
            services.AddScoped<IComputerVision, ComputerVision>();
            services.AddScoped<IOrderLineService, OrderLineService>();

            #region FluentEmail_Smtp
            SmtpClient smtp = new SmtpClient
            {
                //The address of the SMTP server (I'll take mailbox 126 as an example, which can be set according to the specific mailbox you use)
                Host = smtpConfig.SmtpHost,
                Port = smtpConfig.SmtpPort,
                UseDefaultCredentials = true,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                //Enter the user name and password of your sending SMTP server here
                Credentials = new NetworkCredential(smtpConfig.SmtpEmail, smtpConfig.SmtpPassword)
            };
            services
                .AddFluentEmail(smtpConfig.SmtpEmail)
                .AddSmtpSender(smtp); //configure host and port
            #endregion

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Training API", Version = "v1", });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDBContext applicationContext)
        {
            applicationContext.Database.Migrate();
            DbInitializer.Initialize(applicationContext);
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<ExeptionMeddleware>();
            app.UseRouting();
            app.UseCors(myAllowSpecificOrigins);
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
                
    }
}
