using Microsoft.Extensions.Logging;

namespace Micro.Starter.Api.Configs
{
    public class SlackLoggingConfig
    {
        public string WebhookUrl { set; get; }
        public LogLevel MinLogLevel { set; get; }
    }
}
