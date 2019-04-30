using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Discussions.API.Contracts.Attachments;
using Emsa.Mared.Common;
using Emsa.Mared.Common.Claims;
using Emsa.Mared.Common.Security;
using Emsa.Mared.Discussions.API.Database.Repositories.Attachments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Emsa.Mared.Discussions.API.Controllers
{
    /// <summary>
    /// The attachment api controller allows to create, get, update and delete attachments from discussions.
    /// </summary>
    /// 
    /// <seealso cref="ControllerBase" />
    [Route("api/discussions/[controller]")]
    [Route("api/discussions/{discussionId:long}/[controller]")]
    [ApiController]
    public class AttachmentsController : ControllerBase
    {
        #region [Attributes]
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IAttachmentRepository _repo;

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper _mapper;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentController"/> class.
        /// </summary>
        /// 
        /// <param name="mapper">The mapper.</param>
        /// <param name="repo">The repository.</param>
        public AttachmentsController(IAttachmentRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
		#endregion

		#region [Interface Methods]
		/// <summary>
		/// Create a discussion attachment in repository.
		/// </summary>
		/// 
		/// <param name="discussionId">The discussion id.</param>
		/// <param name="attachmentToCreateDto">The attachment information.</param>
		[HttpPost]
        public async Task<IActionResult> Create(long discussionId, AttachmentToCreateDto attachmentToCreateDto)
        {
            var membership = CreateMembership();

			var attachment = _mapper.Map<Attachment>(attachmentToCreateDto);

            attachment.DiscussionId = discussionId;

            var attachmentCreated = await _repo.CreateAsync(attachment, membership);

            var attachmentToReturn = _mapper.Map<AttachmentToReturnDto>(attachmentCreated);

            return Created(new Uri($"{Request.GetDisplayUrl()}/{attachmentToReturn.Id}"), attachmentToReturn
);
        }

        /// <summary>
        /// Get all discussion attachments. 
        /// </summary>
        /// 
        /// <param name="discussionId">The discussion id.</param>
		/// <param name="parameters">The parameters.</param>
        [HttpGet("/api/discussions/{discussionId:long}/attachments")]
        public async Task<IActionResult> GetAllByDiscussion(long discussionId, [FromQuery] AttachmentParameters parameters = null)
        {
            var membership = CreateMembership();

			var attachments = await _repo.GetByDiscussion(discussionId, parameters, membership);

            var attachmentToReturn = attachments.Select(p => _mapper.Map<AttachmentToReturnDto>(p));

            //this.Response.AddHeader("Pagination", new PaginationHeader
            //(
            //    responses.PageNumber,
            //    responses.PageSize,
            //    responses.TotalPages,
            //    responses.TotalCount
            //));

            return Ok(attachmentToReturn);
        }

		/// <summary>
		/// Get a attachment by id. 
		/// </summary>
		/// 
		/// <param name="discussionId">The discussion id.</param>
		/// <param name="attachmentId">The attachment id.</param>
		[HttpGet("/api/discussions/{discussionId:long}/attachments{attachmentId:long}")]
        public async Task<IActionResult> Get(long discussionId, long attachmentId)
        {
            var membership = CreateMembership();

			var attachment = await _repo.GetAsync(attachmentId, membership);

            var attachmentToReturn = _mapper.Map<AttachmentToReturnDto>(attachment);

            return Ok(attachmentToReturn);
        }

        /// <summary>
        /// Update a attachment in repository. 
        /// </summary>
        /// 
        /// <param name="discussionId">The discussion id.</param>
        /// <param name="attachmentId">The attachment id.</param>
		/// <param name="attachmentToUpdateDto">The attchment information.</param>
        [HttpPut("/api/discussions/{discussionId:long}/attachments/{attachmentId:long}")]
        public async Task<IActionResult> Update(long discussionId, long attachmentId, AttachmentToUpdateDto attachmentToUpdateDto)
        {
            var membership = CreateMembership();

			var updateAttachment = _mapper.Map<Attachment>(attachmentToUpdateDto);

            updateAttachment.Id = attachmentId;
            updateAttachment.DiscussionId = discussionId;

            var attachmentUpdated = await _repo.UpdateAsync(updateAttachment, membership);

            return Created(new Uri($"{Request.GetDisplayUrl()}/{attachmentUpdated.Id}"), attachmentUpdated);
        }

        /// <summary>
        /// Delete a attachment in repository. 
        /// </summary>
        /// 
        /// <param name="discussionId">The discussion id.</param>
        /// <param name="attachmentId">The attachment id.</param>
        [HttpDelete("/api/discussions/{discussionId:long}/attachments/{attachmentId:long}")]
        public async Task<IActionResult> Delete(long discussionId, long attachmentId)
        {
            var membership = CreateMembership();

			await _repo.DeleteAsync(attachmentId, membership);

            return Ok();
        }
		#endregion

		#region [Methods] Utility
		/// <summary>
		/// Create a membership.
		/// </summary>
		public UserMembership CreateMembership()
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