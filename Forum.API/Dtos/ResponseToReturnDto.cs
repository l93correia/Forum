using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Dtos
{
    public class ResponseToReturnDto
    {
        public int Id { get; set; }
        public int DiscussionId { get; set; }
        public string Username { get; set; }
        public string Response { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
