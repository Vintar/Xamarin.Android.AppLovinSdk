using System;
using Android.App;
using Android.OS;
using Android.Support.Constraints;
using Android.Support.V4.View;
using Android.Widget;
using Com.Applovin.Adview;
using Com.Applovin.Sdk;

namespace AppLovinSdkSample.Droid.Mrecs
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MRecProgrammaticActivity : AdStatusActivity, IAppLovinAdLoadListener, IAppLovinAdDisplayListener, IAppLovinAdClickListener, Com.Applovin.Adview.IAppLovinAdViewEventListener
    {
        private AppLovinAdView adView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_mrec_programmatic);

            adStatusTextView = FindViewById<TextView>(Resource.Id.status_label);

            // Create MRec
            adView = new AppLovinAdView(AppLovinAdSize.Mrec, this);
            adView.Id = ViewCompat.GenerateViewId();

            var mrecConstraintLayout = FindViewById<ConstraintLayout>(Resource.Id.mrec_programmatic_constraint_layout);
            var layoutParams = new ConstraintLayout.LayoutParams(AppLovinSdkUtils.DpToPx(this, AppLovinAdSize.Mrec.Width), AppLovinSdkUtils.DpToPx(this, AppLovinAdSize.Mrec.Height));
            mrecConstraintLayout.AddView(adView, layoutParams);

            var set = new ConstraintSet();
            set.Clone(mrecConstraintLayout);
            set.Connect(adView.Id, ConstraintSet.Top, mrecConstraintLayout.Id, ConstraintSet.Top, AppLovinSdkUtils.DpToPx(this, 80));
            set.CenterHorizontally(adView.Id, mrecConstraintLayout.Id);
            set.ApplyTo(mrecConstraintLayout);

            var loadButton = FindViewById<Button>(Resource.Id.load_button);
            loadButton.Click += LoadButton_Click;

            adView.SetAdLoadListener(this);
            adView.SetAdDisplayListener(this);
            adView.SetAdClickListener(this);
            adView.SetAdViewEventListener(this);
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