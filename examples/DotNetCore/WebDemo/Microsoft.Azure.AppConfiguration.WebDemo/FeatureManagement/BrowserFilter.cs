using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace Microsoft.Azure.AppConfiguration.WebDemo.FeatureManagement
{
    //https://github.com/kdcllc/FeatureManagementWorkshop/blob/master/src/FeatureManagement.Core.AspNetCore/FeatureFilters/BrowserFilter.cs
    public class BrowserFilterSettings
    {
        public List<string> AllowedBrowsers { get; set; } = new List<string>();
    }

    [FilterAlias("Browser")]
    public class BrowserFilter : IFeatureFilter
    {
        private const string Chrome = "Chrome";
        private const string Edge = "Edge";

        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BrowserFilter(IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger<BrowserFilter>();
        }

        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            var settings = context.Parameters.Get<BrowserFilterSettings>() ?? new BrowserFilterSettings();

            if (settings.AllowedBrowsers.Any(browser => browser.Contains(Chrome, StringComparison.OrdinalIgnoreCase)) && IsChrome())
            {
                return Task.FromResult(true);
            }

            //if (settings.AllowedBrowsers.Any(browser => browser.Contains(Edge, StringComparison.OrdinalIgnoreCase)) && IsBrowser(Edge))
            //{
            //    return Task.FromResult(true);
            //}

           

            _logger.LogWarning($"The AllowedBrowsers list is empty or the current browser is not enabled for this feature");

            return Task.FromResult(false);
        }

        private string GetUserAgent()
        {
            var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"];
            return userAgent;
        }

        private bool IsBrowser(string browser)
        {
            var userAgent = GetUserAgent();
            return userAgent != null && userAgent.Contains(browser, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsChrome()
        {
            string userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"];

            return userAgent?.Contains("Chrome", StringComparison.OrdinalIgnoreCase) == true
                   && !userAgent.Contains("Ed", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsEdge()
        {
            string userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"];
            return userAgent?.Contains("Ed", StringComparison.OrdinalIgnoreCase) == true
                   && userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase);
        }
    }
}
