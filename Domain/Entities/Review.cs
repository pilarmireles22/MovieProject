﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Review
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }
        public string Description { get; set; } 
    }
}
