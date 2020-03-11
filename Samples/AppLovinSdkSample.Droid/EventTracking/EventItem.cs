using System.Collections.Generic;
using Com.Applovin.Sdk;

namespace AppLovinSdkSample.Droid.EventTracking
{
    public class EventItem
    {
        private string name;
        private string description;
        private string eventType;
        private Dictionary<string, string> parameters;

        public EventItem(string name, string description, string appLovinEventType, Dictionary<string, string> parameters)
        {
            this.name = name;
            this.description = description;
            this.eventType = appLovinEventType;
            this.parameters = parameters;
        }

        public string GetName()
        {
            return name;
        }

        public string GetDescription()
        {
            return description;
        }

        public void TrackEvent(IAppLovinEventService eventService)
        {
            if (eventType.Equals(AppLovinEventTypes.UserCompletedInAppPurchase))
            {
                // eventService.trackInAppPurchase(responseIntentFromOnActivityResult, parameters);
                // responseIntentFromOnActivityResult is the Intent returned to you by Google Play upon a purchase within the onActivityResult method, as described in the Android Developer Portal.
            }
            else
            {
                eventService.TrackEvent(eventType, parameters);
            }
        }
    }
}