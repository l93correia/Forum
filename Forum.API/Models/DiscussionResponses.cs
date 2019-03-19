using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Models
{
    public class DiscussionResponses
    {
        public int Id { get; set; }
        public int DiscussionId { get; set; }
        public Discussions Discussion { get; set; }
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public string Response { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
