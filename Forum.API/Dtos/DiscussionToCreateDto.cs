using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Dtos
{
    public class DiscussionToCreateDto
    {
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Comment { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}
