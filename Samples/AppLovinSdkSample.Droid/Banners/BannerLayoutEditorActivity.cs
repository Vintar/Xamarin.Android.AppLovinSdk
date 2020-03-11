using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Com.Applovin.Adview;
using Com.Applovin.Sdk;

namespace AppLovinSdkSample.Droid.Banners
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class BannerLayoutEditorActivity : AdStatusActivity, IAppLovinAdLoadListener, IAppLovinAdDisplayListener, IAppLovinAdClickListener, Com.Applovin.Adview.IAppLovinAdViewEventListener
    {
        private AppLovinAdView adView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_banner_layout_editor);

            adStatusTextView = FindViewById<TextView>(Resource.Id.status_label);

            // Retrieve banner from layout editor
            adView = FindViewById<AppLovinAdView>(Resource.Id.ad_view);
            var loadButton = FindViewById<Button>(Resource.Id.load_button);

            loadButton.Click += LoadButton_Click;
            adView.SetAdLoadListener(this);
            adView.SetAdDisplayListener(this);
            adView.SetAdClickListener(this);
            adView.SetAdViewEventListener(this);

            // Load an ad!
            adView.LoadNextAd();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            Log("Loading ad...");
            adView.LoadNextAd();
        }

        public void AdReceived(IAppLovinAd ad)
        {
            Log("Banner loaded");
        }

        public void FailedToReceiveAd(int errorCode)
        {
            // Look at AppLovinErrorCodes.java for list of error codes
            Log("Banner failed to load with error code " + errorCode);
        }

        public void AdDisplayed(IAppLovinAd ad)
        {
            Log("Banner Displayed");
        }

        public void AdHidden(IAppLovinAd ad)
        {
            Log("Banner Hidden");
        }

        public void AdClicked(IAppLovinAd ad)
        {
            Log("Banner Clicked");
        }

        public void AdClosedFullscreen(IAppLovinAd p0, AppLovinAdView p1)
        {
            Log("Banner closed fullscreen");
        }

        public void AdFailedToDisplay(IAppLovinAd p0, AppLovinAdView p1, AppLovinAdViewDisplayErrorCode p2)
        {
            Log("Banner failed to display with error code " + p2.ToString());
        }

        public void AdLeftApplication(IAppLovinAd p0, AppLovinAdView p1)
        {
            Log("Banner left application");
        }

        public void AdOpenedFullscreen(IAppLovinAd p0, AppLovinAdView p1)
        {
            Log("Banner opened fullscreen");
        }
    }
}