using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemsRelations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemsRelations
{
    /// <summary>
	/// Implements the work item relation entity framework configuration.
	/// </summary>
	/// 
	/// <seealso cref="IEntityTypeConfiguration{WorkItemRelation}" />
    public class WorkItemRelationConfiguration : IEntityTypeConfiguration<WorkItemRelation>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WorkItemRelation> builder)
        {
        }
    }
}
