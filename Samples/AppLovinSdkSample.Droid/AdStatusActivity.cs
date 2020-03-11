using Android.App;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace AppLovinSdkSample.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class AdStatusActivity : AppCompatActivity
    {
        protected TextView adStatusTextView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

          //  SupportActionBar.SetBackgroundDrawable(new ColorDrawable("0xDD0A83AA"));
            SupportActionBar.Show();
        }

        protected void Log(string message)
        {
            if (adStatusTextView != null)
            {
                RunOnUiThread(() => adStatusTextView.Text = message);
            }

            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}