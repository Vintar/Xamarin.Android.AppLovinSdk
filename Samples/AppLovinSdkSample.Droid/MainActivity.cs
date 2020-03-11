using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using AppLovinSdkSample.Droid.Banners;
using AppLovinSdkSample.Droid.EventTracking;
using AppLovinSdkSample.Droid.Interstitials;
using AppLovinSdkSample.Droid.Mrecs;
using AppLovinSdkSample.Droid.Rewarded;
using Com.Applovin.Sdk;

namespace AppLovinSdkSample.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : DemoMenuActivity
    {
        private IMenuItem muteToggleMenuItem;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AppLovinSdk.GetInstance(this).InitializeSdk(this);

            // Set an identifier for the current user. This identifier will be tied to various analytics events and rewarded video validation
            AppLovinSdk.GetInstance(this).UserIdentifier = "support@applovin.com";

            // Check that SDK key is present in Android Manifest
            CheckSdkKey();
        }

        protected override List<DemoMenuItem> GetListViewContents()
        {
            var result = new List<DemoMenuItem>()
            {
                new DemoMenuItem( "Interstitials", "Full screen ads. Graphic or video", new Intent(this, typeof(InterstitialDemoMenuActivity))),
                new DemoMenuItem( "Rewarded Videos (Incentivized Ads)", "Reward your users for watching these on-demand videos", new Intent(this, typeof(RewardedVideosDemoMenuActivity))),
               // new DemoMenuItem( "Native Ads", "In-content ads that blend in seamlessly", new Intent(this, typeof(NativeAdDemoMenuActivity))),
                new DemoMenuItem( "Banners", "320x50 Classic banner ads", new Intent(this, typeof(BannerDemoMenuActivity))),
                new DemoMenuItem( "MRECs", "300x250 Rectangular ads typically used in-content", new Intent(this, typeof(MRecDemoMenuActivity))),
                new DemoMenuItem( "Event Tracking", "Track in-app events for your users", new Intent(this, typeof(EventTrackingActivity))),
                new DemoMenuItem( "Resources", "https://support.applovin.com/support/home", new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://support.applovin.com/support/home"))),
                new DemoMenuItem( "Contact", "support@applovin.com", GetContactIntent())
            };

            return result;
        }

        private void CheckSdkKey()
        {
            var currentSdkKey = "YOUR_SDK_KEY";
            var sdkKey = AppLovinSdk.GetInstance(ApplicationContext).SdkKey;

            if (!currentSdkKey.Equals(sdkKey))
            {
                new AlertDialog.Builder(this)
                    .SetTitle("ERROR")
                    .SetMessage("Please update your sdk key in the manifest file.")
                    .SetCancelable(false)
                    .SetNeutralButton("OK", listener: null)
                    .Show();
            }
        }

        private Intent GetContactIntent()
        {
            Intent intent = new Intent(Intent.ActionSendto);
            intent.SetType("text/plain");
            intent.SetData(Android.Net.Uri.Parse("mailto:" + "support@applovin.com"));
            intent.PutExtra(Intent.ExtraSubject, "Android SDK support");
            intent.PutExtra(Intent.ExtraText, "\n\n\n---\nSDK Version: " + AppLovinSdk.Version);
            return intent;
        }

        // Mute Toggling

        /**
         * Toggling the sdk mute setting will affect whether your video ads begin in a muted state or not.
         */
        private void toggleMute()
        {
            AppLovinSdk sdk = AppLovinSdk.GetInstance(this);
            sdk.Settings.Muted = !sdk.Settings.Muted;
            muteToggleMenuItem.SetIcon(GetMuteIconForCurrentSdkMuteSetting());
        }

        private Drawable GetMuteIconForCurrentSdkMuteSetting()
        {
            AppLovinSdk sdk = AppLovinSdk.GetInstance(this);
            int drawableId = sdk.Settings.Muted ? Resource.Drawable.mute : Resource.Drawable.unmute;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.LollipopMr1)
            {
                return Resources.GetDrawable(drawableId, Theme);
            }
            else
            {
                return Resources.GetDrawable(drawableId);
            }
        }

        protected override void SetupListViewFooter()
        {
            string appVersion = "";
            try
            {
                var pInfo = PackageManager.GetPackageInfo(PackageName, 0);
                appVersion = pInfo.VersionName;
            }
            catch (PackageManager.NameNotFoundException e)
            {
                e.PrintStackTrace();
            }

            var versionName = Build.VERSION.Sdk;
            var apiLevel = Build.VERSION.SdkInt;

            var footer = new TextView(ApplicationContext);
            footer.SetTextColor(Color.Gray);
            footer.SetPadding(0, 20, 0, 0);
            footer.Gravity = GravityFlags.Center;
            footer.TextSize = 18;
            footer.Text = "\nApp Version: " + appVersion +
                "\nSDK Version: " + AppLovinSdk.Version +
                "\nOS Version: " + versionName + "(API " + apiLevel + ")";

            listView.AddFooterView(footer );
            listView.SetFooterDividersEnabled(false);
        }

        //Options Menu Functions

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);

            return true;
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            muteToggleMenuItem = menu.FindItem(Resource.Id.action_toggle_mute);
            muteToggleMenuItem.SetIcon(GetMuteIconForCurrentSdkMuteSetting());

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_toggle_mute)
            {
                toggleMute();
            }

            return true;
        }
    }
}