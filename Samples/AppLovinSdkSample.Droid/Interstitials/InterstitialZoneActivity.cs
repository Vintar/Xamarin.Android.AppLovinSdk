using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Com.Applovin.Adview;
using Com.Applovin.Sdk;

namespace AppLovinSdkSample.Droid.Interstitials
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class InterstitialZoneActivity : AdStatusActivity, IAppLovinAdLoadListener, IAppLovinAdDisplayListener, IAppLovinAdClickListener, Com.Applovin.Sdk.IAppLovinAdViewEventListener
    {
        private Button showButton;
        private IAppLovinAd currentAd;
        private IAppLovinInterstitialAdDialog interstitialAd;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_interstitial_manual_loading);

            adStatusTextView = FindViewById<TextView>(Resource.Id.status_label);

            interstitialAd = AppLovinInterstitialAd.Create(AppLovinSdk.GetInstance(this), this);

            var loadButton = FindViewById<Button>(Resource.Id.loadButton);
            showButton = FindViewById<Button>(Resource.Id.showButton);

            loadButton.Click += LoadButton_Click;
            showButton.Click += ShowButton_Click;

            interstitialAd.SetAdDisplayListener(this);
            interstitialAd.SetAdClickListener(this);
            interstitialAd.SetAdVideoPlaybackListener(this);
        }

        private void ShowButton_Click(object sender, EventArgs e)
        {
            if (currentAd != null)
            {
                interstitialAd.ShowAndRender(currentAd);
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            Log("Interstitial loading...");
            showButton.Enabled = false;

            AppLovinSdk.GetInstance(ApplicationContext).AdService.LoadNextAdForZoneId("YOUR_ZONE_ID", this);
        }

        public void AdReceived(IAppLovinAd ad)
        {
            Log("Interstitial Loaded");

            currentAd = ad;

            RunOnUiThread(() => showButton.Enabled = true);
        }

        public void FailedToReceiveAd(int errorCode)
        {
            // Look at AppLovinErrorCodes.java for list of error codes
            Log("Interstitial failed to load with error code " + errorCode);
        }

        public void AdDisplayed(IAppLovinAd p0)
        {
            Log("Interstitial Displayed");
        }

        public void AdHidden(IAppLovinAd p0)
        {
            Log("Interstitial Hidden");
        }

        public void AdClicked(IAppLovinAd p0)
        {
            Log("Interstitial Clicked");
        }

        public void VideoPlaybackBegan(IAppLovinAd p0)
        {
            Log("Video Started");
        }

        public void VideoPlaybackEnded(IAppLovinAd p0, double p1, bool p2)
        {
            Log("Video Ended");
        }
    }
}