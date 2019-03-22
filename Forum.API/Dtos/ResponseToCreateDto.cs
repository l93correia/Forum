﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Dtos
{
    public class ResponseToCreateDto
    {
        [Required]
        public string Response { get; set; }
        [Required]
        public long CreatedById { get; set; }
    }
}
