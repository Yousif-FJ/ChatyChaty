using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services.GoogleFirebase
{
    public class FirstTest
    {
        private readonly ILogger<FirstTest> logger;

        public FirstTest(ILogger<FirstTest> logger)
        {
            this.logger = logger;
            try
			{
                var App = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.GetApplicationDefault(),
                });
                logger.LogWarning($"Firebase was created succefully with the name:{App.Name}");
            }
			catch (Exception e)
			{
                logger.LogWarning($"GoogleCredential threw an exception:{e.Message}");
			}
        }
    }
}
