using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Com.Applovin.Adview;
using Com.Applovin.Sdk;

namespace AppLovinSdkSample.Droid.Mrecs
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MRecLayoutEditorActivity : AdStatusActivity, IAppLovinAdLoadListener, IAppLovinAdDisplayListener, IAppLovinAdClickListener, Com.Applovin.Adview.IAppLovinAdViewEventListener
    {
        private AppLovinAdView adView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_mrec_layout_editor);

            adStatusTextView = (TextView)FindViewById(Resource.Id.status_label);

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

        public void AdClicked(IAppLovinAd appLovinAd)
        {
            Log("MRec clicked");
        }

        public void AdDisplayed(IAppLovinAd appLovinAd)
        {
            Log("MRec displayed");
        }

        public void AdHidden(IAppLovinAd appLovinAd)
        {
            Log("MRec hidden");
        }

        public void AdReceived(IAppLovinAd appLovinAd)
        {
            Log("MRec loaded");
        }

        public void FailedToReceiveAd(int errorCode)
        {
            // Look at AppLovinErrorCodes.java for list of error codes.
            Log("MRec failed to load with error code " + errorCode);
        }

        public void AdClosedFullscreen(IAppLovinAd p0, AppLovinAdView p1)
        {
            Log("MRec closed fullscreen");
        }

        public void AdFailedToDisplay(IAppLovinAd p0, AppLovinAdView p1, AppLovinAdViewDisplayErrorCode p2)
        {
            Log("MRec failed to display with error code " + p2.ToString());
        }

        public void AdLeftApplication(IAppLovinAd p0, AppLovinAdView p1)
        {
            Log("MRec left application");
        }

        public void AdOpenedFullscreen(IAppLovinAd p0, AppLovinAdView p1)
        {
            Log("MRec opened fullscreen");
        }
    }
}