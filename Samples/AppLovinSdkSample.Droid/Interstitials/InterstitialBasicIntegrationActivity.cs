using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Com.Applovin.Adview;
using Com.Applovin.Sdk;

namespace AppLovinSdkSample.Droid.Interstitials
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class InterstitialBasicIntegrationActivity : AdStatusActivity, IAppLovinAdLoadListener, IAppLovinAdDisplayListener, IAppLovinAdClickListener, Com.Applovin.Sdk.IAppLovinAdViewEventListener
    {
        private IAppLovinInterstitialAdDialog interstitialAd;
        private Button showButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_interstitial_basic_integration);

            adStatusTextView = (TextView)FindViewById(Resource.Id.status_label);

            interstitialAd = AppLovinInterstitialAd.Create(AppLovinSdk.GetInstance(this), this);

            showButton = (Button)FindViewById(Resource.Id.showButton);
            showButton.Click += ShowButton_Click;
        }

        private void ShowButton_Click(object sender, EventArgs e)
        {
            showButton.Enabled = false;

            Log("Showing...");

            //
            // Optional: Set ad load, ad display, ad click, and ad video playback callback listeners
            //
            interstitialAd.SetAdLoadListener(this);
            interstitialAd.SetAdDisplayListener(this);
            interstitialAd.SetAdClickListener(this);
            interstitialAd.SetAdVideoPlaybackListener(this); // This will only ever be used if you have video ads enabled.

            interstitialAd.Show();
        }

        public void AdClicked(IAppLovinAd appLovinAd)
        {
            Log("Interstitial Clicked");
        }

        public void AdDisplayed(IAppLovinAd appLovinAd)
        {
            Log("Interstitial Displayed");
        }

        public void AdHidden(IAppLovinAd appLovinAd)
        {
            Log("Interstitial Hidden");
        }

        public void AdReceived(IAppLovinAd appLovinAd)
        {
            Log("Interstitial loaded");
            showButton.Enabled = true;
        }

        public void FailedToReceiveAd(int errorCode)
        {
            // Look at AppLovinErrorCodes.java for list of error codes
            Log("Interstitial failed to load with error code " + errorCode);

            showButton.Enabled = true;
        }

        public void VideoPlaybackBegan(IAppLovinAd appLovinAd)
        {
            Log("Video Started");
        }

        public void VideoPlaybackEnded(IAppLovinAd appLovinAd, double percentViewed, bool wasFullyViewed)
        {
            Log("Video Ended");
        }
    }
}