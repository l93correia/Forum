using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemEvents;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemAttachments;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemComments;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemRelations;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Emsa.Mared.ContentManagement.WorkItems.Controllers
{
    /// <summary>
    /// The events api controller allows to create, get, update and delete events.
    /// </summary>
    /// 
    /// <seealso cref="BaseWorkItemsController" />
    [ApiController]
	[AllowAnonymous]
	public class EventsController : BaseWorkItemsController
    {
        #region [Attributes]
        /// <summary>
        /// The work items repository.
        /// </summary>
        private readonly IWorkItemRepository WorkItemRepository;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="EventsController"/> class.
        /// </summary>
        /// 
        /// <param name="mapper">The mapper.</param>
        /// <param name="workItemRepository">The repository.</param>
        /// <param name="attachmentsRepository">The repository.</param>
        /// <param name="commentRepository">The repository.</param>
        /// <param name="participantRepository">The repository.</param>
        /// <param name="relationRepository">The repository.</param>
        public EventsController
        (
            IWorkItemRepository workItemRepository,
            IWorkItemAttachmentRepository attachmentsRepository,
            IWorkItemCommentRepository commentRepository,
            IWorkItemParticipantRepository participantRepository,
            IWorkItemRelationRepository relationRepository,
            IMapper mapper
        )
        : base(attachmentsRepository, commentRepository, participantRepository, relationRepository, mapper)
        {
            this.WorkItemRepository = workItemRepository;
        }
        #endregion

        #region [Methods] Events
        /// <summary>
        /// Create a event work item in the repository.
        /// </summary>
        /// 
        /// <param name="createEvent">The event to create.</param>
        [HttpPost]
        public virtual async Task<IActionResult> Create(EventToCreate createEvent)
        {
            var membership = this.CreateMembership();
            var @event = this.Mapper.Map<WorkItem>(createEvent);
            @event.UserId = membership.UserId;
            @event.Type = WorkItemType.Event;

            @event = await this.WorkItemRepository.CreateAsync(@event, membership);

            return Created(new Uri
            (
                $"{this.Request.GetDisplayUrl()}/{@event.Id}"),
                this.Mapper.Map<EventToReturn>(@event)
            );
        }

        /// <summary>
        /// Update a event work item in the repository.
        /// </summary>
        /// 
        /// <param name="id">The event id.</param>
        /// <param name="updateEvent">The event to update.</param>
        [HttpPut("{id:long}")]
        public virtual async Task<IActionResult> Update(long id, EventToUpdate updateEvent)
        {
            var membership = this.CreateMembership();
            var @event = this.Mapper.Map<WorkItem>(updateEvent);
            @event.Id = id;

            if (await this.WorkItemRepository.IsType(id, WorkItemType.Event))
            {
                await this.WorkItemRepository.UpdateAsync(@event, membership);

                return this.Ok();
            }
            return NotFound();
        }

        /// <summary>
        /// Deletes a event work item from the repository.
        /// </summary>
        /// 
        /// <param name="id">The event id.</param>
        [HttpDelete("{id:long}")]
        public virtual async Task<IActionResult> Delete(long id)
        {
            var membership = this.CreateMembership();

            if (await this.WorkItemRepository.IsType(id, WorkItemType.Event))
            {
                await this.WorkItemRepository.DeleteAsync(id, membership);

                return this.Ok();
            }
            return NotFound();
        }

        /// <summary>
        /// Get a event work item by id from the repository.
        /// </summary>
        /// 
        /// <param name="id">The event id.</param>
        [HttpGet("{id:long}")]
        public virtual async Task<IActionResult> Get(long id)
        {
            var membership = this.CreateMembership();

            if (await this.WorkItemRepository.IsType(id, WorkItemType.Event))
            {
                var @event = await this.WorkItemRepository.GetAsync(id, membership);
                var returnEvent = this.Mapper.Map<EventToReturn>(@event);

                return this.Ok(returnEvent);
            }
            return NotFound();
        }

        /// <summary>
        /// Get all the events work items from the repository.
        /// </summary>
        /// 
        /// <param name="parameters">The parameters.</param>
        [HttpGet]
        public virtual async Task<IActionResult> GetAll([FromQuery] WorkItemParameters parameters = null)
        {
            var membership = this.CreateMembership();
            parameters.WorkItemType = WorkItemType.Event;

            var events = await this.WorkItemRepository.GetAllAsync(parameters, membership);
            var returnEvents = this.Mapper.Map<IEnumerable<EventToReturn>>(events);

            return this.Ok(returnEvents);
        }
        #endregion
    }
}