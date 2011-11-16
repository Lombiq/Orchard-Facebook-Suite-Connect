using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Piedone.Facebook.Suite.Models
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookUserPartRecord : ContentPartRecord
    {
        //public virtual int UserId { get; set; }
        public virtual long FacebookUserId { get; set; }
        public virtual string Name { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Link { get; set; }
        public virtual string FacebookUserName { get; set; }
        public virtual string Gender { get; set; }
        public virtual int TimeZone { get; set; }
        public virtual string Locale { get; set; }
        public virtual bool IsVerified { get; set; }
    }

    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookUserPart : ContentPart<FacebookUserPartRecord>
    {
        /// <summary>
        /// The id of the corresponding User part.
        /// </summary>
        //public int UserId
        //{
        //    get { return Record.UserId; }
        //    set { Record.UserId = value; }
        //}

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

        public string PictureLink
        {
            get { return "http://graph.facebook.com/" + FacebookUserId + "/picture"; }
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