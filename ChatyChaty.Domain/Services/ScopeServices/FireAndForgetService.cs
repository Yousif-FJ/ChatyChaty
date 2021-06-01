using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Services.ScopeServices
{
    public class FireAndForgetService : IFireAndForgetService
    {
        private readonly ILogger<FireAndForgetService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public FireAndForgetService(
            ILogger<FireAndForgetService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public void RunActionWithoutWaiting<T>(Action<T> bullet, Action<Exception> handler = null)
        {
            _logger.LogInformation("Unawaited action started.");
            Task.Run(() =>
            {
                using var scope = _scopeFactory.CreateScope();
                var dependency = scope.ServiceProvider.GetRequiredService<T>();
                try
                {
                    bullet(dependency);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unawaited action crashed!");
                    handler?.Invoke(e);
                }
                finally
                {
                    dependency = default;
                }
            });
        }

        public void RunActionWithoutWaitingAsync<T>(Func<T, Task> bullet, Action<Exception> handler = null)
        {
            _logger.LogInformation("Async unawaited action started.");
            Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var dependency = scope.ServiceProvider.GetRequiredService<T>();
                try
                {
                    await bullet(dependency);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Async unawaited crashed!");
                    handler?.Invoke(e);
                }
                finally
                {
                    dependency = default;
                }
            });
        }
    }
}
