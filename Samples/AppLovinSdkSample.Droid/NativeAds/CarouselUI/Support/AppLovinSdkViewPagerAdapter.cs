//using System;
//using Android.Views;

//namespace AppLovinSdkSample.Droid.NativeAds.CarouselUI.Support
//{
//    public abstract class AppLovinSdkViewPagerAdapter : SdkPagerAdapter
//    {
//        /**
//        * Get a View that displays the data at the specified position in the data set.
//        *
//        * @param position The position of the item within the adapter's data set of the item whose view we want.
//        * @param pager    The ViewPager that this view will eventually be attached to.
//        *
//        * @return A View corresponding to the data at the specified position.
//        */
//        public abstract View GetView(int position, SdkCenteredViewPager pager);

//        /**
//        * Determines whether a page View is associated with a specific key object as
//        * returned by instantiateItem(ViewGroup, int).
//        *
//        * @param view   Page View to check for association with object
//        * @param object Object to check for association with view
//        *
//        * @return true if view is associated with the key object object.
//        */
//        public override bool IsViewFromObject(View view, object @object)
//        {
//            return view == @object;
//        }

//        /**
//         * Create the page for the given position.
//         *
//         * @param container The containing View in which the page will be shown.
//         * @param position  The page position to be instantiated.
//         *
//         * @return Returns an Object representing the new page. This does not need
//         * to be a View, but can be some other container of the page.
//         */
//        public override object InstantiateItem(ViewGroup container, int position)
//        {
//            SdkCenteredViewPager pager = (SdkCenteredViewPager)container;
//            View view = GetView(position, pager);

//            pager.RemoveView(view);
//            pager.AddView(view);

//            return view;
//        }

//        /**
//         * Remove a page for the given position.
//         *
//         * @param container The containing View from which the page will be removed.
//         * @param position  The page position to be removed.
//         * @param view      The same object that was returned by instantiateItem(View, int).
//         */
//        public override void DestroyItem(ViewGroup container, int position, Object view)
//        {
//            ((SdkCenteredViewPager)container).RemoveView((View)view);
//        }
//    }
//}