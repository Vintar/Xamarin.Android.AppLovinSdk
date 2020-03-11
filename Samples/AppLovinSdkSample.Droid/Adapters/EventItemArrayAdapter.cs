using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using AppLovinSdkSample.Droid.EventTracking;

namespace AppLovinSdkSample.Droid.Adapters
{
    public class EventItemArrayAdapter : ArrayAdapter<EventItem>
    {
        private EventItem[] items;

        public EventItemArrayAdapter(Activity context, int textViewResourceId, EventItem[] items) : base(context, textViewResourceId, items)
        {
            this.items = items;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var row = convertView;

            if (row == null)
            {
                LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                row = inflater.Inflate(Android.Resource.Layout.SimpleListItem2, parent, false);
            }

            var item = items[position];

            var title = row.FindViewById<TextView>(Android.Resource.Id.Text1);
            title.Text = item.GetName();
            var subtitle = row.FindViewById<TextView>(Android.Resource.Id.Text2);
            subtitle.Text = item.GetDescription();

            return row;
        }
    }
}