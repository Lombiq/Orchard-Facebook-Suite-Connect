using System.IO;
using System.Net;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Logging;
using Piedone.Avatars.Services;
using Piedone.Facebook.Suite.Models;

namespace Piedone.Facebook.Suite.EventHandlers
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class AvatarsEventHandler : IFacebookConnectEventHandler
    {
        private readonly IAvatarsService _avatarsService;

        public ILogger Logger { get; set; }

        public AvatarsEventHandler(IAvatarsService avatarsService)
        {
            _avatarsService = avatarsService;

            Logger = NullLogger.Instance;
        }

        public void UserUpdated(IFacebookUser facebookUser)
        {
            var part = facebookUser.As<FacebookUserPart>();

            using (var wc = new WebClient())
            {
                try
                {
                    var stream = new MemoryStream(wc.DownloadData(part.GetPictureLink()));
                    _avatarsService.SaveAvatarFile(part.Id, stream, "jpg"); // We could look at the bytes to detect the file type, but rather not
                }
                catch (WebException ex)
                {
                    Logger.Error(ex, "Downloading of Facebok profile picture failed: " + ex.Message);
                }

                // Async versions throw exception regarding the transaction

                //wc.DownloadDataCompleted += _taskFactory.BuildAsyncEventHandler<object, DownloadDataCompletedEventArgs>(
                //    (sender, e) =>
                //    {
                //        if (e.Error == null)
                //        {
                //            var stream = new MemoryStream(e.Result);
                //            _avatarsService.SaveAvatarFile(facebookUserPart.Id, stream, "jpg"); // We could look at the bytes to detect the file type, but rather not
                //        }

                //        else
                //        {
                //            string message = "Downloading of Facebok profile picture failed: " + e.Error.Message;
                //            Logger.Error(e.Error, message);
                //        }
                //    }, false).Invoke;

                //wc.DownloadDataCompleted +=
                //    (sender, e) =>
                //    {
                //        if (e.Error == null)
                //        {
                //            var stream = new MemoryStream(e.Result);
                //            _avatarsService.SaveAvatarFile(facebookUserPart.Id, stream, "jpg"); // We could look at the bytes to detect the file type, but rather not
                //        }

                //        else
                //        {
                //            string message = "Downloading of Facebok profile picture failed: " + e.Error.Message;
                //            Logger.Error(e.Error, message);
                //        }
                //    };


                //wc.DownloadDataAsync(new Uri(part.PictureLink));
            }
        }
    }
}