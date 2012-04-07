using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Piedone.Facebook.Suite.Models
{
    public abstract class FacebookSession
    {
        public long UserId { get; set; }
        public string AccessToken { get; set; }
    }
}