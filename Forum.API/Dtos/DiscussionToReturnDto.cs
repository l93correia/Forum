﻿using Forum.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Dtos
{
    public class DiscussionToReturnDto
    {
        public long Id { get; set; }
        public string Subject { get; set; }
        public string Username { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Comment { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public List<ResponseToReturnDto> DiscussionResponses { get; set; }
    }
}
