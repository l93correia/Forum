using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Models
{
    public class DiscussionResponses
    {
        public const string DoesNotExist = "The Response does not exist.";

        public long Id { get; set; }
        public long DiscussionId { get; set; }
        public Discussions Discussion { get; set; }
        public long CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public string Response { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
