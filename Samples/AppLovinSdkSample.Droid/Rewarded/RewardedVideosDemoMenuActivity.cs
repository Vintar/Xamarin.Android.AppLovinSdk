using System.Collections.Generic;
using Android.App;
using Android.Content;

namespace AppLovinSdkSample.Droid.Rewarded
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class RewardedVideosDemoMenuActivity : DemoMenuActivity
    {
        protected override List<DemoMenuItem> GetListViewContents()
        {
            var result = new List<DemoMenuItem>
            {
                new DemoMenuItem( "Basic Integration", "Quick rewarded video integration", new Intent(this, typeof(RewardedVideosActivity))),
                new DemoMenuItem( "Zone Integration", "Create different user experiences of the same ad type", new Intent(this, typeof(RewardedVideosZoneActivity)))
            };

            return result;
        }
    }
}