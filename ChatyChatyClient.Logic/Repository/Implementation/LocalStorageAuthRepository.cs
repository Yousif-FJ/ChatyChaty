using Blazored.LocalStorage;
using ChatyChatyClient.Logic.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.Repository.Implementation
{
    public class LocalStorageAuthRepository : IAuthenticationRepository
    {
        private static readonly string tokenKey = "token";
        private readonly ILocalStorageService localStorage;
        public LocalStorageAuthRepository(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }

        public async Task<string> GetToken()
        {
            return await localStorage.GetItemAsStringAsync(tokenKey);
        }

        public async Task RemoveToken()
        {
            await localStorage.RemoveItemAsync(tokenKey);
        }

        public async Task SetToken(string value)
        {
            await localStorage.SetItemAsync(tokenKey, value);
        }
    }
}
