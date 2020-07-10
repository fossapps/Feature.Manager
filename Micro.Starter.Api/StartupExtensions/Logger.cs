using System;
using Micro.Starter.Api.Configs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Slack;

namespace Micro.Starter.Api.StartupExtensions
{
    public static class Logger
    {
        public static void ConfigureLoggerWithSlack(this ILoggerFactory loggerFactory, SlackLoggingConfig slackConfig,
            IHostEnvironment env)
        {
            if (string.IsNullOrEmpty(slackConfig.WebhookUrl))
            {
                return;
            }
            loggerFactory.AddSlack(new SlackConfiguration
            {
                MinLevel = slackConfig.MinLogLevel,
                WebhookUrl = new Uri(slackConfig.WebhookUrl)
            }, env.ApplicationName, env.EnvironmentName);
        }
    }
}