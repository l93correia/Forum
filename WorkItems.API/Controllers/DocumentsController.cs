using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emsa.Mared.Common.Security;
using Emsa.Mared.Discussions.API.Controllers;
using Emsa.Mared.WorkItems.API.Contracts.WorkItemDocuments;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemAttachments;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemComments;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemRelations;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Emsa.Mared.WorkItems.API.Controllers
{
    /// <summary>
    /// The documents api controller allows to create, get, update and delete documents.
    /// </summary>
    /// 
    /// <seealso cref="BaseWorkItemsController" />
    [ApiController]
    public class DocumentsController : BaseWorkItemsController
    {
        #region [Attributes]
        /// <summary>
        /// The work items repository.
        /// </summary>
        private readonly IWorkItemRepository WorkItemRepository;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentsController"/> class.
        /// </summary>
        /// 
        /// <param name="mapper">The mapper.</param>
        /// <param name="workItemRepository">The repository.</param>
        /// <param name="attachmentsRepository">The repository.</param>
        /// <param name="commentRepository">The repository.</param>
        /// <param name="participantRepository">The repository.</param>
        /// <param name="relationRepository">The repository.</param>
        public DocumentsController
        (
            IWorkItemRepository workItemRepository,
            IWorkItemAttachmentRepository attachmentsRepository,
            IWorkItemCommentRepository commentRepository,
            IWorkItemParticipantRepository participantRepository,
            IWorkItemRelationRepository relationRepository,
            IMapper mapper
        )
        : base(attachmentsRepository, commentRepository, participantRepository, relationRepository, mapper)
        {
            this.WorkItemRepository = workItemRepository;
        }
        #endregion

        #region [Methods] Documents
        /// <summary>
        /// Create a document work item in the repository.
        /// </summary>
        /// 
        /// <param name="createDocument">The document to create.</param>
        [HttpPost]
        public virtual async Task<IActionResult> Create(DocumentToCreate createDocument)
        {
            var membership = this.CreateMembership();
            var document = this.Mapper.Map<WorkItem>(createDocument);
            document.UserId = membership.UserId;
            document.Type = WorkItemType.Document;

            document = await this.WorkItemRepository.CreateAsync(document, membership);

            return Created(new Uri
            (
                $"{this.Request.GetDisplayUrl()}/{document.Id}"),
                this.Mapper.Map<DocumentToReturn>(document)
            );
        }

        /// <summary>
        /// Update a document work item in the repository.
        /// </summary>
        /// 
        /// <param name="id">The document id.</param>
        /// <param name="updateDocument">The document to update.</param>
        [HttpPut("{id:long}")]
        public virtual async Task<IActionResult> Update(long id, DocumentToUpdate updateDocument)
        {
            var membership = this.CreateMembership();
            var document = this.Mapper.Map<WorkItem>(updateDocument);
            document.Id = id;

            if (await this.WorkItemRepository.IsType(id, WorkItemType.Document))
            {
                await this.WorkItemRepository.UpdateAsync(document, membership);

                return this.Ok();
            }
            return NotFound();
        }

        /// <summary>
        /// Deletes a document work item from the repository.
        /// </summary>
        /// 
        /// <param name="id">The document id.</param>
        [HttpDelete("{id:long}")]
        public virtual async Task<IActionResult> Delete(long id)
        {
            var membership = this.CreateMembership();

            if (await this.WorkItemRepository.IsType(id, WorkItemType.Document))
            {
                await this.WorkItemRepository.DeleteAsync(id, membership);

                return this.Ok();
            }
            return NotFound();
        }

        /// <summary>
        /// Get a document work item by id from the repository.
        /// </summary>
        /// 
        /// <param name="id">The document id.</param>
        [HttpGet("{id:long}")]
        public virtual async Task<IActionResult> Get(long id)
        {
            var membership = this.CreateMembership();

            if (await this.WorkItemRepository.IsType(id, WorkItemType.Document))
            {
                var document = await this.WorkItemRepository.GetAsync(id, membership);
                var returnDocument = this.Mapper.Map<DocumentToReturn>(document);

                return this.Ok(returnDocument);
            }
            return NotFound();
        }

        /// <summary>
        /// Get all the documents work items from the repository.
        /// </summary>
        /// 
        /// <param name="parameters">The parameters.</param>
        [HttpGet]
        public virtual async Task<IActionResult> GetAll([FromQuery] WorkItemParameters parameters = null)
        {
            var membership = this.CreateMembership();
            parameters.WorkItemType = WorkItemType.Document;

            var documents = await this.WorkItemRepository.GetAllAsync(parameters, membership);
            var returnDocuments = this.Mapper.Map<IEnumerable<DocumentToReturn>>(documents);

            return this.Ok(returnDocuments);
        }
        #endregion
    }
}