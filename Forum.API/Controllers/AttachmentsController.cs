using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Discussions.API.Contracts.Attachments;
using Emsa.Mared.Common;
using Emsa.Mared.Discussions.API.Database.Repositories.Attachments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

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

        #region [Methods] Utility
        /// <summary>
        /// Create a discussion attachment in repository.
        /// </summary>
        /// 
        /// <param name="discussionId">The discussion id.</param>
        /// <param name="attachmentToCreateDto">The attachment information.</param>
        [HttpPost]
        public async Task<IActionResult> Create(long discussionId, AttachmentToCreateDto attachmentToCreateDto)
        {
            var membership = new UserMembership
            {
                UserId = 1,
                GroupIds = new long[0],//new long[] { 1 }, //
                OrganizationsIds = new long[0]
            };

            var attachment = _mapper.Map<Attachment>(attachmentToCreateDto);

            attachment.DiscussionId = discussionId;

            var attachmentCreated = await _repo.Create(attachment, membership);

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
            var membership = new UserMembership
            {
                UserId = 10,
                GroupIds = new long[] { 1 }, //new long[0],
                OrganizationsIds = new long[0]
            };

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
        /// Get all attachments. 
        /// </summary>
        /// 
		/// <param name="parameters">The parameters.</param>
        [HttpGet("/api/discussions/attachments")]
        public async Task<IActionResult> GetAll([FromQuery] AttachmentParameters parameters = null)
        {
            var membership = new UserMembership
            {
                UserId = 10,
                GroupIds = new long[] { 1 }, //new long[0],
                OrganizationsIds = new long[0]
            };

            var attachments = await _repo.GetAll(parameters: null, membership: membership);

            var attachmentsToReturn = _mapper.Map<IEnumerable<AttachmentToReturnDto>>(attachments);

            return Ok(attachmentsToReturn);
        }

        /// <summary>
        /// Get a attachment by id. 
        /// </summary>
        /// 
        /// <param name="id">The attachment id.</param>
        [HttpGet("{attachmentId:long}")]
        public async Task<IActionResult> Get(long id)
        {
            var membership = new UserMembership
            {
                UserId = 10,
                GroupIds = new long[] { 1 }, //new long[0],
                OrganizationsIds = new long[0]
            };

            var attachment = await _repo.Get(id, membership);

            var attachmentToReturn = _mapper.Map<AttachmentToReturnDto>(attachment);

            return Ok(attachmentToReturn);
        }

        /// <summary>
        /// Update a attachment in repository. 
        /// </summary>
        /// 
        /// <param name="id">The attachment id.</param>
		/// <param name="attachmentToUpdateDto">The attchment information.</param>
        [HttpPut("{attachmentId:long}")]
        public async Task<IActionResult> Update(long id, AttachmentToUpdateDto attachmentToUpdateDto)
        {
            var membership = new UserMembership
            {
                UserId = 10,
                GroupIds = new long[] { 1 }, //new long[0],
                OrganizationsIds = new long[0]
            };

            var updateAttachment = _mapper.Map<Attachment>(attachmentToUpdateDto);

            updateAttachment.Id = id;

            var attachmentUpdated = await _repo.Update(updateAttachment, membership);

            return Created(new Uri($"{Request.GetDisplayUrl()}/{attachmentUpdated.Id}"), attachmentUpdated);
        }

        /// <summary>
        /// Delete a attachment in repository. 
        /// </summary>
        /// 
        /// <param name="discussionId">The discussion id.</param>
        /// <param name="attachmentId">The attachment id.</param>
        [HttpDelete("{attachmentId:long}")]
        public async Task<IActionResult> Delete(long discussionId, long attachmentId)
        {
            var membership = new UserMembership
            {
                UserId = 1,
                GroupIds = new long[0],
                OrganizationsIds = new long[0]
            };

            await _repo.Delete(attachmentId, membership);

            return Ok();
        }
        #endregion
    }
}