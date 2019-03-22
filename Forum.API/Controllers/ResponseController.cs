using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Forum.API.Data;
using Forum.API.Dtos;
using Forum.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Forum.API.Controllers
{
    [Route("api/discussion/{discussionId:long}/[controller]")]
    [ApiController]
    public class ResponseController : ControllerBase
    {
        private readonly IResponseRepository _repo;
        private readonly IMapper _mapper;

        public ResponseController(IResponseRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // POST api/discussion/id/response
        [HttpPost]
        public async Task<IActionResult> Create(long discussionId, ResponseToCreateDto responseToCreateDto)
        {
            var response = _mapper.Map<DiscussionResponses>(responseToCreateDto);

            response.DiscussionId = discussionId;

            var responseCreated = await _repo.Create(response);

            return this.Created(new Uri($"{this.Request.GetDisplayUrl()}/{response.Id}"), responseCreated);
        }

        // GET api/discussion/id/response
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var responses = await _repo.GetAll();

            var responsesToReturn = _mapper.Map<IEnumerable<ResponseToReturnDto>>(responses);

            return Ok(responsesToReturn);
        }

        // GET api/discussion/id/response
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var response = await _repo.Get(id);

            var responseToReturn = _mapper.Map<ResponseToReturnDto>(response);

            return Ok(responseToReturn);
        }


        // PUT api/discussion/id/response
        [HttpPut("{id}")]
        public async Task<IActionResult>  Update(long id, UpdateResponseDto updateResponseDto)
        {
            var updateResponse = _mapper.Map<DiscussionResponses>(updateResponseDto);

            updateResponse.Id = id;

            var responseUpdated = await _repo.Update(updateResponse);

            return this.Created(new Uri($"{this.Request.GetDisplayUrl()}/{responseUpdated.Id}"), responseUpdated);
        }

        // DELETE api/discussion/id/response
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _repo.Delete(id);

            return this.Ok();
        }
    }
}