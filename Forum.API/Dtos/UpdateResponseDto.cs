using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Dtos
{
    public class UpdateResponseDto
    {
        [Required]
        public string Response { get; set; }
    }
}
