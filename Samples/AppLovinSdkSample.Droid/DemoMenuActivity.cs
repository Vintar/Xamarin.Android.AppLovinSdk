using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Com.Applovin.Sdk;
using static Com.Applovin.Sdk.AppLovinSdk;
using Android.Content;
using System.Collections.Generic;
using AppLovinSdkSample.Droid.Adapters;

namespace AppLovinSdkSample.Droid
{
    public abstract class DemoMenuActivity : AppCompatActivity, ISdkInitializationListener
    {
        protected ListView listView;
        private List<DemoMenuItem> items;
        protected abstract List<DemoMenuItem> GetListViewContents();

        public void OnSdkInitialized(IAppLovinSdkConfiguration p0)
        {
            // SDK finished initialization
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_list);

            listView = FindViewById<ListView>(Resource.Id.listView);

            SetupListViewFooter();
            SetupListViewContents(GetListViewContents());
        }

        protected virtual void SetupListViewFooter()
        {
        }

        private void SetupListViewContents(List<DemoMenuItem> items)
        {
            this.items = items;
            listView.Adapter = new DemoMenuItemArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, items.ToArray());
            listView.ItemClick += ListView_ItemClick;
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = items[e.Position].GetIntent();

            if (intent != null)
            {
                StartActivity(intent);
            }
        }
    }
}