using System.Threading.Channels;

using IntelligentPlant.IndustrialAppStore.Authentication;
using IntelligentPlant.IndustrialAppStore.Client;

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
                    var client = scope.ServiceProvider.GetRequiredService<IndustrialAppStoreHttpClient>();
                    var tokenStore = scope.ServiceProvider.GetRequiredService<ITokenStore>();
                    await tokenStore.InitAsync(logonEvent.UserId, logonEvent.SessionId);

                    var userInfo = await client.UserInfo.GetUserInfoAsync(stoppingToken);
                    var groups = await client.Organization.GetGroupMembershipsAsync(stoppingToken);
                    foreach (var group in groups) {
                        _logger.LogInformation($"{userInfo.DisplayName} is a member of {group.DisplayName} (Organisation: {group.Org})");
                    }
                }
            }
        }
    }


    public record UserLogonEvent(string UserId, string SessionId);
}
