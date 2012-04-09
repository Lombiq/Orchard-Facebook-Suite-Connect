using Orchard.ContentManagement;

namespace Piedone.Facebook.Suite.Models
{
    /// <summary>
    /// A uniform interface for Facebook profile data
    /// </summary>
    public interface IFacebookUser : IContent
    {
        long FacebookUserId { get; }
        string Name { get; }
        string FirstName { get; }
        string LastName { get; }
        string Link { get; }
        string FacebookUserName { get; }
        string Gender { get; }
        int TimeZone { get; }
        string Locale { get; }
        bool IsVerified { get; }
    }

    public static class FacebookUserExtensions
    {
        public static string GetPictureLink(this IFacebookUser user)
        {
            return "http://graph.facebook.com/" + user.FacebookUserId + "/picture";
        }
    }
}