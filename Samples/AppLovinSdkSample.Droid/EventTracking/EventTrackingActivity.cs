using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using AppLovinSdkSample.Droid.Adapters;
using Com.Applovin.Sdk;
using Java.Lang;

namespace AppLovinSdkSample.Droid.EventTracking
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class EventTrackingActivity : AppCompatActivity
    {
        private EventItem[] events;
        private IAppLovinEventService eventService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_list);
            SetTitle(Resource.String.title_activity_event_tracking);

            eventService = AppLovinSdk.GetInstance(this).EventService;

            events = new EventItem[] 
            {
                new EventItem(GetString(Resource.String.event_name_began_checkout), GetString(Resource.String.event_description_began_checkout), AppLovinEventTypes.UserBeganCheckout, new Dictionary<string, string>()
                {
                    { AppLovinEventParameters.ProductIdentifier, GetString(Resource.String.event_parameter_product_description) },
                    { AppLovinEventParameters.RevenueAmount, GetString(Resource.String.event_parameter_price_description) },
                    { AppLovinEventParameters.RevenueCurrency, GetString(Resource.String.event_parameter_currency_description) }
                }),
                new EventItem(GetString(Resource.String.event_name_cart), GetString(Resource.String.event_description_cart), AppLovinEventTypes.UserAddedItemToCart, new Dictionary<string, string>()
                {
                    { AppLovinEventParameters.ProductIdentifier, GetString(Resource.String.event_parameter_product_description) }
                }),
                new EventItem(GetString(Resource.String.event_name_achievement), GetString(Resource.String.event_description_achievement), AppLovinEventTypes.UserCompletedAchievement, new Dictionary<string, string>()
                {
                    { AppLovinEventParameters.CompletedAchievementIdentifier, GetString(Resource.String.event_parameter_achievement_description) }
                }),
                new EventItem(GetString(Resource.String.event_name_completed_checkout), GetString(Resource.String.event_description_completed_checkout), AppLovinEventTypes.UserCompletedCheckout, new Dictionary<string, string>()
                {
                    { AppLovinEventParameters.CheckoutTransactionIdentifier, GetString(Resource.String.event_parameter_transaction_description) },
                    { AppLovinEventParameters.ProductIdentifier, GetString(Resource.String.event_parameter_product_description) },
                    { AppLovinEventParameters.RevenueAmount, GetString(Resource.String.event_parameter_amount_description) },
                    { AppLovinEventParameters.RevenueCurrency, GetString(Resource.String.event_parameter_currency_description) }
                }),
                new EventItem(GetString(Resource.String.event_name_level), GetString(Resource.String.event_description_level), AppLovinEventTypes.UserCompletedLevel, new Dictionary<string, string>()
                {
                    { AppLovinEventParameters.CompletedLevelIdentifier, GetString(Resource.String.event_parameter_level_description) }
                }),
                new EventItem(GetString(Resource.String.event_name_reservation), GetString(Resource.String.event_description_reservation), AppLovinEventTypes.UserCreatedReservation, new Dictionary<string, string>()
                {
                    { AppLovinEventParameters.ProductIdentifier, GetString(Resource.String.event_parameter_product_description) },
                    { AppLovinEventParameters.ReservationStartTimestamp, Long.ToString(JavaSystem.CurrentTimeMillis() / 1000L) },
                    { AppLovinEventParameters.ReservationEndTimestamp, Long.ToString(JavaSystem.CurrentTimeMillis() / 1000L) }
                }),
                new EventItem(GetString(Resource.String.event_name_in_app_purchase), GetString(Resource.String.event_description_in_app_purchase), AppLovinEventTypes.UserCompletedInAppPurchase, new Dictionary<string, string>()
                {
                    { AppLovinEventParameters.RevenueAmount, GetString(Resource.String.event_parameter_amount_description) },
                    { AppLovinEventParameters.RevenueCurrency, GetString(Resource.String.event_parameter_currency_description) }
                }),
                new EventItem(GetString(Resource.String.event_name_login), GetString(Resource.String.event_description_login), AppLovinEventTypes.UserLoggedIn, new Dictionary<string, string>()
                {
                    { AppLovinEventParameters.UserAccountIdentifier, GetString(Resource.String.event_parameter_user_description) }
                }),
                new EventItem(GetString(Resource.String.event_name_payment_info), GetString(Resource.String.event_description_payment_info), AppLovinEventTypes.UserProvidedPaymentInformation, new Dictionary<string, string>()),
                new EventItem(GetString(Resource.String.event_name_registration),  GetString(Resource.String.event_description_registration), AppLovinEventTypes.UserCreatedAccount, new Dictionary<string, string>()
                {
                    { AppLovinEventParameters.UserAccountIdentifier, GetString(Resource.String.event_parameter_user_description) }
                }),
                new EventItem(GetString(Resource.String.event_name_search), GetString(Resource.String.event_description_search), AppLovinEventTypes.UserExecutedSearch, new Dictionary<string, string>()
                {
                    { AppLovinEventParameters.SearchQuery, GetString(Resource.String.event_parameter_search_description) }
                }),
                new EventItem(GetString(Resource.String.event_name_invitation), GetString(Resource.String.event_description_invitation), AppLovinEventTypes.UserSentInvitation, new Dictionary<string, string>()),
                new EventItem(GetString(Resource.String.event_name_shared_link), GetString(Resource.String.event_description_shared_link), AppLovinEventTypes.UserSharedLink, new Dictionary<string, string>()),
                new EventItem(GetString(Resource.String.event_name_virt_currency), GetString(Resource.String.event_description_virt_currency), AppLovinEventTypes.UserSpentVirtualCurrency, new Dictionary<string, string>()
                {
                    { AppLovinEventParameters.VirtualCurrencyAmount, GetString(Resource.String.event_parameter_virt_amount_description) },
                    { AppLovinEventParameters.VirtualCurrencyName, GetString(Resource.String.event_paramter_virt_currency_description) }
                }),
                new EventItem(GetString(Resource.String.event_name_tutorial), GetString(Resource.String.event_description_tutorial), AppLovinEventTypes.UserCompletedTutorial, new Dictionary<string, string>()),
                new EventItem(GetString(Resource.String.event_name_viewed_content), GetString(Resource.String.event_description_viewed_content), AppLovinEventTypes.UserViewedContent, new Dictionary<string, string>()
                {
                    { AppLovinEventParameters.ContentIdentifier, GetString(Resource.String.event_parameter_content_description) }
                }),
                new EventItem(GetString(Resource.String.event_name_viewed_product), GetString(Resource.String.event_description_viewed_product), AppLovinEventTypes.UserViewedProduct, new Dictionary<string, string>()
                {
                    { AppLovinEventParameters.ProductIdentifier, GetString(Resource.String.event_parameter_product_description) }
                }),
                new EventItem(GetString(Resource.String.event_name_wishlist), GetString(Resource.String.event_description_wishlist), AppLovinEventTypes.UserAddedItemToWishlist, new Dictionary<string, string>()
                {
                    { AppLovinEventParameters.ProductIdentifier, GetString(Resource.String.event_parameter_product_description) }
                })
            };

            var listView = FindViewById<ListView>(Resource.Id.listView);
            listView.Adapter = new EventItemArrayAdapter(this, Android.Resource.Layout.SimpleExpandableListItem2, events);
            listView.ItemClick += ListView_ItemClick;
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var @event = events[e.Position];
            @event.TrackEvent(eventService);

            var eventName = @event.GetName();
            Title = eventName;
        }
    }
}