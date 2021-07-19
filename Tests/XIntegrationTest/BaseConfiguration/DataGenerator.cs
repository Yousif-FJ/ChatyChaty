using ChatyChaty.HttpShemas.v1.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XIntegrationTest.BaseConfiguration
{
    public static class DataGenerator
    {
        private const string DisplayName = "name";
        private const string Password = "goodpass4321";

        public static IEnumerable<object[]> GetAccount()
        {
            yield return new object[] {
                CreateRandomAccount(),
            };
        }

        public static IEnumerable<object[]> GetSenderAndReceiver()
        {
            yield return new object[] {
                CreateRandomAccount(),
                CreateRandomAccount(),
            };
        }

        public static (CreateAccountSchema, CreateAccountSchema) Get2AccountTuple()
        {
            return (CreateRandomAccount(), CreateRandomAccount());
        }

        private static CreateAccountSchema CreateRandomAccount()
        {
            return new CreateAccountSchema(GenerateRandomString(), Password, DisplayName);
        }

        private static string GenerateRandomString()
        { 
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
