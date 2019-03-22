using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Dtos
{
    public class ResponseToReturnDto
    {
        public long Id { get; set; }
        public long DiscussionId { get; set; }
        public string Username { get; set; }
        public string Response { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
