using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Models
{
    public class DiscussionParticipants
    {
        public long Id { get; set; }
        public long DiscussionId { get; set; }
        public Discussions Discussion { get; set; }
        public ICollection<OrganizationType> OrganizationType { get; set; }
    }
}
