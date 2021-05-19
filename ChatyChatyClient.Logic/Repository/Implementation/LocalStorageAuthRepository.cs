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

        public ValueTask<bool> IsAuthenticated()
        {
            return localStorage.ContainKeyAsync(tokenKey);
        }

        public ValueTask<string> GetToken()
        {
            return localStorage.GetItemAsStringAsync(tokenKey);
        }

        public ValueTask RemoveToken()
        {
            return localStorage.RemoveItemAsync(tokenKey);
        }

        public ValueTask SetToken(string value)
        {
            return localStorage.SetItemAsync(tokenKey, value);
        }
    }
}
