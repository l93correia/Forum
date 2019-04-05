using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emsa.Mared.Common.Database;
using Emsa.Mared.Common.Controllers;
using Forum.API.Data;
using Forum.API.Dtos;
using Forum.API.Models;
using Forum.API.Models.Repository.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Forum.API.Controllers
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

        #region [Methods] Utility
        /// <summary>
        /// Create a discussion response in repository.
        /// </summary>
        /// 
        /// <param name="discussionId">The discussion id.</param>
        /// <param name="responseToCreateDto">The response information.</param>
        [HttpPost]
        public async Task<IActionResult> Create(long discussionId, ResponseToCreateDto responseToCreateDto)
        {
            var response = _mapper.Map<DiscussionResponse>(responseToCreateDto);

            response.DiscussionId = discussionId;

            var responseCreated = await _repo.Create(response);

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
            var responses = await _repo.GetByDiscussion(discussionId);

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
        /// Get all responses. 
        /// </summary>
        /// 
		/// <param name="parameters">The parameters.</param>
        [HttpGet("/api/discussions/responses")]
        public async Task<IActionResult> GetAll([FromQuery] ResponseParameters parameters = null)
        {
            var responses = await _repo.GetAll();

            var responsesToReturn = _mapper.Map<IEnumerable<ResponseToReturnDto>>(responses);

            return Ok(responsesToReturn);
        }

        /// <summary>
        /// Get a response by id. 
        /// </summary>
        /// 
        /// <param name="id">The response id.</param>
        [HttpGet("{id:long}")]
        public async Task<IActionResult> Get(long id)
        {
            var response = await _repo.Get(id);

            var responseToReturn = _mapper.Map<ResponseToReturnDto>(response);

            return Ok(responseToReturn);
        }


        /// <summary>
        /// Update a response in repository. 
        /// </summary>
        /// 
        /// <param name="id">The response id.</param>
		/// <param name="updateResponseDto">The response information.</param>
        [HttpPut("{id:long}")]
        public async Task<IActionResult>  Update(long id, UpdateResponseDto updateResponseDto)
        {
            var updateResponse = _mapper.Map<DiscussionResponse>(updateResponseDto);

            updateResponse.Id = id;

            var responseUpdated = await _repo.Update(updateResponse);

            return this.Created(new Uri($"{this.Request.GetDisplayUrl()}/{responseUpdated.Id}"), responseUpdated);
        }

        /// <summary>
        /// Delete a response in repository. 
        /// </summary>
        /// 
        /// <param name="discussionId">The discussion id.</param>
        /// <param name="responseId">The response id.</param>
        [HttpDelete("{responseId:long}")]
        public async Task<IActionResult> Delete(long discussionId, long responseId)
        {
            await _repo.Delete(responseId);

            //TODO hide a discussion, do not remove from db

            return this.Ok();
        }
        #endregion
    }
}