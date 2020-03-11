using System.Collections.Generic;
using Android.App;
using Android.Content;

namespace AppLovinSdkSample.Droid.Mrecs
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MRecDemoMenuActivity : DemoMenuActivity
    {
        protected override List<DemoMenuItem> GetListViewContents()
        {
            var result = new List<DemoMenuItem> 
            {
                new DemoMenuItem("Programmatic", "Programmatically creating an instance of it", new Intent(this, typeof(MRecProgrammaticActivity))),
                new DemoMenuItem("Layout Editor", "Create an MRec from the layout editor", new Intent(this, typeof(MRecLayoutEditorActivity))),
            };

            return result;
        }
    }
}