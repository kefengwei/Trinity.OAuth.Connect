using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;

namespace Trinity.OAuth.Connect.Wexin
{
    public static class WeChatExtensions
    {
        public static AuthenticationBuilder AddWeChat(this AuthenticationBuilder builder)
            => builder.AddWeChat(WeChatDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddWeChat(this AuthenticationBuilder builder, Action<WeChatOptions> configureOptions)
            => builder.AddWeChat(WeChatDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddWeChat(this AuthenticationBuilder builder, string authenticationScheme, Action<WeChatOptions> configureOptions)
            => builder.AddWeChat(authenticationScheme, WeChatDefaults.DisplayName, configureOptions);


        public static AuthenticationBuilder AddWeChat(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<WeChatOptions> configureOptions)
        {

            builder.Services.TryAddTransient<ISecureDataFormat<AuthenticationProperties>>((provider) =>
            {
                var dataProtectionProvider = provider.GetRequiredService<IDataProtectionProvider>();
                var distributedCache = provider.GetRequiredService<IDistributedCache>();

                var dataProtector = dataProtectionProvider.CreateProtector(
                    typeof(WeChatHandler).FullName ?? string.Empty,
                    typeof(string).FullName ?? string.Empty, WeChatDefaults.AuthenticationScheme,
                    "v1");

                var dataFormat = new CachedPropertiesDataFormat(distributedCache, dataProtector);
                return dataFormat;
            });

     
            return builder.AddOAuth<WeChatOptions, WeChatHandler>(authenticationScheme, 
                displayName, configureOptions);
        }



    }
}
