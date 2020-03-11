using Android.App;
using Android.Content;
using System.Collections.Generic;

namespace AppLovinSdkSample.Droid.Interstitials
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class InterstitialDemoMenuActivity : DemoMenuActivity
    {
        protected override List<DemoMenuItem> GetListViewContents()
        {
            List<DemoMenuItem> result = new List<DemoMenuItem>()
            {
                new DemoMenuItem("Basic Integration", "Quick interstitial ads integration", new Intent(this, typeof(InterstitialManualLoadingActivity))),
                new DemoMenuItem("Manually loading ad", "Use this for greater control over the ad load process", new Intent(this, typeof(InterstitialManualLoadingActivity))),
                new DemoMenuItem("Zone Integration", "Create different user experiences of the same ad type", new Intent(this, typeof(InterstitialZoneActivity)))
            };
        
            return result;
        }
    }
}