using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Forum.API.Data;
using Forum.API.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Forum.API.Controllers
{
    [Route("api/discussion/{discussionId:int}/[controller]")]
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
        public async Task<IActionResult> PostResponse(ResponseToCreateDto responseToCreateDto)
        {
            var responseCreated = await _repo.AddResponse(responseToCreateDto.Response,
                    responseToCreateDto.UserId, responseToCreateDto.DiscussionId);

            return StatusCode(201);
        }
    }
}