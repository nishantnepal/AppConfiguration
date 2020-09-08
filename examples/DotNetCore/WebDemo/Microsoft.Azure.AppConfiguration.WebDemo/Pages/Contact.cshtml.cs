using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement;

namespace Microsoft.Azure.AppConfiguration.WebDemo.Pages
{
    public class ContactModel : PageModel
    {
        public string Message { get; set; }
        public bool BrowserRendererEnabled { get; set; }

        private readonly IFeatureManager _featureManager;
        //private readonly IFeatureManagerSnapshot _featureSnapshot;

        public ContactModel(IFeatureManager featureManager)
        {
            _featureManager = featureManager;
            
        }

        public void OnGet()
        {
            BrowserRendererEnabled = _featureManager.IsEnabledAsync("BrowserRenderer").Result;
            if (BrowserRendererEnabled)
            {
                Message = "Congrats - your are eligible for a discount because your browser qualifies.";
            }
            else
            {
                Message = "Bummer - your browser is not qualified for a discount.";
            }
        }
    }
}
