using Android.Content;

namespace AppLovinSdkSample.Droid
{
    public class DemoMenuItem
    {
        private string title;
        private string subtitle;
        private Intent intent;

        public DemoMenuItem(string title, string subtitle, Intent intent)
        {
            this.title = title;
            this.subtitle = subtitle;
            this.intent = intent;
        }

        public string GetTitle()
        {
            return title;
        }

        public string GetSubtitle()
        {
            return subtitle;
        }

        public Intent GetIntent()
        {
            return intent;
        }

        public override string ToString()
        {
            return GetTitle();
        }
    }
}