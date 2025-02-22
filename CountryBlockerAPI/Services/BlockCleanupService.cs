using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CountryBlockerAPI.Services
{
    public class BlockCleanupService : BackgroundService
    {
        private readonly BlockService _blockService;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5); // Run every 5 minutes

        public BlockCleanupService(BlockService blockService)
        {
            _blockService = blockService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("🚀 Running Block Cleanup Service..."); //  Ensure the service is running

                _blockService.RemoveExpiredBlocks(); //  Remove countries with expired blocks

                await Task.Delay(_interval, stoppingToken); //  Wait 5 minutes before repeating
            }
        }
    }
}
