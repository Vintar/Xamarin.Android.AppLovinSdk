using System.Collections.Generic;
using Android.App;
using Android.Content;

namespace AppLovinSdkSample.Droid.Banners
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class BannerDemoMenuActivity : DemoMenuActivity
    {
        protected override List<DemoMenuItem> GetListViewContents()
        {
            var result = new List<DemoMenuItem> 
            {
                    new DemoMenuItem("Programmatic", "Programmatically creating an instance of it", new Intent(this, typeof(BannerProgrammaticActivity))),
                    new DemoMenuItem( "Layout Editor", "Create a banner from the layout editor", new Intent(this, typeof(BannerLayoutEditorActivity))),
                    new DemoMenuItem( "Zone Integration", "Create different user experiences of the same ad type", new Intent(this, typeof(BannerZoneActivity))),
            };

            return result;
        }
    }
}