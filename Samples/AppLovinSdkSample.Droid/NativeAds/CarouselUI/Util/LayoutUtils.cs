//using Android.Content;
//using Android.Views;
//using Android.Widget;
//using Com.Applovin.Sdk;

//namespace AppLovinSdkSample.Droid.NativeAds.CarouselUI.Util
//{
//    public class LayoutUtils
//    {
//        public static int MATCH_PARENT = ViewGroup.LayoutParams.MatchParent;
//        public static int WRAP_CONTENT = ViewGroup.LayoutParams.WrapContent;

//        /**
//         * NOTE FOR UNDERSTANDING GRAVITY / LAYOUT GRAVITY "Layout Gravity" affects your position in the superview. "Gravity" affects the position of your subviews within you.
//         *
//         * Said another way, Layout Gravity positions you yourself while gravity positions your children.
//         */

//        public static LinearLayout.LayoutParams CreateLinearParams(int width, int height, int layoutGravity)
//        {
//            var @params = new LinearLayout.LayoutParams(width, height)
//            {
//                Gravity = (GravityFlags)layoutGravity
//            };

//            return @params;
//        }

//        public static FrameLayout.LayoutParams CreateFrameParams(int width, int height, int layoutGravity)
//        {
//            var @params = new FrameLayout.LayoutParams(width, height)
//            {
//                Gravity = (GravityFlags)layoutGravity
//            };

//            return @params;
//        }

//        public static LinearLayout.LayoutParams CreateLinearParams(int width, int height, int layoutGravity, Margins margins)
//        {
//            var @params = new LinearLayout.LayoutParams(width, height)
//            {
//                Gravity = (GravityFlags)layoutGravity
//            };

//            @params.SetMargins(margins.Left, margins.Top, margins.Right, margins.Bottom);

//            return @params;
//        }

//        public static FrameLayout.LayoutParams CreateFrameParams(int width, int height, int layoutGravity, Margins margins)
//        {
//            var @params = new FrameLayout.LayoutParams(width, height)
//            {
//                Gravity = (GravityFlags)layoutGravity
//            };

//            @params.SetMargins(margins.Left, margins.Top, margins.Right, margins.Bottom);
            
//            return @params;
//        }

//        public class Margins
//        {
//            public int Left { get; private set; }
//            public int Top { get; private set; }
//            public int Right { get; private set; }
//            public int Bottom { get; private set; }

//            public Margins(int left, int top, int right, int bottom)
//            {
//                Left = left;
//                Top = top;
//                Right = right;
//                Bottom = bottom;
//            }
//        }

//        public class DPMargins : Margins
//        {
//            public DPMargins(Context context, int left, int top, int right, int bottom) : base(AppLovinSdkUtils.DpToPx(context, left), AppLovinSdkUtils.DpToPx(context, top), AppLovinSdkUtils.DpToPx(context, right), AppLovinSdkUtils.DpToPx(context, bottom))
//            {
//            }
//        }
//    }
//}