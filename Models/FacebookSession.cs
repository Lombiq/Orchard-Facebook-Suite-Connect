using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Piedone.Facebook.Suite.Models
{
    /// <summary>
    /// Stores the Facebook session associated with a user.
    /// </summary>
    /// <remarks>
    /// The cookie created by the Facebook JS SDK could be directly parsed, but because its format is not guaranteed to be stable it's better to use an
    /// own storage.
    /// </remarks>
    public abstract class FacebookSession
    {
        public long UserId { get; protected set; }
        public string AccessToken { get; protected set; }
        public DateTime ExpiresUtc { get; protected set; }
    }
}