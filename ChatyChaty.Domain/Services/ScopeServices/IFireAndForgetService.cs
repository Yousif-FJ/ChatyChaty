﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Services.ScopeServices
{
    /// <summary>
    /// Perform a task safely without waiting for it
    /// </summary>
    public interface IFireAndForgetService
    {
        public void RunActionWithoutWaiting<T>(Action<T> bullet, Action<Exception> handler = null);
        public void RunActionWithoutWaitingAsync<T>(Func<T, Task> bullet, Action<Exception> handler = null);
    }
}
