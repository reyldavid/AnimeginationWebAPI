using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimeginationApi.Models
{
    public class ClaimModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }
    }
}