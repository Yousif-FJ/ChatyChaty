using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using ChatyChaty.Infrastructure.PictureServices;
using ChatyChaty.Domain.Model.Entity;

namespace ChatyChaty.HealthChecks
{
    public class CloudinaryUploadHealthCheck : IHealthCheck
    {
        private readonly CloudinaryPictureProvider pictureProvider;

        public CloudinaryUploadHealthCheck(IConfiguration configuration)
        {
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory().CreateLogger<CloudinaryPictureProvider>(); ;
            var pictureProvider = new CloudinaryPictureProvider(
                new Cloudinary(configuration["CLOUDINARY_URL"]), logger);
            this.pictureProvider = pictureProvider;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            UserId userID = new(Guid.NewGuid().ToString());
            using var Fs = new FileStream(path: "HealthChecks/PhotoUploadTestSamples/Untitled.png", FileMode.Open);
            var FF = new FormFile(Fs, 0, Fs.Length, "SomeFile", "SomeUnknowFileName");


            var photoUrl = await pictureProvider.ChangePhoto(userID: userID, FF.FileName, FF.OpenReadStream());

            HttpClient httpClient = new();
            var response = await httpClient.GetAsync(photoUrl, cancellationToken);

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
            await pictureProvider.DeletePhoto(userID);

            return result;
        }
    }
}
