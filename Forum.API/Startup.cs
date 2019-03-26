using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Forum.API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Forum.API
{
    /// <summary>
	/// Implements the start-up flow.
	/// </summary>
    public class Startup
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="Startup"/> class.
		/// </summary>
		/// 
		/// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
		/// This method gets called by the runtime. Use this method to add services to the container.
		/// </summary>
		/// 
		/// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(x => x.UseMySql
                (Configuration.GetConnectionString("DefaultConnection")));
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions
                (
                    options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                        options.SerializerSettings.Converters.Add(new StringEnumConverter(true));
                        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    }
                );
            services.AddScoped<IResponseRepository, ResponseRepository>();
            services.AddScoped<IDiscussionRepository, DiscussionRepository>();
            services.AddAutoMapper();
            services.AddCors();
        }

        /// <summary>
		/// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		/// </summary>
		/// 
		/// <param name="applicationBuilder">The application builder.</param>
		/// <param name="environment">The environment.</param>
        public void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                applicationBuilder.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                applicationBuilder.UseHsts();
            }
            // Enable CORS
            applicationBuilder.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            applicationBuilder.UseHttpsRedirection();
            // Enable MVC
            applicationBuilder.UseMvc();
        }
    }
}
