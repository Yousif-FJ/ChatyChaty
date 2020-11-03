using ChatyChaty.Infrastructure.PictureServices;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

namespace XUnitTest
{
    //to do - this should be a health check not a unit test
    public class CouldinaryPictureProviderTest
    {
        //This assume we have a CLOUDINARY_URL Environment variable
        private readonly CloudinaryPictureProvider pictureProvider;
        public CouldinaryPictureProviderTest()
        {
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory().CreateLogger<CloudinaryPictureProvider>(); ;
            var pictureProvider = new CloudinaryPictureProvider(
                new Cloudinary(), logger);
            this.pictureProvider = pictureProvider;
        }

        [Fact]
        public async void ChangePhoto_New_Upload()
        {
            //Arrange
            string UserName = "SomeVeryUniqueName";
            long UserID = 4363293744729;
            using var Fs = new FileStream(path: "TestSamples/Untitled.png", FileMode.Open);
            var FF = new FormFile(Fs, 0, Fs.Length, "SomeFile", "SomeUnknowFileName");

            //Act
            await pictureProvider.ChangePhoto(UserID:UserID , UserName:UserName,FF.FileName, FF.OpenReadStream());

            //Assert
            var PhotoUrl = pictureProvider.GetPhotoURL(UserID, UserName);
            HttpClient httpClient = new HttpClient();
            var result = await httpClient.GetAsync(PhotoUrl);
            Assert.True(HttpStatusCode.OK == result.StatusCode);

            //Clean up
            await pictureProvider.DeletePhoto(UserID, UserName);
        }
    }
}
