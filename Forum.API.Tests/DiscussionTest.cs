using AutoMapper;
using Forum.API.Controllers;
using Forum.API.Data;
using Forum.API.Dtos;
using Forum.API.Helpers;
using Forum.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forum.API.Tests
{
    [TestClass]
    public class DiscussionTest
    {
        #region [Constants]
        /// <summary>
        /// The subject to test.
        /// </summary>
        private readonly string _subject = "Test {0}";

        /// <summary>
        /// The comment to test.
        /// </summary>
        private readonly string _comment = "Test comment {0}";

        /// <summary>
        /// The status to test.
        /// </summary>
        private readonly string _status = "Created";
        #endregion

        #region [Attributes]
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly Mock<IDiscussionRepository> _mockRepo;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscussionTest"/> class.
        /// </summary>
        /// 
        public DiscussionTest()
        {
            _mockRepo = new Mock<IDiscussionRepository>();

            //mock to create discussion
            


        }
        #endregion

        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            Mapper.Initialize(m => m.AddProfile<AutoMapperProfile>());
        }

        public Discussion CreateDiscussion(long? index = 0)
        {
            return new Discussion
            {
                Id = index.Value,
                Subject = string.Format(_subject, index),
                Comment = string.Format(_comment, index),
                CreatedDate = DateTime.Now,
                Status = _status,
                UserId = 1
            };

        }

        public void MockGetById()
        {
            //mock to get by id
            var getDiscussion = CreateDiscussion();
            _mockRepo.Setup(p => p.Get(1)).Returns(Task.FromResult(getDiscussion));
        }

        public void MockGetAll()
        {
            //mock to get all
            List<Discussion> getDiscussions = new List<Discussion>();
            for (int i = 0; i < 3; i++)
            {
                getDiscussions.Add(CreateDiscussion(i));
            }
            _mockRepo.Setup(p => p.GetAll()).Returns(Task.FromResult(getDiscussions));
        }

        public void MockHttpRequest(ControllerBase controller)
        {
            //var fakeHttpContext = new Mock<HttpContext>();
            var controllerContext = new Mock<ControllerContext>();
            var httpContext = new Mock<HttpContext>();
            var httpRequest = new Mock<HttpRequest>();
            //controllerContext.Setup(t => t.HttpContext).Returns(fakeHttpContext.Object);
            //controller.ControllerContext = controllerContext.Object;

            controller.ControllerContext = controllerContext.Object;
            controller.ControllerContext.HttpContext = httpContext.Object;

            httpContext.SetupGet(x => x.Request).Returns(httpRequest.Object);
           
            httpRequest.SetupGet(x => x.Scheme).Returns("https");
            httpRequest.SetupGet(x => x.PathBase).Returns("");
            //httpRequest.Object.Host.Value 

            var hostString = new HostString("https://localhost/api/discussions");

            httpRequest.Object.Host = hostString;

            var path = new PathString("/api/discussions");

            httpRequest.Object.Path = path;

            var query = new QueryString("");

            httpRequest.Object.QueryString = query;

            //httpRequest.SetupGet(x => x.Host.Value).Returns("localhost");
            //httpRequest.SetupGet(x => x.Path.Value).Returns("/api/discussions");
            //httpRequest.SetupGet(x => x.QueryString.Value).Returns("");
        }

        public DiscussionsController MockCreate()
        {
            _mockRepo.Setup(p => p.Create(It.IsAny<Discussion>()))
                .Returns((Discussion discussion) =>
                {
                    discussion.Id = 1;

                    return Task.FromResult(discussion);
                });

            var discussionController = new DiscussionsController(_mockRepo.Object, Mapper.Instance);

            MockHttpRequest(discussionController);

            return discussionController;
        }

        [TestMethod]
        public void TestGetById()
        {
            MockGetById();

            DiscussionsController discussionController = new DiscussionsController(_mockRepo.Object, Mapper.Instance);
            OkObjectResult result = discussionController.Get(1).Result as OkObjectResult;
            DiscussionToReturnDto discussion = result.Value as DiscussionToReturnDto;
            Assert.AreEqual(string.Format(_subject, 0), discussion.Subject);
            Assert.AreEqual(string.Format(_comment, 0), discussion.Comment);
            Assert.AreEqual(_status, discussion.Status);
            Assert.AreEqual(0, discussion.DiscussionResponses.Count);
        }

        [TestMethod]
        public void TestGetAll()
        {
            MockGetAll();

            DiscussionsController discussionController = new DiscussionsController(_mockRepo.Object, Mapper.Instance);
            OkObjectResult result = discussionController.GetAll().Result as OkObjectResult;
            List<DiscussionForListDto> discussions = result.Value as List<DiscussionForListDto>;
            Assert.AreEqual(3, discussions.Count);

            var i = 0;
            foreach(DiscussionForListDto discussion in discussions)
            {
                Assert.AreEqual(string.Format(_subject, i), discussion.Subject);
                Assert.AreEqual(string.Format(_comment, i), discussion.Comment);
                Assert.AreEqual(_status, discussion.Status);
                Assert.AreEqual(0, discussion.ResponsesCount);
                i++;
            }

        }

        [TestMethod]
        public void TestCreateDiscussion()
        {
            DiscussionsController discussionController = MockCreate();

            var createDiscussion = new DiscussionToCreateDto
            {
                Subject = string.Format(_subject, 0),
                Comment = string.Format(_comment, 0),
                UserId = 1
            };

            object result2 = discussionController.Create(createDiscussion);
            CreatedResult result = discussionController.Create(createDiscussion).Result as CreatedResult;
            DiscussionToReturnDto discussion = result.Value as DiscussionToReturnDto;
            //Assert.AreEqual(discussionController.Request.GetDisplayUrl(), result.Location);
            Assert.AreEqual(string.Format(_subject, 0), discussion.Subject);
            Assert.AreEqual(string.Format(_comment, 0), discussion.Comment);
            Assert.AreEqual(_status, discussion.Status);
            Assert.AreEqual(0, discussion.DiscussionResponses.Count);
        }
    }
}
