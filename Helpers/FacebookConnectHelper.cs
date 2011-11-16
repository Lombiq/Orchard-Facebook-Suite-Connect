using System;
using System.Linq;

namespace Piedone.Facebook.Suite.Helpers
{
    public class FacebookConnectHelper
    {
        /// <summary>
        /// Converts a comma-delimited string of Facebook permissions to an array
        /// </summary>
        /// <param name="permissions">The permissions string</param>
        public static string[] PermissionSettingsToArray(string permissions)
        {
            string[] permissionArray = new string[0];
            if (permissions != null)
            {
                permissionArray = permissions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                permissionArray = (from p in permissionArray select p.Trim()).ToArray();
            }
            return permissionArray;
        }
    }
}