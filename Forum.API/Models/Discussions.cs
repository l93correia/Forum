using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Models
{
    public class Discussions
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DocumentId { get; set; }
        public Document Document { get; set; }
        public string Comment { get; set; }
        public bool IsClosed { get; set; }

        public ICollection<DiscussionResponses> DiscussionResponses { get; set; }
    }
}
