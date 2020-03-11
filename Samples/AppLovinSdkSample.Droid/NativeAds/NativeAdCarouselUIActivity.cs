//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Android.App;
//using Android.Graphics.Drawables;
//using Android.OS;
//using Android.Views;
//using Android.Widget;
//using Com.Applovin.NativeAds;
//using Com.Applovin.Sdk;

//namespace AppLovinSdkSample.Droid.NativeAds
//{
//    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
//    public class NativeAdCarouselUIActivity : AdStatusActivity, IAppLovinNativeAdPrecacheListener, IAppLovinNativeAdLoadListener, IAppLovinPostbackListener
//    {
//        private const int NUM_ADS_TO_LOAD = 1;

//        private IAppLovinNativeAd nativeAd;

//        private ImageView appIcon;
//        private ImageView appRating;
//        private TextView appTitleTextView;
//        private TextView appDescriptionTextView;
//        private Button appDownloadButton;

//        private FrameLayout mediaViewPlaceholder;

//        private TextView impressionStatusTextView;
//        private Button loadButton;
//        private Button precacheButton;
//        private Button showButton;

//        protected override void OnCreate(Bundle savedInstanceState)
//        {
//            base.OnCreate(savedInstanceState);
//            SetContentView(Resource.Layout.activity_native_ad_carousel_ui);

//            adStatusTextView = FindViewById<TextView>(Resource.Id.status_label);
//            impressionStatusTextView = FindViewById<TextView>(Resource.Id.impressionStatusTextView);
//            appRating = FindViewById<ImageView>(Resource.Id.appRating);
//            appTitleTextView = FindViewById<TextView>(Resource.Id.appTitleTextView);
//            appDescriptionTextView = FindViewById<TextView>(Resource.Id.appDescriptionTextView);
//            mediaViewPlaceholder = FindViewById<FrameLayout>(Resource.Id.mediaViewPlaceholder);

//            appIcon = FindViewById<ImageView>(Resource.Id.appIcon);
//            appIcon.Click += AppIcon_Click;

//            loadButton = FindViewById<Button>(Resource.Id.loadButton);
//            loadButton.Click += LoadButton_Click;

//            precacheButton = FindViewById<Button>(Resource.Id.precacheButton);
//            precacheButton.Click += PrecacheButton_Click;

//            showButton = FindViewById<Button>(Resource.Id.showButton);
//            showButton.Click += ShowButton_Click;

//            appDownloadButton = FindViewById<Button>(Resource.Id.appDownloadButton);
//            appDownloadButton.Click += AppDownloadButton_Click;
//        }

//        private void AppIcon_Click(object sender, EventArgs e)
//        {
//            if (nativeAd != null)
//            {
//                nativeAd.LaunchClickTarget(FindViewById<View>(Android.Resource.Id.content).Context);
//            }
//        }

//        private void LoadButton_Click(object sender, EventArgs e)
//        {
//            Log("Native ad loading...");

//            loadButton.Enabled = false;
//            precacheButton.Enabled = false;
//            showButton.Enabled = false;

//            impressionStatusTextView.Text = "No impression to track";

//            LoadNativeAds(NUM_ADS_TO_LOAD);
//        }

//        private void PrecacheButton_Click(object sender, EventArgs e)
//        {
//            Log("Native ad precaching...");
//            var sdk = AppLovinSdk.GetInstance(ApplicationContext);

//            sdk.NativeAdService.PrecacheResources(nativeAd, this);
//        }

//        public void OnNativeAdImagePrecachingFailed(IAppLovinNativeAd p0, int i)
//        {
//            Log("Native ad failed to precache images with error code " + i);
//        }

//        public void OnNativeAdImagesPrecached(IAppLovinNativeAd p0)
//        {
//            Log("Native ad precached images");
//        }

//        public void OnNativeAdVideoPrecachingFailed(IAppLovinNativeAd p0, int i)
//        {
//            Log("Native ad failed to precache videos with error code " + i);
//        }

//        public void OnNativeAdVideoPreceached(IAppLovinNativeAd p0)
//        {
//            // This will get called whether an ad actually has a video to precache or not
//            Log("Native ad done precaching");

//            RunOnUiThread(() =>
//            {
//                showButton.Enabled = true;
//                precacheButton.Enabled = false;
//            });
//        }

//        private void ShowButton_Click(object sender, EventArgs e)
//        {
//            RunOnUiThread(() =>
//            {
//                Log("Native ad rendered");

//                loadButton.Enabled = true;
//                showButton.Enabled = false;

//                appTitleTextView.Text = nativeAd.Title;
//                appDescriptionTextView.Text = nativeAd.DescriptionText;

//                AppLovinSdkUtils.SafePopulateImageView(appIcon, Android.Net.Uri.Parse(nativeAd.IconUrl), AppLovinSdkUtils.DpToPx(ApplicationContext, AppLovinCarouselViewSettings.ICON_IMAGE_MAX_SCALE_SIZE));

//                var starRatingDrawable = GetStarRatingDrawable(nativeAd.StarRating);
//                appRating.SetImageDrawable(starRatingDrawable);

//                appDownloadButton.Text = nativeAd.CtaText;

//                var mediaView = new InlineCarouselCardMediaView(NativeAdCarouselUIActivity.this);
//                mediaView.SetAd(nativeAd);
//                mediaView.SetCardState(new InlineCarouselCardState());
//                mediaView.SetSdk(AppLovinSdk.getInstance(getApplicationContext()));
//                mediaView.SetUiHandler(new Handler(Looper.getMainLooper()));
//                mediaView.SetUpView();
//                mediaView.AutoplayVideo();

//                mediaViewPlaceholder.RemoveAllViews();
//                mediaViewPlaceholder.AddView(mediaView);

//                //
//                // You are responsible for firing impressions
//                //
//                TrackImpression(nativeAd);
//            });
//        }

//        private void AppDownloadButton_Click(object sender, EventArgs e)
//        {
//            if (nativeAd != null)
//            {
//                nativeAd.LaunchClickTarget(FindViewById<View>(Android.Resource.Id.content).Context);
//            }
//        }

//        public void LoadNativeAds(int numAdsToLoad)
//        {
//            var sdk = AppLovinSdk.GetInstance(ApplicationContext);
//            sdk.NativeAdService.LoadNativeAds(numAdsToLoad, this);
//        }

//        public void OnNativeAdsFailedToLoad(int errorCode)
//        {
//            // Native ads failed to load for some reason, likely a network error.
//            // Compare errorCode to the available constants in AppLovinErrorCodes.

//            Log("Native ad failed to load with error code " + errorCode);

//            if (errorCode == AppLovinErrorCodes.NoFill)
//            {
//                // No ad was available for this placement
//            }
//            // else if (errorCode == .... ) { ... }
//        }

//        public void OnNativeAdsLoaded(IList<IAppLovinNativeAd> list)
//        {
//            // Native ads loaded; do something with this, e.g. render into your custom view.

//            RunOnUiThread(() =>
//            {
//                Log("Native ad loaded, assets not retrieved yet.");

//                nativeAd = (IAppLovinNativeAd)list.First();
//                precacheButton.Enabled = true;
//            });
//        }

//        // Track an impression, though all other postbacks are handled identically
//        private void TrackImpression(IAppLovinNativeAd nativeAd)
//        {
//            impressionStatusTextView.Text = "Tracking Impression...";

//            nativeAd.TrackImpression(this);
//        }

//        public void OnPostbackFailure(string p0, int p1)
//        {
//            RunOnUiThread(() => impressionStatusTextView.Text = "Impression Failed to Track!");
//        }

//        public void OnPostbackSuccess(string p0)
//        {
//            RunOnUiThread(() => impressionStatusTextView.Text = "Impression Tracked!");
//        }

//        private Drawable GetStarRatingDrawable(float starRating)
//        {
//            var sanitizedRating = starRating.ToString().Replace(".", "_");
//            var resourceName = "applovin_star_sprite_" + sanitizedRating;
//            var drawableId = ApplicationContext.Resources.GetIdentifier(resourceName, "drawable", ApplicationContext.PackageName);

//            return ApplicationContext.Resources.GetDrawable(drawableId);
//        }
//    }
//}