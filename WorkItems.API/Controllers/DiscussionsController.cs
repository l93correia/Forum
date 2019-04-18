using AutoMapper;

using Emsa.Mared.Common.Security;
using Emsa.Mared.WorkItems.API.Contracts.WorkItemDiscussions;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemAttachments;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemComments;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemRelations;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems;
using Microsoft.AspNetCore.Http.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.Discussions.API.Controllers
{
    /// <summary>
    /// The discussion api controller allows to create, get, update and delete discussions.
    /// </summary>
    /// 
    /// <seealso cref="BaseWorkItemsController" />
    [ApiController]
    public class DiscussionsController : BaseWorkItemsController
    {
        #region [Attributes]
        /// <summary>
        /// The work items repository.
        /// </summary>
        private readonly IWorkItemRepository WorkItemRepository;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscussionsController"/> class.
        /// </summary>
        /// 
        /// <param name="mapper">The mapper.</param>
        /// <param name="workItemRepository">The repository.</param>
        /// <param name="attachmentsRepository">The repository.</param>
        /// <param name="commentRepository">The repository.</param>
        /// <param name="participantRepository">The repository.</param>
        /// <param name="relationRepository">The repository.</param>
        public DiscussionsController
        (
            IWorkItemRepository workItemRepository,
            IWorkItemAttachmentRepository attachmentsRepository,
            IWorkItemCommentRepository commentRepository,
            IWorkItemParticipantRepository participantRepository,
            IWorkItemRelationRepository relationRepository,
            IMapper mapper
        )
        : base (attachmentsRepository, commentRepository, participantRepository, relationRepository, mapper)
        {
            this.WorkItemRepository = workItemRepository;
        }
        #endregion

        #region [Methods] Discussions
        /// <summary>
        /// Create a discussion work item in the repository.
        /// </summary>
        /// 
        /// <param name="createDiscussion">The discussion to create.</param>
        [HttpPost]
        public virtual async Task<IActionResult> Create(DiscussionToCreate createDiscussion)
        {
            var membership = this.CreateMembership();
            var discussion = this.Mapper.Map<WorkItem>(createDiscussion);
            discussion.UserId = membership.UserId;
            discussion.Type = WorkItemType.Discussion;

            discussion = await this.WorkItemRepository.CreateAsync(discussion, membership);

            return Created(new Uri
            (
                $"{this.Request.GetDisplayUrl()}/{discussion.Id}"),
                this.Mapper.Map<DiscussionToReturn>(discussion)
            );
        }

        /// <summary>
        /// Update a discussion work item in the repository.
        /// </summary>
        /// 
        /// <param name="id">The discussion id.</param>
        /// <param name="updateDiscussion">The discussion to update.</param>
        [HttpPut("{id:long}")]
        public virtual async Task<IActionResult> Update(long id, DiscussionToUpdate updateDiscussion)
        {
            var membership = this.CreateMembership();
            var discussion = this.Mapper.Map<WorkItem>(updateDiscussion);
            discussion.Id = id;
            
            if(await this.WorkItemRepository.IsType(id, WorkItemType.Discussion))
            {
                await this.WorkItemRepository.UpdateAsync(discussion, membership);

                return this.Ok();
            }
            return NotFound();
        }

        /// <summary>
        /// Deletes a discussion work item from the repository.
        /// </summary>
        /// 
        /// <param name="id">The discussion id.</param>
        [HttpDelete("{id:long}")]
        public virtual async Task<IActionResult> Delete(long id)
        {
            var membership = this.CreateMembership();

            if (await this.WorkItemRepository.IsType(id, WorkItemType.Discussion))
            {
                await this.WorkItemRepository.DeleteAsync(id, membership);

                return this.Ok();
            }
            return NotFound();            
        }

        /// <summary>
        /// Get a discussion work item by id from the repository.
        /// </summary>
        /// 
        /// <param name="id">The discussion id.</param>
        [HttpGet("{id:long}")]
        public virtual async Task<IActionResult> Get(long id)
        {
            var membership = this.CreateMembership();

            if (await this.WorkItemRepository.IsType(id, WorkItemType.Discussion))
            {
                var discussion = await this.WorkItemRepository.GetAsync(id, membership);
                var returnDiscussion = this.Mapper.Map<DiscussionToReturn>(discussion);

                return this.Ok(returnDiscussion);
            }
            return NotFound();                
        }

        /// <summary>
        /// Get all the discussion work items from the repository.
        /// </summary>
        /// 
        /// <param name="parameters">The parameters.</param>
        [HttpGet]
        public virtual async Task<IActionResult> GetAll([FromQuery] WorkItemParameters parameters = null)
        {
            var membership = this.CreateMembership();
            parameters.WorkItemType = WorkItemType.Discussion;

            var discussions = await this.WorkItemRepository.GetAllAsync(parameters, membership);
            var returnDiscussions = this.Mapper.Map<IEnumerable<DiscussionToReturn>>(discussions);

            return this.Ok(returnDiscussions);
        }
        #endregion   
	}
}