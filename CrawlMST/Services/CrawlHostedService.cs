using CrawlMST.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public class CrawlHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public CrawlHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var crawlService = scope.ServiceProvider.GetRequiredService<CrawlService>();
            await crawlService.CrawlAndSaveAllDataAsync("hai-phong-99");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
