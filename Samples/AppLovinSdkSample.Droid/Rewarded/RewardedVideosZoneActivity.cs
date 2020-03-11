using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Widget;
using Com.Applovin.Adview;
using Com.Applovin.Sdk;

namespace AppLovinSdkSample.Droid.Rewarded
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class RewardedVideosZoneActivity : AdStatusActivity, IAppLovinAdLoadListener, IAppLovinAdVideoPlaybackListener, IAppLovinAdRewardListener, IAppLovinAdDisplayListener, IAppLovinAdClickListener
    {
        private AppLovinIncentivizedInterstitial incentivizedInterstitial;
        private Button showButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_rewarded_videos);

            adStatusTextView = FindViewById<TextView>(Resource.Id.status_label);

            var loadButton = FindViewById<Button>(Resource.Id.loadButton);
            showButton = FindViewById<Button>(Resource.Id.showButton);

            incentivizedInterstitial = AppLovinIncentivizedInterstitial.Create("YOUR_ZONE_ID", AppLovinSdk.GetInstance(this));

            loadButton.Click += LoadButton_Click;
            showButton.Click += ShowButton_Click;
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            Log("Rewarded video loading...");
            showButton.Enabled = false;
            incentivizedInterstitial.Preload(this);
        }

        public void AdReceived(IAppLovinAd p0)
        {
            Log("Rewarded video loaded.");
            showButton.Enabled = true;
        }

        public void FailedToReceiveAd(int errorCode)
        {
            Log("Rewarded video failed to load with error code " + errorCode);
        }

        private void ShowButton_Click(object sender, EventArgs e)
        {
            showButton.Enabled = false;
            incentivizedInterstitial.Show(this, this, this, this, this);
        }

        public void UserDeclinedToViewAd(IAppLovinAd p0)
        {
            // This method will be invoked if the user selected "no" when asked if they want to view an ad.
            // If you've disabled the pre-video prompt in the "Manage Apps" UI on our website, then this method won't be called.

            Log("User declined to view ad");
        }

        public void UserOverQuota(IAppLovinAd p0, IDictionary<string, string> p1)
        {
            // Your user has already earned the max amount you allowed for the day at this point, so
            // don't give them any more money. By default we'll show them a alert explaining this,
            // though you can change that from the AppLovin dashboard.

            Log("Reward validation request exceeded quota with response: " + p1);
        }

        public void UserRewardRejected(IAppLovinAd p0, IDictionary<string, string> p1)
        {
            // Your user couldn't be granted a reward for this view. This could happen if you've blacklisted
            // them, for example. Don't grant them any currency. By default we'll show them an alert explaining this,
            // though you can change that from the AppLovin dashboard.

            Log("Reward validation request was rejected with response: " + p1);
        }

        public void UserRewardVerified(IAppLovinAd p0, IDictionary<string, string> p1)
        {
            // AppLovin servers validated the reward. Refresh user balance from your server.  We will also pass the number of coins
            // awarded and the name of the currency.  However, ideally, you should verify this with your server before granting it.

            // i.e. - "Coins", "Gold", whatever you set in the dashboard.
            var currencyName = p1["currency"];

            // For example, "5" or "5.00" if you've specified an amount in the UI.
            var amountGivenString = p1["amount"];

            Log("Rewarded " + amountGivenString + " " + currencyName);

            // By default we'll show a alert informing your user of the currency & amount earned.
            // If you don't want this, you can turn it off in the Manage Apps UI.
        }

        public void ValidationRequestFailed(IAppLovinAd p0, int responseCode)
        {
            if (responseCode == AppLovinErrorCodes.IncentivizedUserClosedVideo)
            {
                // Your user exited the video prematurely. It's up to you if you'd still like to grant
                // a reward in this case. Most developers choose not to. Note that this case can occur
                // after a reward was initially granted (since reward validation happens as soon as a
                // video is launched).
            }
            else if (responseCode == AppLovinErrorCodes.IncentivizedServerTimeout || responseCode == AppLovinErrorCodes.IncentivizedUnknownServerError)
            {
                // Some server issue happened here. Don't grant a reward. By default we'll show the user
                // a alert telling them to try again later, but you can change this in the
                // AppLovin dashboard.
            }
            else if (responseCode == AppLovinErrorCodes.IncentivizedNoAdPreloaded)
            {
                // Indicates that the developer called for a rewarded video before one was available.
                // Note: This code is only possible when working with rewarded videos.
            }

            Log("Reward validation request failed with error code: " + responseCode);
        }

        public void AdDisplayed(IAppLovinAd p0)
        {
            Log("Ad Displayed");
        }

        public void AdHidden(IAppLovinAd p0)
        {
            Log("Ad Dismissed");
        }

        public void AdClicked(IAppLovinAd p0)
        {
            Log("Ad Click");
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