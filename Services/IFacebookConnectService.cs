using Orchard;
using Piedone.Facebook.Suite.Models;
using Piedone.HelpfulLibraries.ServiceValidation.ServiceInterfaces;
using Orchard.Security;
using System;
using System.Linq;
using Orchard.ContentManagement;

namespace Piedone.Facebook.Suite.Services
{
    /// <summary>
    /// Use these values to check the ValidationDictionary against errors
    /// </summary>
    public enum FacebookConnectValidationKey
    {
        NotAuthenticated,
        NoPermissionsGranted,
        NotVerified
    }

    /// <summary>
    /// Facebook Suite Connect services
    /// 
    /// Any interaction with the Facebook Connect feature's content parts/types/records should happen here.
    /// Inherits from IDependency as Facebook authentication should be validated on a per-request basis.
    /// </summary>
    public interface IFacebookConnectService : IDependency
    {
        /// <summary>
        /// Facebook UserId of the currently authenticated Facebok user
        /// </summary>
        long AuthenticatedFacebookUserId { get; }

        /// <summary>
        /// Checks if the user is connected to our Facebook app and is authenticated on Facebook
        /// </summary>
        bool IsAuthenticated();

        /// <summary>
        /// Checks if the user is authenticated and granted permissions
        /// </summary>
        /// <param name="permissions">An array of Facebook permissions to authorize against</param>
        bool IsAuthorized(string[] permissions = null);

        /// <summary>
        /// Fetches the currently authenticated user's profile data from Facebook
        /// </summary>
        IFacebookUser FetchMe();

        /// <summary>
        /// Updates a local user with Facebook user profile data
        /// </summary>
        /// <param name="user">The local user</param>
        /// <param name="facebookUser">The Facebook user profile data</param>
        void UpdateFacebookUser(IUser user, IFacebookUser facebookUser);

        /// <summary>
        /// Gets data of a saved Facebook user profile
        /// </summary>
        /// <param name="facebookId">The numerical ID of the Facebook profile</param>
        IFacebookUser GetFacebookUser(long facebookId);
    }

    public static class FacebookConnectServiceExtensions
    {
        /// <summary>
        /// Converts a comma-delimited string of Facebook permissions to an array
        /// </summary>
        /// <param name="permissions">The permissions string</param>
        public static bool IsAuthorized(this IFacebookConnectService service, string permissions)
        {
            string[] permissionArray = new string[0];
            if (permissions != null)
            {
                permissionArray = permissions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                permissionArray = (from p in permissionArray select p.Trim()).ToArray();
            }

            return service.IsAuthorized(permissionArray);
        }

        /// <summary>
        /// Checks if the currently authenticated Facebook user's profile data is saved to a local user profile
        /// </summary>
        public static bool AuthenticatedFacebookUserIsSaved(this IFacebookConnectService service)
        {
            return service.GetFacebookUser(service.AuthenticatedFacebookUserId) != null;
        }

        /// <summary>
        /// Gets the current authenticated user's Facebook user profile data
        /// </summary>
        public static IFacebookUser GetAuthenticatedFacebookUser(this IFacebookConnectService service)
        {
            return service.GetFacebookUser(service.AuthenticatedFacebookUserId);
        }

        /// <summary>
        /// Updates the currently authenticated Facebook user's local profile with fresh profile data from Facebook
        /// </summary>
        /// <returns>The updated </returns>
        public static IFacebookUser UpdateAuthenticatedFacebookUser(this IFacebookConnectService service)
        {
            var facebookUser = GetAuthenticatedFacebookUser(service);

            if (facebookUser == null) return null;
            service.UpdateFacebookUser(facebookUser.As<IUser>(), service.FetchMe());

            return facebookUser;
        }
    }
}
