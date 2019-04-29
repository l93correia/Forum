using AutoMapper;
using Emsa.Mared.Common.Claims;
using Emsa.Mared.Common.Security;
using Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemAttachments;
using Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemComments;
using Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemParticipants;
using Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemRelations;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemAttachments;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemComments;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemRelations;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.ContentManagement.WorkItems.Controllers
{
    /// <summary>
    /// The base work items api controller that allows to
    /// create, get, update and delete attachments, comments, participants and relations.
    /// </summary>
    /// 
    /// <seealso cref="ControllerBase" />
    [ApiController]
    [Route("api/[controller]/")]
    public abstract class BaseWorkItemsController : ControllerBase
    {
        #region [Attributes]
        /// <summary>
        /// The attachments repository.
        /// </summary>
        protected readonly IWorkItemAttachmentRepository AttachmentRepository;

        /// <summary>
        /// The comments repository.
        /// </summary>
        protected readonly IWorkItemCommentRepository CommentRepository;

        /// <summary>
        /// The participants repository.
        /// </summary>
        protected readonly IWorkItemParticipantRepository ParticipantRepository;

        /// <summary>
        /// The relations repository.
        /// </summary>
        protected readonly IWorkItemRelationRepository RelationRepository;

        /// <summary>
        /// The mapper.
        /// </summary>
        protected readonly IMapper Mapper;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWorkItemsController"/> class.
        /// </summary>
        /// 
        /// <param name="mapper">The mapper.</param>
        /// <param name="attachmentRepository">The repository.</param>
        /// <param name="commentRepository">The repository.</param>
        /// <param name="participantRepository">The repository.</param>
        /// <param name="relationRepository">The repository.</param>
        protected BaseWorkItemsController
        (
            IWorkItemAttachmentRepository attachmentRepository,
            IWorkItemCommentRepository commentRepository,
            IWorkItemParticipantRepository participantRepository,
            IWorkItemRelationRepository relationRepository,
            IMapper mapper
        )
        {
            this.AttachmentRepository = attachmentRepository;
            this.CommentRepository = commentRepository;
            this.ParticipantRepository = participantRepository;
            this.RelationRepository = relationRepository;
            this.Mapper = mapper;
        }
        #endregion

        #region [Methods] Attachments
        /// <summary>
        /// Create an attachment in the repository.
        /// </summary>
        /// 
        /// <param name="workItemId">The attachments work item id.</param>
        /// <param name="createAttachment">The attachment to create.</param>
        [HttpPost("{workItemId:long}/attachments")]
        public virtual async Task<IActionResult> CreateAttachment(long workItemId, AttachmentToCreate createAttachment)
        {
            var membership = this.CreateMembership();
            var attachment = this.Mapper.Map<WorkItemAttachment>(createAttachment);
            attachment.WorkItemId = workItemId;
            attachment.UserId = membership.UserId;

            attachment = await this.AttachmentRepository.CreateAsync(attachment, membership);

            return Created(new Uri
            (
                $"{this.Request.GetDisplayUrl()}/{attachment.Id}"),
                this.Mapper.Map<AttachmentToReturn>(attachment)
            );
        }

        /// <summary>
        /// Update an attachment in the repository.
        /// </summary>
        /// 
        /// <param name="id">The attachment id.</param>
        /// <param name="workItemId">The attachments work item id.</param>
        /// <param name="updateAttachment">The attachment to update.</param>
        [HttpPut("{workItemId:long}/attachments/{id:long}")]
        public virtual async Task<IActionResult> UpdateAttachment(long id, long workItemId, AttachmentToUpdate updateAttachment)
        {
            var membership = this.CreateMembership();
            var attachment = this.Mapper.Map<WorkItemAttachment>(updateAttachment);
            attachment.Id = id;

            if(await this.AttachmentRepository.BelongsToWorkItem(workItemId, id))
            {
                await this.AttachmentRepository.UpdateAsync(attachment, membership);

                return this.Ok();
            }

            return this.NotFound();
        }

        /// <summary>
        /// Deletes an attachment from the repository.
        /// </summary>
        /// 
        /// <param name="id">The attachment id.</param>
        /// <param name="workItemId">The attachments work item id.</param>
        [HttpDelete("{workItemId:long}/attachments/{id:long}")]
        public virtual async Task<IActionResult> DeleteAttachment(long id, long workItemId)
        {
            var membership = this.CreateMembership();

            if (await this.AttachmentRepository.BelongsToWorkItem(workItemId, id))
            {
                await this.AttachmentRepository.DeleteAsync(id, membership);

                return Ok();
            }

            return this.NotFound();           
        }

        /// <summary>
        /// Get an attachment by id from the repository.
        /// </summary>
        /// 
        /// <param name="id">The attachment id.</param>
        /// <param name="workItemId">The attachments work item id.</param>
        [HttpGet("{workItemId:long}/attachments/{id:long}")]
        public virtual async Task<IActionResult> GetAttachment(long id, long workItemId)
        {
            var membership = this.CreateMembership();

            if (await this.AttachmentRepository.BelongsToWorkItem(workItemId, id))
            {
                var attachment = await this.AttachmentRepository.GetAsync(id, membership);
                var returnAttachment = this.Mapper.Map<AttachmentToReturn>(attachment);

                return this.Ok(returnAttachment);
            }
            return this.NotFound();
        }

        /// <summary>
        /// Get all the attachment from the repository.
        /// </summary>
        /// 
        /// <param name="workItemId">The attachments work item id.</param>
        /// <param name="parameters">The parameters.</param>
        [HttpGet("{workItemId:long}/attachments")]
        public virtual async Task<IActionResult> GetAllAttachments(long workItemId, [FromQuery] WorkItemAttachmentParameters parameters = null)
        {
            var membership = this.CreateMembership();
            parameters.WorkItemId = workItemId;

            var attachments = await this.AttachmentRepository.GetAllAsync(parameters, membership);
            var returnAttachments = this.Mapper.Map<IEnumerable<AttachmentToReturn>>(attachments);

            return this.Ok(returnAttachments);
        }
        #endregion

        #region [Methods] Comments
        /// <summary>
        /// Create a comment in the repository.
        /// </summary>
        /// 
        /// <param name="workItemId">The attachments work item id.</param>
        /// <param name="createComment">The comment to create.</param>
        [HttpPost("{workItemId:long}/comments")]
        public virtual async Task<IActionResult> CreateComment(long workItemId, CommentToCreate createComment)
        {
            var membership = this.CreateMembership();
            var comment = this.Mapper.Map<WorkItemComment>(createComment);
            comment.WorkItemId = workItemId;
            comment.UserId = membership.UserId;

            comment = await this.CommentRepository.CreateAsync(comment, membership);

            return Created(new Uri
            (
                $"{this.Request.GetDisplayUrl()}/{comment.Id}"),
                this.Mapper.Map<CommentToReturn>(comment)
            );
        }

        /// <summary>
        /// Update a comment in the repository.
        /// </summary>
        /// 
        /// <param name="id">The comment id.</param>
        /// <param name="workItemId">The attachments work item id.</param>
        /// <param name="updateComment">The comment to update.</param>
        [HttpPut("{workItemId:long}/comments/{id:long}")]
        public virtual async Task<IActionResult> UpdateComment(long id, long workItemId, CommentToUpdate updateComment)
        {
            var membership = this.CreateMembership();
            var comment = this.Mapper.Map<WorkItemComment>(updateComment);
            comment.Id = id;

            if (await this.CommentRepository.BelongsToWorkItem(workItemId, id))
            {
                await this.CommentRepository.UpdateAsync(comment, membership);

                return this.Ok();
            }

            return this.NotFound();
        }

        /// <summary>
        /// Deletes a comment from the repository.
        /// </summary>
        /// 
        /// <param name="id">The comment id.</param>
        /// <param name="workItemId">The work item id.</param>
        [HttpDelete("{workItemId:long}/comments/{id:long}")]
        public virtual async Task<IActionResult> DeleteComment(long id, long workItemId)
        {
            var membership = this.CreateMembership();

            if (await this.CommentRepository.BelongsToWorkItem(workItemId, id))
            {
                await this.CommentRepository.DeleteAsync(id, membership);

                return Ok();
            }

            return this.NotFound();
        }

        /// <summary>
        /// Get a comment by id from the repository.
        /// </summary>
        /// 
        /// <param name="id">The comment id.</param>
        /// <param name="workItemId">The comment work item id.</param>
        [HttpGet("{workItemId:long}/comments/{id:long}")]
        public virtual async Task<IActionResult> GetComment(long id, long workItemId)
        {
            var membership = this.CreateMembership();

            if (await this.CommentRepository.BelongsToWorkItem(workItemId, id))
            {
                var comment = await this.CommentRepository.GetAsync(id, membership);
                var returnComment = this.Mapper.Map<AttachmentToReturn>(comment);

                return this.Ok(returnComment);
            }
            return this.NotFound();
        }

        /// <summary>
        /// Get all the comments from the repository.
        /// </summary>
        /// 
        /// <param name="workItemId">The comments work item id.</param>
        /// <param name="parameters">The parameters.</param>
        [HttpGet("{workItemId:long}/comments")]
        public virtual async Task<IActionResult> GetAllComments(long workItemId, [FromQuery] WorkItemCommentParameters parameters = null)
        {
            var membership = this.CreateMembership();
            parameters.WorkItemId = workItemId;

            var comments = await this.CommentRepository.GetAllAsync(parameters, membership);
            var returnComments = this.Mapper.Map<IEnumerable<CommentToReturn>>(comments);

            return this.Ok(returnComments);
        }
        #endregion

        #region [Methods] Participants
        /// <summary>
        /// Create a participant in the repository.
        /// </summary>
        /// 
        /// <param name="workItemId">The participants work item id.</param>
        /// <param name="createParticipant">The participant to create.</param>
        [HttpPost("{workItemId:long}/participants")]
        public virtual async Task<IActionResult> CreateParticipant(long workItemId, ParticipantToCreate createParticipant)
        {
            var membership = this.CreateMembership();
            var participant = this.Mapper.Map<WorkItemParticipant>(createParticipant);
            participant.WorkItemId = workItemId;
            participant.UserId = membership.UserId;

            participant = await this.ParticipantRepository.CreateAsync(participant, membership);

            return Created(new Uri
            (
                $"{this.Request.GetDisplayUrl()}/{participant.Id}"),
                this.Mapper.Map<ParticipantToReturn>(participant)
            );
        }

        /// <summary>
        /// Update a participant in the repository.
        /// </summary>
        /// 
        /// <param name="id">The comment id.</param>
        /// <param name="workItemId">The participants work item id.</param>
        /// <param name="updateParticipant">The participant to update.</param>
        [HttpPut("{workItemId:long}/participants/{id:long}")]
        public virtual async Task<IActionResult> UpdateParticipant(long id, long workItemId, ParticipantToUpdate updateParticipant)
        {
            var membership = this.CreateMembership();
            var participant = this.Mapper.Map<WorkItemParticipant>(updateParticipant);
            participant.Id = id;

            if (await this.ParticipantRepository.BelongsToWorkItem(workItemId, id))
            {
                await this.ParticipantRepository.UpdateAsync(participant, membership);

                return this.Ok();
            }

            return this.NotFound();
        }

        /// <summary>
        /// Deletes a participant from the repository.
        /// </summary>
        /// 
        /// <param name="id">The participant id.</param>
        /// <param name="workItemId">The work item id.</param>
        [HttpDelete("{workItemId:long}/participants/{id:long}")]
        public virtual async Task<IActionResult> DeleteParticipant(long id, long workItemId)
        {
            var membership = this.CreateMembership();

            if (await this.ParticipantRepository.BelongsToWorkItem(workItemId, id))
            {
                await this.ParticipantRepository.DeleteAsync(id, membership);

                return Ok();
            }

            return this.NotFound();
        }

        /// <summary>
        /// Get a participant by id from the repository.
        /// </summary>
        /// 
        /// <param name="id">The participant id.</param>
        /// <param name="workItemId">The participant work item id.</param>
        [HttpGet("{workItemId:long}/participants/{id:long}")]
        public virtual async Task<IActionResult> GetParticipant(long id, long workItemId)
        {
            var membership = this.CreateMembership();

            if (await this.ParticipantRepository.BelongsToWorkItem(workItemId, id))
            {
                var participant = await this.ParticipantRepository.GetAsync(id, membership);
                var returnParticipant = this.Mapper.Map<AttachmentToReturn>(participant);

                return this.Ok(returnParticipant);
            }
            return this.NotFound();
        }

        /// <summary>
        /// Get all the participants from the repository.
        /// </summary>
        /// 
        /// <param name="workItemId">The participants work item id.</param>
        /// <param name="parameters">The parameters.</param>
        [HttpGet("{workItemId:long}/participants")]
        public virtual async Task<IActionResult> GetAllParticipants(long workItemId, [FromQuery] WorkItemParticipantParameters parameters = null)
        {
            var membership = this.CreateMembership();
            parameters.WorkItemId = workItemId;

            var participants = await this.ParticipantRepository.GetAllAsync(parameters, membership);
            var returnParticipants = this.Mapper.Map<IEnumerable<CommentToReturn>>(participants);

            return this.Ok(returnParticipants);
        }
        #endregion

        #region [Methods] Relations
        /// <summary>
        /// Create a relation in the repository.
        /// </summary>
        /// 
        /// <param name="workItemId">The relations work item id.</param>
        /// <param name="createRelation">The relations to create.</param>
        [HttpPost("{workItemId:long}/relations")]
        public virtual async Task<IActionResult> CreateRelation(long workItemId, RelationToCreate createRelation)
        {
            var membership = this.CreateMembership();
            var relation = this.Mapper.Map<WorkItemRelation>(createRelation);
            relation.RelatedFromWorkItemId = workItemId;
            relation.UserId = membership.UserId;

            relation = await this.RelationRepository.CreateAsync(relation, membership);

            return Created(new Uri
            (
                $"{this.Request.GetDisplayUrl()}/{relation.Id}"),
                this.Mapper.Map<ParticipantToReturn>(relation)
            );
        }

        /// <summary>
        /// Update a relation in the repository.
        /// </summary>
        /// 
        /// <param name="id">The relation id.</param>
        /// <param name="workItemId">The relations work item id.</param>
        /// <param name="updateRelation">The relation to update.</param>
        [HttpPut("{workItemId:long}/relations/{id:long}")]
        public virtual async Task<IActionResult> UpdateRelation(long id, long workItemId, RelationToUpdate updateRelation)
        {
            var membership = this.CreateMembership();
            var relation = this.Mapper.Map<WorkItemRelation>(updateRelation);
            relation.Id = id;

            if (await this.RelationRepository.BelongsToWorkItem(workItemId, id))
            {
                await this.RelationRepository.UpdateAsync(relation, membership);

                return this.Ok();
            }

            return this.NotFound();
        }

        /// <summary>
        /// Deletes a relation from the repository.
        /// </summary>
        /// 
        /// <param name="id">The relation id.</param>
        /// <param name="workItemId">The work item id.</param>
        [HttpDelete("{workItemId:long}/relations/{id:long}")]
        public virtual async Task<IActionResult> DeleteRelation(long id, long workItemId)
        {
            var membership = this.CreateMembership();

            if (await this.RelationRepository.BelongsToWorkItem(workItemId, id))
            {
                await this.RelationRepository.DeleteAsync(id, membership);

                return Ok();
            }

            return this.NotFound();
        }

        /// <summary>
        /// Get a relation by id from the repository.
        /// </summary>
        /// 
        /// <param name="id">The relation id.</param>
        /// <param name="workItemId">The relation work item id.</param>
        [HttpGet("{workItemId:long}/relations/{id:long}")]
        public virtual async Task<IActionResult> GetRelation(long id, long workItemId)
        {
            var membership = this.CreateMembership();

            if (await this.RelationRepository.BelongsToWorkItem(workItemId, id))
            {
                var relation = await this.RelationRepository.GetAsync(id, membership);
                var returnRelation = this.Mapper.Map<AttachmentToReturn>(relation);

                return this.Ok(returnRelation);
            }
            return this.NotFound();
        }

        /// <summary>
        /// Get all the relations from the repository.
        /// </summary>
        /// 
        /// <param name="workItemId">The relations work item id.</param>
        /// <param name="parameters">The parameters.</param>
        [HttpGet("{workItemId:long}/relations")]
        public virtual async Task<IActionResult> GetAllRelations(long workItemId, [FromQuery] WorkItemRelationParameters parameters = null)
        {
            var membership = this.CreateMembership();
            parameters.WorkItemId = workItemId;

            var relations = await this.RelationRepository.GetAllAsync(parameters, membership);
            var returnRelations = this.Mapper.Map<IEnumerable<CommentToReturn>>(relations);

            return this.Ok(returnRelations);
        }
        #endregion

        #region [Methods] Utility
        /// <summary>
        /// Create a membership.
        /// </summary>
        protected UserMembership CreateMembership()
		{
			long userId = 1;
			long[] groupsIds = new long[0];
			long[] organizationIds = new long[0];

			var headers = this.Request.Headers;
			if (headers.TryGetValue("UserId", out StringValues values))
			{
				userId = long.Parse(values.FirstOrDefault());
			}
			if (headers.TryGetValue("GroupId", out values))
			{
				groupsIds = values.Select(value => long.Parse(value)).ToArray();
			}
			if (headers.TryGetValue("OrganizationId", out values))
			{
				organizationIds = values.Select(value => long.Parse(value)).ToArray();
			}

			return new UserMembership
			{
				UserId = userId,
				Groups = new GroupClaimType[0],
				Organizations = new OrganizationClaimType[0]
			};
		}
		#endregion
	}
}