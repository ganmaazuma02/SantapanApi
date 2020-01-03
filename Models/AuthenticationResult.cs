﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Models
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public string Token { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
}