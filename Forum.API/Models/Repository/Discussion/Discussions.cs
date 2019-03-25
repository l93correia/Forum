using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Models
{
    public class Discussions
    {
        public const string DoesNotExist = "The Discussion does not exist.";

        public long Id { get; set; }
        public string Subject { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? DocumentId { get; set; }
        public Document Document { get; set; }
        public string Comment { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ICollection<DiscussionResponses> DiscussionResponses { get; set; }
    }
}
