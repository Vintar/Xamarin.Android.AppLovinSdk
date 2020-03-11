using System.Collections.Generic;
using Android.App;
using Android.Content;

namespace AppLovinSdkSample.Droid.Leaders
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class LeaderDemoMenuActivity : DemoMenuActivity
    {
        protected override List<DemoMenuItem> GetListViewContents()
        {
            var result = new List<DemoMenuItem>()
            {
                new DemoMenuItem( "Programmatic", "Programmatically create an instance of it", new Intent(this, typeof(LeaderProgrammaticActivity))),
                new DemoMenuItem( "Layout Editor", "Create a Leader from the layout editor", new Intent(this, typeof(LeaderLayoutEditorActivity))),
            };

            return result;
        }
    }
}