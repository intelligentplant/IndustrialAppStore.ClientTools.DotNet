using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using IntelligentPlant.IndustrialAppStore.Authentication;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ExampleMvcApplication.Services {
    public class BackgroundGroupFetcher : BackgroundService {

        private static readonly Channel<UserLogonEvent> s_channel = Channel.CreateUnbounded<UserLogonEvent>(new UnboundedChannelOptions() { 
            AllowSynchronousContinuations = false,
            SingleReader = true,
            SingleWriter = false
        });

        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger _logger;


        public BackgroundGroupFetcher(IServiceProvider serviceProvider, ILogger<BackgroundGroupFetcher> logger) { 
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public static async ValueTask OnUserLogonAsync(UserLogonEvent userLogonEvent, CancellationToken cancellationToken) {
            await s_channel.Writer.WriteAsync(userLogonEvent, cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            await foreach (var logonEvent in s_channel.Reader.ReadAllAsync(stoppingToken)) {
                using (var scope = _serviceProvider.CreateScope()) {
                    var client = scope.ServiceProvider.GetRequiredService<BackchannelIndustrialAppStoreHttpClient>();
                    var tokenStore = scope.ServiceProvider.GetRequiredService<ITokenStore>();
                    await tokenStore.InitAsync(logonEvent.UserId, logonEvent.SessionId);

                    var userInfo = await client.UserInfo.GetUserInfoAsync(tokenStore, stoppingToken);
                    var groups = await client.Organization.GetGroupMembershipsAsync(tokenStore, stoppingToken);
                    foreach (var group in groups) {
                        _logger.LogInformation($"{userInfo.DisplayName} is a member of {group.DisplayName} (Organisation: {group.Org})");
                    }
                }
            }
        }
    }


    public record UserLogonEvent(string UserId, string SessionId);
}
