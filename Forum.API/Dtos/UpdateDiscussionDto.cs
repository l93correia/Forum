using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Dtos
{
    public class UpdateDiscussionDto
    {
        [Required]
        public string Comment { get; set; }
    }
}
