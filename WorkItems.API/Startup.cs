using Emsa.Mared.Common.Controllers.Utility;
using Emsa.Mared.ContentManagement.WorkItems.Database;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemAttachments;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemComments;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemRelations;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItems;
using Emsa.Mared.ContentManagement.WorkItems.Utility;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using OrchardCore.Modules;
using System;

namespace Emsa.Mared.ContentManagement.WorkItems
{
	/// <summary>
	/// Implements the start-up flow.
	/// </summary>
	public sealed class Startup : StartupBase
	{
		#region [Properties]
		/// <summary>
		/// Gets the configuration.
		/// </summary>
		private readonly IConfiguration Configuration;

		/// <summary>
		/// The environment.
		/// </summary>
		private readonly IHostingEnvironment Environment;
		#endregion

		#region [Constructors]
		/// <summary>
		/// Initializes a new instance of the <see cref="Startup"/> class.
		/// </summary>
		/// 
		/// <param name="configuration">The configuration.</param>
		/// <param name="environment">The environment.</param>
		public Startup(IConfiguration configuration, IHostingEnvironment environment)
		{
			this.Configuration = configuration;
			this.Environment = environment;
		}
		#endregion

		#region [Methods]
		/// <summary>
		/// This method gets called by the runtime. Use this method to add services to the container.
		/// </summary>
		/// 
		/// <param name="services">The services.</param>
		[UsedImplicitly]
		public override void ConfigureServices(IServiceCollection services)
		{
			base.ConfigureServices(services);

			services
				.AddMvcCore(options =>
				{
					var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

					options.Filters.Add(new AuthorizeFilter(policy));

				})
				.AddApiExplorer()
				.AddJsonFormatters()
				.AddJsonOptions(options =>
				{
					options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
					options.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
				})
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services
				.AddDbContext<WorkItemsContext>(x => x.UseMySql(Configuration.GetConnectionString(AppSettingsKeys.DefaultConnection)), ServiceLifetime.Transient);
	
			services
				.AddScoped<IWorkItemRepository, WorkItemRepository>();
			services
				.AddScoped<IWorkItemCommentRepository, WorkItemCommentRepository>();
			services
				.AddScoped<IWorkItemParticipantRepository, WorkItemParticipantRepository>();
			services
				.AddScoped<IWorkItemAttachmentRepository, WorkItemAttachmentRepository>();
			services
				.AddScoped<IWorkItemRelationRepository, WorkItemRelationRepository>();
		}

		/// <summary>
		/// Configures the specified application builder.
		/// </summary>
		/// 
		/// <param name="routeBuilder">The routes builder.</param>
		/// <param name="applicationBuilder">The application builder.</param>
		/// <param name="serviceProvider">The service provider.</param>
		[UsedImplicitly]
		public override void Configure(IApplicationBuilder applicationBuilder, IRouteBuilder routeBuilder, IServiceProvider serviceProvider)
		{
			base.Configure(applicationBuilder, routeBuilder, serviceProvider);

			if (this.Environment.IsDevelopment())
			{
				applicationBuilder.UseDeveloperExceptionPage();
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
		}
		#endregion
	}
}