using Orchard;
using Piedone.Facebook.Suite.Models;
using Piedone.ServiceValidation.Dictionaries;

namespace Piedone.Facebook.Suite.Services
{
    /// <summary>
    /// Describes the interface a Facebook Connect service should have
    /// 
    /// Any interaction with the Facebook Connect feature's content parts/types/records should happen here.
    /// Inherits from IDependency as Facebook authentication should be validated on a per-request basis.
    /// </summary>
    public interface IFacebookConnectService : IDependency
    {
        IServiceValidationDictionary ValidationDictionary { get; }

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
        /// Tries to authenticate the user with Facebook and authorize against permissions as well as authenticates
        /// the user with Orchard. Returns true on success.
        /// </summary>
        /// <param name="permissions">An array of Facebook permissions to authorize against.</param>
        /// <param name="onlyAllowVerified">If true, only verified Facebook users will be authenticated.</param>
        bool Authorize(string[] permissions = null, bool onlyAllowVerified = false);

        /// <summary>
        /// Gets data of a saved Facebook user profile
        /// </summary>
        /// <param name="facebookId">The numerical ID of the Facebook profile</param>
        FacebookUserPart GetFacebookUserPart(long facebookId);

        /// <summary>
        /// Gets data of a saved Facebook user profile
        /// </summary>
        /// <param name="id">The id of the part (the same as the corresponding user part)</param>
        FacebookUserPart GetFacebookUserPart(int id);

        /// <summary>
        /// Gets the current authenticated user's Facebook user profile data
        /// </summary>
        FacebookUserPart GetAuthenticatedFacebookUserPart();
    }
}
