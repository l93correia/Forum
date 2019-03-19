using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Models
{
    public class OrganizationType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int DiscussionParticipantsId { get; set; }
        public DiscussionParticipants DiscussionParticipants { get; set; }
    }
}
