using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emsa.Mared.Common;
using Emsa.Mared.Common.Security;
using Emsa.Mared.Discussions.API.Contracts;
using Emsa.Mared.Discussions.API.Database.Repositories.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Emsa.Mared.Discussions.API.Controllers
{
    /// <summary>
    /// The responses api controller allows to create, get, update and delete responses from discussions.
    /// </summary>
    /// 
    /// <seealso cref="ControllerBase" />
    [Route("api/discussions/[controller]")]
    [Route("api/discussions/{discussionId:long}/[controller]")]
    [ApiController]
    public class ResponsesController : ControllerBase
    {
        #region [Attributes]
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IResponseRepository _repo;

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper _mapper;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponsesController"/> class.
        /// </summary>
        /// 
        /// <param name="mapper">The mapper.</param>
        /// <param name="repo">The repository.</param>
        public ResponsesController(IResponseRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
		#endregion

		#region [Interface Methods]
		/// <summary>
		/// Create a discussion response in repository.
		/// </summary>
		/// 
		/// <param name="discussionId">The discussion id.</param>
		/// <param name="responseToCreateDto">The response information.</param>
		[HttpPost]
        public async Task<IActionResult> Create(long discussionId, ResponseToCreateDto responseToCreateDto)
        {
            var membership = CreateMembership();

			var response = _mapper.Map<Response>(responseToCreateDto);

            response.DiscussionId = discussionId;

            var responseCreated = await _repo.CreateAsync(response, membership);

            var responseToReturn = _mapper.Map<ResponseToReturnDto>(responseCreated);

            return Created(new Uri($"{Request.GetDisplayUrl()}/{responseToReturn.Id}"), responseToReturn);
        }

        /// <summary>
        /// Get all discussion responses. 
        /// </summary>
        /// 
        /// <param name="discussionId">The discussion id.</param>
		/// <param name="parameters">The parameters.</param>
        [HttpGet("/api/discussions/{discussionId:long}/responses")]
        public async Task<IActionResult> GetAllByDiscussion(long discussionId, [FromQuery] ResponseParameters parameters = null)
        {
            var membership = CreateMembership();

			var responses = await _repo.GetByDiscussion(discussionId, parameters, membership);

            var responseToReturn = responses.Select(p => _mapper.Map<ResponseToReturnDto>(p));

            //this.Response.AddHeader("Pagination", new PaginationHeader
            //(
            //    responses.PageNumber,
            //    responses.PageSize,
            //    responses.TotalPages,
            //    responses.TotalCount
            //));

            return Ok(responseToReturn);
        }

		/// <summary>
		/// Get a response by id. 
		/// </summary>
		/// 
		/// <param name="discussionId">The discussion id.</param>
		/// <param name="responseId">The response id.</param>
		[HttpGet("/api/discussions/{discussionId:long}/responses/{responseId:long}")]
		public async Task<IActionResult> Get(long discussionId, long responseId)
        {
            var membership = CreateMembership();

			var response = await _repo.GetAsync(responseId, membership);

            var responseToReturn = _mapper.Map<ResponseToReturnDto>(response);

            return Ok(responseToReturn);
        }

		/// <summary>
		/// Update a response in repository. 
		/// </summary>
		/// 
		/// <param name="responseId">The response id.</param>
		/// <param name="discussionId">The discussion id.</param>
		/// <param name="updateResponseDto">The response information.</param>
		[HttpPut("/api/discussions/{discussionId:long}/responses/{responseId:long}")]
		public async Task<IActionResult>  Update(long discussionId, long responseId, UpdateResponseDto updateResponseDto)
        {
            var membership = CreateMembership();

			var updateResponse = _mapper.Map<Response>(updateResponseDto);

            updateResponse.Id = responseId;

            var responseUpdated = await _repo.UpdateAsync(updateResponse, membership);

            return Created(new Uri($"{Request.GetDisplayUrl()}/{responseUpdated.Id}"), responseUpdated);
        }

        /// <summary>
        /// Delete a response in repository. 
        /// </summary>
        /// 
        /// <param name="discussionId">The discussion id.</param>
        /// <param name="responseId">The response id.</param>
        [HttpDelete("/api/discussions/{discussionId:long}/responses/{responseId:long}")]
        public async Task<IActionResult> Delete(long discussionId, long responseId)
        {
            var membership = CreateMembership();

			await _repo.DeleteAsync(responseId, membership);

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
				GroupIds = new long[0],
				OrganizationsIds = new long[0]
			};
		}
		#endregion
	}
}