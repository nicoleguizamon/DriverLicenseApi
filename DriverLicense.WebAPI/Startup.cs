using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DriverLicense.Interfaces.Repositories;
using DriverLicense.Maps;
using DriverLicense.Models.Common;
using DriverLicense.Models.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using DriverLicense.DAL;
using DriverLicense.Services;
using DriverLicense.Interfaces.Services;
using DriverLicense.Interfaces.Maps;
using DriverLicense.WebAPI.Extentions;
using Newtonsoft.Json;

namespace DriverLicense.WebAPI
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
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            services.Configure<IISOptions>(options =>
            {
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            services.AddMvc().AddJsonOptions(options => {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Driver License API",
                    Description = "Endpoints for mobile apps",
                    TermsOfService = "None",
                    License = new License { Name = "Use under License by Nico Leguizamon", Url = "http://www.nicoleguizamon.com" }
                });

                //Set the comments path for the swagger json and ui.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "DriverLicense.WebAPI.xml");
                c.IncludeXmlComments(xmlPath);
            });

            services.AddDbContext<DriverLicenseContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DriverLicenseDatabase")));

            services.AddOptions();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));


            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Services
            services.AddTransient<IAnswersService, AnswersService>();
            services.AddTransient<IQuestionsService, QuestionsService>();
            services.AddTransient<ILocationsService, LocationsService>();


            //Maps
            services.AddTransient<IAnswersMap, AnswersMap>();
            services.AddTransient<ILocationsMap, LocationsMap>();
            services.AddTransient<IQuestionsMap, QuestionsMap>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            app.UseHttpException();

            app.UseCors("CorsPolicy");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Workflow API v1");
            });
        }
    }
}
