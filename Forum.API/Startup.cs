using AutoMapper;
using Emsa.Mared.Common.Controllers;
using Emsa.Mared.Common.Controllers.Utility;
using Emsa.Mared.Discussions.API.Database;
using Emsa.Mared.Discussions.API.Database.Repositories;
using Emsa.Mared.Discussions.API.Database.Repositories.Attachments;
using Emsa.Mared.Discussions.API.Database.Repositories.Discussions;
using Emsa.Mared.Discussions.API.Database.Repositories.Participants;
using Emsa.Mared.Discussions.API.Database.Repositories.Responses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;

namespace Emsa.Mared.Discussions.API
{
    /// <summary>
	/// Implements the start-up flow.
	/// </summary>
    public class Startup
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
		/// Initializes a new instance of the <see cref="Startup"/> class.
		/// </summary>
		/// 
		/// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
		/// This method gets called by the runtime. Use this method to add services to the container.
		/// </summary>
		/// 
		/// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DiscussionContext>(x => x.UseMySql
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
            services.AddScoped<IParticipantRepository, ParticipantRepository>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
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
                applicationBuilder.UseExceptionHandler(builder =>
                {
                    builder.Run(Utilities.ProcessExceptionAsync);
                });
                applicationBuilder.UseDatabaseErrorPage();
                
            }
            else
            {
                applicationBuilder.UseExceptionHandler(builder =>
                {
                    builder.Run(Utilities.ProcessExceptionAsync);
                });
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
