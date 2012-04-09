using System;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Security;
using Piedone.Facebook.Suite.Models;
using Facebook;

namespace Piedone.Facebook.Suite.Services
{
    /// <summary>
    /// Use these values to check the ValidationDictionary against errors
    /// </summary>
    public enum FacebookConnectValidationKey
    {
        NotAuthenticated,
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
        /// The Facebook session associated with the current user.
        /// NULL, if there is no session (i.e. if the user is not authenticated with our app yet).
        /// </summary>
        FacebookSession Session { get; }

        /// <summary>
        /// Sets the Facebook session for the current user
        /// </summary>
        /// <param name="userId">Facebook user id of the current user</param>
        /// <param name="accessToken">The access token for the current user</param>
        /// <param name="expiresUtc">The UTC timestamp when the access token will expire</param>
        void SetSession(long userId, string accessToken, DateTime expiresUtc);

        /// <summary>
        /// Destroys the Facebook session of the current user
        /// </summary>
        void DestroySession();

        /// <summary>
        /// The FacebookClient object associated with the current session
        /// </summary>
        FacebookClient ClientForSession { get; }

        /// <summary>
        /// Fetches the currently authenticated user's profile data from Facebook
        /// </summary>
        IFacebookUser FetchMe();

        /// <summary>
        /// Checks if the user is valid to be connected, depending on the provided settings
        /// </summary>
        /// <param name="facebookUser">The Facebook user to check</param>
        /// <param name="settings">The settings to validate against</param>
        bool UserIsValid(IFacebookUser facebookUser, IFacebookConnectSettings settings, out IEnumerable<FacebookConnectValidationKey> errors);

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
        /// Checks if the user is connected to our Facebook app and is authenticated on Facebook
        /// </summary>
        public static bool IsAuthenticated(this IFacebookConnectService service)
        {
            return service.Session != null;
        }

        /// <summary>
        /// Checks if the currently authenticated Facebook user's profile data is saved to a local user profile
        /// </summary>
        public static bool AuthenticatedFacebookUserIsSaved(this IFacebookConnectService service)
        {
            if (!service.IsAuthenticated()) return false;

            return service.GetFacebookUser(service.Session.UserId) != null;
        }

        /// <summary>
        /// Gets the current authenticated user's Facebook user profile data
        /// </summary>
        public static IFacebookUser GetAuthenticatedFacebookUser(this IFacebookConnectService service)
        {
            if (!service.IsAuthenticated()) return null;

            return service.GetFacebookUser(service.Session.UserId);
        }

        /// <summary>
        /// Updates the currently authenticated Facebook user's local profile with fresh profile data from Facebook
        /// </summary>
        /// <param name="newData">Data to update the profile with. If not set, fresh data will be fetched from Facebook.</param>
        /// <returns>The updated user</returns>
        public static IFacebookUser UpdateAuthenticatedFacebookUser(this IFacebookConnectService service, IFacebookUser newData = null)
        {
            var facebookUser = GetAuthenticatedFacebookUser(service);

            if (facebookUser == null) return null;

            if (newData == null) newData = service.FetchMe();
            service.UpdateFacebookUser(facebookUser.As<IUser>(), newData);

            return facebookUser;
        }
    }
}
