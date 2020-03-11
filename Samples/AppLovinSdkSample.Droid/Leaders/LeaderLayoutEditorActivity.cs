using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Com.Applovin.Adview;
using Com.Applovin.Sdk;

namespace AppLovinSdkSample.Droid.Leaders
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class LeaderLayoutEditorActivity : AdStatusActivity, IAppLovinAdLoadListener, IAppLovinAdDisplayListener, IAppLovinAdClickListener, Com.Applovin.Adview.IAppLovinAdViewEventListener
    {
        private AppLovinAdView adView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_leader_layout_editor);

            adStatusTextView = FindViewById<TextView>(Resource.Id.status_label);

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

        public void AdClicked(IAppLovinAd appLovinAd)
        {
            Log("Leader clicked");
        }

        public void AdDisplayed(IAppLovinAd appLovinAd)
        {
            Log("Leader displayed");
        }

        public void AdHidden(IAppLovinAd appLovinAd)
        {
            Log("Leader hidden");
        }

        public void AdReceived(IAppLovinAd appLovinAd)
        {
            Log("Leader loaded");
        }

        public void FailedToReceiveAd(int errorCode)
        {
            // Look at AppLovinErrorCodes.java for list of error codes.
            Log("Leader failed to load with error code " + errorCode);
        }

        public void AdClosedFullscreen(IAppLovinAd p0, AppLovinAdView p1)
        {
            Log("Leader closed fullscreen");
        }

        public void AdFailedToDisplay(IAppLovinAd p0, AppLovinAdView p1, AppLovinAdViewDisplayErrorCode p2)
        {
            Log("Leader failed to display with error code " + p2.ToString());
        }

        public void AdLeftApplication(IAppLovinAd p0, AppLovinAdView p1)
        {
            Log("Leader left application");
        }

        public void AdOpenedFullscreen(IAppLovinAd p0, AppLovinAdView p1)
        {
            Log("Leader opened fullscreen");
        }
    }
}