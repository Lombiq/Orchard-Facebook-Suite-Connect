using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.ContentManagement;
using Orchard.Security;

namespace Piedone.Facebook.Suite.Models
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookUserPart : ContentPart<FacebookUserPartRecord>, IFacebookUser
    {
        public long FacebookUserId
        {
            get { return Record.FacebookUserId; }
            set { Record.FacebookUserId = value; }
        }

        public string Name
        {
            get { return Record.Name; }
            set { Record.Name = value; }
        }

        public string FirstName
        {
            get { return Record.FirstName; }
            set { Record.FirstName = value; }
        }

        public string LastName
        {
            get { return Record.LastName; }
            set { Record.LastName = value; }
        }

        public string Link
        {
            get { return Record.Link; }
            set { Record.Link = value; }
        }

        /// <summary>
        /// The user name on Facebook
        /// </summary>
        public string FacebookUserName
        {
            get { return Record.FacebookUserName; }
            set { Record.FacebookUserName = value; }
        }

        public string Gender
        {
            get { return Record.Gender; }
            set { Record.Gender = value; }
        }

        public int TimeZone
        {
            get { return Record.TimeZone; }
            set { Record.TimeZone = value; }
        }

        public string Locale
        {
            get { return Record.Locale; }
            set { Record.Locale = value; }
        }

        public bool IsVerified
        {
            get { return Record.IsVerified; }
            set { Record.IsVerified = value; }
        }
    }
}