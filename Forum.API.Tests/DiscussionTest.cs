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

            //mock to get all
            var disc = CreateDiscussion();
            _mockRepo.Setup(p => p.Get(1)).Returns(Task.FromResult(disc));

            //mock to get all
            List<Discussion> discussions = new List<Discussion>();
            for (int i = 0; i < 3; i++)
            {
                discussions.Add(CreateDiscussion(i));
            }
            _mockRepo.Setup(p => p.GetAll()).Returns(Task.FromResult(discussions));




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

        [TestMethod]
        public void TestGetById()
        {
            DiscussionsController discussionController = new DiscussionsController(_mockRepo.Object, Mapper.Instance);
            OkObjectResult result = discussionController.Get(1).Result as OkObjectResult;
            DiscussionToReturnDto discussion = result.Value as DiscussionToReturnDto;
            Assert.AreEqual(string.Format(_subject, 0), discussion.Subject);
            Assert.AreEqual(string.Format(_comment, 0), discussion.Comment);
            Assert.AreEqual(_status, discussion.Status);
        }

        [TestMethod]
        public void TestGetAll()
        {
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
                i++;
            }

        }
    }
}
