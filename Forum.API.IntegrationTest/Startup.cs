using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Forum.API.Data;
using Forum.API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Forum.API.IntegrationTest
{
    /// <summary>
	/// Implements the start-up flow.
	/// </summary>
    public class Startup
    {
        #region [Constants]
        /// <summary>
        /// The subject to test.
        /// </summary>
        private readonly string _subject = "Test {0}";

        /// <summary>
        /// The comment to test.
        /// </summary>
        private readonly string _comment = "Test comment {0}";

        /// <summary>
        /// The status to test.
        /// </summary>
        private readonly string _status = "Created";
        #endregion

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
            services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase());

            services.AddMvc();

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
        public void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            var context = applicationBuilder.ApplicationServices.GetService<DataContext>();
            AddTestData(context);

            // Enable MVC
            applicationBuilder.UseMvc();
        }

        private void AddTestData(DataContext context)
        {
            var _repo = new DiscussionRepository(context);

            var discussions = new List<Discussion>();
            for (int i = 1; i <= 5; i++)
            {
                discussions.Add(CreateDiscussion(i));
            }

            var user = new User
            {
                Id = 1,
                Name = "Peter"
            };
            context.User.Add(user);
            context.Discussions.AddRange(discussions);
            context.SaveChanges();
        }
        #region [Methods] Utility
        /// <summary>
        /// Create discussion.
        /// </summary>
        /// 
        /// <param name="index">The discussion index.</param>
        public Discussion CreateDiscussion(long? index = 0)
        {
            return new Discussion
            {
                Id = index.Value,
                Subject = string.Format(_subject, index),
                Comment = string.Format(_comment, index),
                CreatedDate = DateTime.Now,
                Status = _status,
                UserId = 1
            };

        }
        #endregion
    }
}
