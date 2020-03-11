//using System;
//using System.Collections.Generic;
//using Android.App;
//using Android.OS;
//using Android.Support.V7.Widget;
//using Android.Views;
//using Android.Widget;
//using Com.Applovin.NativeAds;
//using Com.Applovin.Sdk;

//namespace AppLovinSdkSample.Droid.NativeAds
//{
//    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
//    public class NativeAdRecyclerViewActivity : AdStatusActivity, IAppLovinNativeAdLoadListener, IAppLovinNativeAdPrecacheListener
//    {
//        private RecyclerView recyclerView;
//        private IAppLovinNativeAdService nativeAdService;
//        private Activity activityRef;
//        IList<IAppLovinNativeAd> nativeAds;

//        protected override void OnCreate(Bundle savedInstanceState)
//        {
//            base.OnCreate(savedInstanceState);
//            SetContentView(Resource.Layout.activity_native_ad_recycler_view);
//            recyclerView = FindViewById<RecyclerView>(Resource.Id.nativeAdsRecyclerView);

//            // Load an initial batch of native ads.
//            // In a real app, you'd ideally load smaller batches, and load more as the user scrolls.
//            var sdk = AppLovinSdk.GetInstance(this);
//            activityRef = this;

//            nativeAdService = sdk.NativeAdService;
//            nativeAdService.LoadNativeAds(10, this);
//        }

//        public void OnNativeAdsFailedToLoad(int errorCode)
//        {
//            RunOnUiThread(() => Toast.MakeText(activityRef, "Failed to load native ads: " + errorCode, ToastLength.Short).Show());
//        }

//        public void OnNativeAdsLoaded(IList<IAppLovinNativeAd> list)
//        {
//            nativeAds = list;

//            RunOnUiThread(() =>
//            {
//                RenderRecyclerView(list);
//                RetrieveImageResources(nativeAdService, list);
//            });
//        }

//        private void RetrieveImageResources(IAppLovinNativeAdService nativeAdService, IList<IAppLovinNativeAd> nativeAds)
//        {
//            foreach (var nativeAd in nativeAds)
//            {
//                nativeAdService.PrecacheResources(nativeAd, this);
//            }
//        }

//        public void OnNativeAdImagePrecachingFailed(IAppLovinNativeAd p0, int errorCode)
//        {
//            RunOnUiThread(() => Toast.MakeText(this, "Failed to load images for native ad: " + errorCode, ToastLength.Long).Show());
//        }

//        public void OnNativeAdImagesPrecached(IAppLovinNativeAd appLovinNativeAd)
//        {
//            RunOnUiThread(() =>
//            {
//                var adapter = (NativeAdRecyclerViewAdapter)recyclerView.GetAdapter();
//                adapter.NotifyItemChanged(nativeAds.IndexOf(appLovinNativeAd));
//            });
//        }

//        public void OnNativeAdVideoPrecachingFailed(IAppLovinNativeAd p0, int p1)
//        {
//            // This example does not implement videos; see NativeAdCarouselUIActivity for an example of a widget which does.
//        }

//        public void OnNativeAdVideoPreceached(IAppLovinNativeAd appLovinNativeAd)
//        {
//            // This example does not implement videos; see NativeAdCarouselUIActivity for an example of a widget which does.
//        }

//        private void RenderRecyclerView(IList<IAppLovinNativeAd> nativeAds)
//        {
//            recyclerView.SetLayoutManager(new LinearLayoutManager(this));
//            recyclerView.SetAdapter(new NativeAdRecyclerViewAdapter(this, nativeAds));
//        }

//        public void OnRecyclerViewItemClicked(View clickedView, IList<IAppLovinNativeAd> nativeAds)
//        {
//            int itemPosition = this.recyclerView.GetChildAdapterPosition(clickedView);
//            var ad = nativeAds[itemPosition];
//            ad.LaunchClickTarget(this);
//        }

//        private class NativeAdRecyclerViewHolder : RecyclerView.ViewHolder
//        {
//            private TextView appTitleTextView;
//            private TextView appDescriptionTextView;
//            private ImageView appIconImageView;

//            public NativeAdRecyclerViewHolder(View itemView) : base(itemView)
//            {
//                appTitleTextView = itemView.FindViewById<TextView>(Resource.Id.appTitleTextView);
//                appDescriptionTextView = itemView.FindViewById<TextView>(Resource.Id.appDescriptionTextView);
//                appIconImageView = itemView.FindViewById<ImageView>(Resource.Id.appIconImageView);
//            }

//            public TextView GetAppTitleTextView()
//            {
//                return appTitleTextView;
//            }

//            public TextView GetAppDescriptionTextView()
//            {
//                return appDescriptionTextView;
//            }

//            public ImageView GetAppIconImageView()
//            {
//                return appIconImageView;
//            }
//        }

//        private class NativeAdRecyclerViewAdapter : RecyclerView.Adapter
//        {
//            private readonly NativeAdRecyclerViewActivity _activity;
//            private IList<IAppLovinNativeAd> nativeAds;

//            public NativeAdRecyclerViewAdapter(NativeAdRecyclerViewActivity activity, IList<IAppLovinNativeAd> nativeAds)
//            {
//                _activity = activity;
//                this.nativeAds = nativeAds;
//            }

//            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
//            {
//                var prototypeView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.recycler_view_cell_nativead, parent, false);

//                if (!prototypeView.HasOnClickListeners)
//                {
//                    prototypeView.Click += PrototypeView_Click;
//                }

//                return new NativeAdRecyclerViewHolder(prototypeView);
//            }

//            private void PrototypeView_Click(object sender, EventArgs e)
//            {
//                _activity.OnRecyclerViewItemClicked((sender as View), nativeAds);
//            }

//            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
//            {
//                var nativeAd = nativeAds[position];

//                (holder as NativeAdRecyclerViewHolder).GetAppTitleTextView().Text = nativeAd.Title;
//                (holder as NativeAdRecyclerViewHolder).GetAppDescriptionTextView().Text = nativeAd.DescriptionText;

//                int maxSizeDp = 50; // match the size defined in the XML layout
//                AppLovinSdkUtils.SafePopulateImageView((holder as NativeAdRecyclerViewHolder).GetAppIconImageView(), Android.Net.Uri.Parse(nativeAd.ImageUrl), maxSizeDp);

//                // Track impression
//                nativeAd.TrackImpression();
//            }

//            public override int ItemCount => (nativeAds != null) ? nativeAds.Count : 0;
//        }
//    }
//}