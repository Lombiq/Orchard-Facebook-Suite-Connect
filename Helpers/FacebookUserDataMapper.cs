using Piedone.Facebook.Suite.Models;

namespace Piedone.Facebook.Suite.Helpers
{
    /// <summary>
    /// Provides a non-changing interface for user data Facebook API results and maps them to FacebookUserPart's properties
    /// </summary>
    public class FacebookUserDataMapper
    {
        private dynamic apiResult;
        public dynamic ApiResult
        {
            get { return apiResult; }
            set
            {
                apiResult = value;

                FacebookUserId = long.Parse(apiResult.id);
                Name = apiResult.name;
                FirstName = apiResult.first_name;
                LastName = apiResult.last_name;
                Email = (apiResult.email != null) ? apiResult.email : "";
                Link = apiResult.link;
                FacebookUserName = apiResult.username;
                Gender = apiResult.gender;
                TimeZone = (int)apiResult.timezone;
                Locale = ((string)apiResult.locale).Replace('_', '-'); // Making locale Orchard-compatible
                IsVerified = (apiResult.verified != null) ? apiResult.verified : false; // Maybe it is possible that verified is set, but is false -> don't take automatically as true if it's set
            }
        }

        public long FacebookUserId { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Link { get; set; }
        public string FacebookUserName { get; set; }
        public string Gender { get; set; }
        public int TimeZone { get; set; }
        public string Locale { get; set; }
        public bool IsVerified { get; set; }
        
        public FacebookUserDataMapper(dynamic apiResult)
        {
            ApiResult = apiResult;
        }

        public static FacebookUserPart MapToFacebookUserPart(FacebookUserDataMapper dataMapper, FacebookUserPart userPart)
        {
            userPart.FacebookUserId = dataMapper.FacebookUserId;
            userPart.Name = dataMapper.Name;
            userPart.FirstName = dataMapper.FirstName;
            userPart.LastName = dataMapper.LastName;
            userPart.Link = dataMapper.Link;
            userPart.FacebookUserName = dataMapper.FacebookUserName;
            userPart.Gender = dataMapper.Gender;
            userPart.TimeZone = dataMapper.TimeZone;
            userPart.Locale = dataMapper.Locale;
            userPart.IsVerified = dataMapper.IsVerified;

            return userPart;
        }
    }
}