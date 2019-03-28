using AutoMapper;
using Forum.API.Controllers;
using Forum.API.Data;
using Forum.API.Dtos;
using Forum.API.Helpers;
using Forum.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Forum.API.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var mockRepo = new Mock<IDiscussionRepository>();
            var disc = new Discussion {
                Id = 1,
                Subject = "Test",
                Comment = "test comment",
                CreatedDate = DateTime.Now,
                Status = "Created",
                UserId = 1
            };
            mockRepo.Setup(p => p.Get(1)).Returns(Task.FromResult(disc));

            Mapper.Initialize(m => m.AddProfile<AutoMapperProfiles>());


            DiscussionsController discussionController = new DiscussionsController(mockRepo.Object, Mapper.Instance);
            var result1 = discussionController.Get(1).Result;
            OkObjectResult result = discussionController.Get(1).Result as OkObjectResult;
            DiscussionToReturnDto discussion = result.Value as DiscussionToReturnDto;
            Assert.AreEqual("Test", discussion.Subject);
        }
    }
}
