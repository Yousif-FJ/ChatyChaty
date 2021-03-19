using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Repository
{
    public class BlazoredLocalStorageRepository : IAuthenticationRepository
    {
        private static readonly string tokenKey = "token";
        private readonly ILocalStorageService localStorage;
        public BlazoredLocalStorageRepository(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }

        public async Task<string> GetToken()
        {
            return await localStorage.GetItemAsStringAsync(tokenKey);
        }

        public async void RemoveToken()
        {
            await localStorage.RemoveItemAsync(tokenKey);
        }

        public async void SetToken(string value)
        {
            await localStorage.SetItemAsync(tokenKey, value);
        }
    }
}
