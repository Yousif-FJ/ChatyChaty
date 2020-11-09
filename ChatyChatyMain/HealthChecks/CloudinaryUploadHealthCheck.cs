using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Net;
using ChatyChaty.Infrastructure.PictureServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using CloudinaryDotNet;

namespace ChatyChaty.HealthChecks
{
    public class CloudinaryUploadHealthCheck : IHealthCheck
    {
        private readonly CloudinaryPictureProvider pictureProvider;
        public CloudinaryUploadHealthCheck()
        {
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory().CreateLogger<CloudinaryPictureProvider>(); ;
            var pictureProvider = new CloudinaryPictureProvider(
                new Cloudinary(), logger);
            this.pictureProvider = pictureProvider;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            string UserName = "SomeVeryUniqueName";
            long UserID = 4363293744729;
            using var Fs = new FileStream(path: "PhotoUploadTestSamples/Untitled.png", FileMode.Open);
            var FF = new FormFile(Fs, 0, Fs.Length, "SomeFile", "SomeUnknowFileName");


            await pictureProvider.ChangePhoto(UserID: UserID, UserName: UserName, FF.FileName, FF.OpenReadStream());

            var PhotoUrl = await pictureProvider.GetPhotoURL(UserID, UserName);
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync(PhotoUrl);

            HealthCheckResult result;
            if (HttpStatusCode.OK == response.StatusCode)
            {
                result = HealthCheckResult.Healthy("Profile Picture Upload is functional");
            }
            else
            {
                result = HealthCheckResult.Unhealthy("Profile Picture Upload is failing");
            }

            //Clean up
            await pictureProvider.DeletePhoto(UserID, UserName);

            return result;
        }
    }
}
