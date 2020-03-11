///*
// * BASED ON THE ANDROID OPEN SOURCE PROJECT VIEWPAGER COMPONENT
// * 
// * Copyright (C) 2011 The Android Open Source Project
// * 
// * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
// * 
// * http://www.apache.org/licenses/LICENSE-2.0
// * 
// * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the
// * License.
// */

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Android.App;
//using Android.Content;
//using Android.Content.Res;
//using Android.Database;
//using Android.Graphics;
//using Android.Graphics.Drawables;
//using Android.OS;
//using Android.Runtime;
//using Android.Support.V4.View;
//using Android.Util;
//using Android.Views;
//using Android.Views.Accessibility;
//using Android.Widget;
//using Java.Lang;
//using Java.Lang.Reflect;
//using static Android.Support.V4.View.ViewPager;

//namespace AppLovinSdkSample.Droid.NativeAds.CarouselUI.Support
//{
//    /**
//     * This is a derivative of the AOSP ViewPager that has been customized to center-lock the pages and provide previews of the views before and after. It has additionally had its dependency on the support library removed; it therefore supports only Android 4.1 (Jellybean) and above.
//     */
//    public class SdkCenteredViewPager : ViewGroup
//    {
//        private static bool AL_CENTER_LOCK_ENABLED = true;
//        private float interPagePadding = 0f;

//        private static string TAG = "ViewPager";
//        private static bool DEBUG = false;

//        private static bool USE_CACHE = false;

//        private static int DEFAULT_OFFSCREEN_PAGES = 1;
//        private static int MAX_SETTLE_DURATION = 600;        // ms
//        private static int MIN_DISTANCE_FOR_FLING = 25;         // dips

//        private static int DEFAULT_GUTTER_SIZE = 16;         // dips

//        private static int MIN_FLING_VELOCITY = 400;        // dips

//        private static int[] LAYOUT_ATTRS = new int[] { Android.Resource.Attribute.layout_gravity };

//        /**
//         * Used to track what the expected number of items in the adapter should be. If the app changes this when we don't expect it, we'll throw a big obnoxious exception.
//         */
//        private int mExpectedAdapterCount;

//        class ItemInfo
//        {
//            public object @object;
//            public int position;
//            public bool scrolling;
//            public float widthFactor;
//            public float offset;
//        }

//        //    //private static Comparator<ItemInfo> COMPARATOR = new Comparator<ItemInfo>()
//        //    //{
//        //    //    @Override
//        //    //    public int compare(ItemInfo lhs, ItemInfo rhs)
//        //    //    {
//        //    //        return lhs.position - rhs.position;
//        //    //    }
//        //    //};

//        //private static Interpolator sInterpolator = new Interpolator()
//        //{
//        //    public float GetInterpolation(float t)
//        //    {
//        //        t -= 1.0f;
//        //        return t * t * t * t * t + 1.0f;
//        //    }
//        //};

//        private List<ItemInfo> mItems = new List<ItemInfo>();
//        private ItemInfo mTempItem = new ItemInfo();

//        private Rect mTempRect = new Rect();

//        private SdkPagerAdapter mAdapter;
//        private int mCurItem;                                            // Index of currently displayed page.
//        private int mRestoredCurItem = -1;
//        private IParcelable mRestoredAdapterState = null;
//        private ClassLoader mRestoredClassLoader = null;
//        private Scroller mScroller;
//        private PagerObserver mObserver;

//        private int mPageMargin;
//        private Drawable mMarginDrawable;
//        private int mTopPageBounds;
//        private int mBottomPageBounds;

//        // Offsets of the first and last items, if known.
//        // Set during population, used to determine if we are at the beginning
//        // or end of the pager data set during touch scrolling.
//        private float mFirstOffset = -float.MaxValue;
//        private float mLastOffset = float.MaxValue;

//        private int mChildWidthMeasureSpec;
//        private int mChildHeightMeasureSpec;
//        private bool mInLayout;

//        private bool mScrollingCacheEnabled;

//        private bool mPopulatePending;
//        private int mOffscreenPageLimit = DEFAULT_OFFSCREEN_PAGES;

//        private bool mIsBeingDragged;
//        private bool mIsUnableToDrag;
//        private int mDefaultGutterSize;
//        private int mGutterSize;
//        private int mTouchSlop;
//        /**
//         * Position of the last motion event.
//         */
//        private float mLastMotionX;
//        private float mLastMotionY;
//        private float mInitialMotionX;
//        private float mInitialMotionY;
//        /**
//         * ID of the active pointer. This is used to retain consistency during drags/flings if multiple pointers are used.
//         */
//        private int mActivePointerId = INVALID_POINTER;
//        /**
//         * Sentinel value for no current active pointer. Used by {@link #mActivePointerId}.
//         */
//        private static int INVALID_POINTER = -1;

//        /**
//         * Determines speed during touch scrolling
//         */
//        private VelocityTracker mVelocityTracker;
//        private int mMinimumVelocity;
//        private int mMaximumVelocity;
//        private int mFlingDistance;
//        private int mCloseEnough;

//        // If the pager is at least this close to its  position, complete the scroll
//        // on touch down and let the user interact with the content inside instead of
//        // "catching" the flinging pager.
//        private static int CLOSE_ENOUGH = 2;                           // dp

//        private bool mFakeDragging;
//        private long mFakeDragBeginTime;

//        private EdgeEffect mLeftEdge;
//        private EdgeEffect mRightEdge;

//        private bool mFirstLayout = true;
//        private bool mCalledSuper;
//        private int mDecorChildCount;

//        private IOnPageChangeListener mOnPageChangeListener;
//        private IOnPageChangeListener mInternalPageChangeListener;
//        private IOnAdapterChangeListener mAdapterChangeListener;
//        private IPageTransformer mPageTransformer;
//        private Method mSetChildrenDrawingOrderEnabled;

//        private static int DRAW_ORDER_DEFAULT = 0;
//        private static int DRAW_ORDER_FORWARD = 1;
//        private static int DRAW_ORDER_REVERSE = 2;
//        private int mDrawingOrder;
//        private List<View> mDrawingOrderedChildren;
//        //private ViewPositionComparator sPositionComparator = new ViewPositionComparator();

//        /**
//         * Indicates that the pager is in an idle, settled state. The current page is fully in view and no animation is in progress.
//         */
//        public const int SCROLL_STATE_IDLE = 0;

//        /**
//         * Indicates that the pager is currently being dragged by the user.
//         */
//        public const int SCROLL_STATE_DRAGGING = 1;

//        /**
//         * Indicates that the pager is in the process of settling to a  position.
//         */
//        public const int SCROLL_STATE_SETTLING = 2;

//        private Task mEndScrollRunnable = new Task(() =>
//        {
//            SetScrollState(SCROLL_STATE_IDLE);
//            Populate();
//        });

//        private int mScrollState = SCROLL_STATE_IDLE;

//        /**
//         * Callback interface for responding to changing state of the selected page.
//         */
//        public interface IOnPageChangeListener
//        {
//            /**
//             * This method will be invoked when the current page is scrolled, either as part of a programmatically initiated smooth scroll or a user initiated touch scroll.
//             *
//             * @param position             Position index of the first page currently being displayed. Page position+1 will be visible if positionOffset is nonzero.
//             * @param positionOffset       Value from [0, 1) indicating the offset from the page at position.
//             * @param positionOffsetPixels Value in pixels indicating the offset from position.
//             */
//            public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels);

//            /**
//             * This method will be invoked when a new page becomes selected. Animation is not necessarily complete.
//             *
//             * @param position Position index of the new selected page.
//             */
//            public void OnPageSelected(int position);

//            /**
//             * Called when the scroll state changes. Useful for discovering when the user begins dragging, when the pager is automatically settling to the current page, or when it is fully stopped/idle.
//             *
//             * @param state The new scroll state.
//             *
//             * @see ViewPager#SCROLL_STATE_IDLE
//             * @see ViewPager#SCROLL_STATE_DRAGGING
//             * @see ViewPager#SCROLL_STATE_SETTLING
//             */
//            public void OnPageScrollStateChanged(int state);
//        }

//        /**
//         * Simple implementation of the {@link OnPageChangeListener} interface with stub implementations of each method. Extend this if you do not intend to override every method of {@link OnPageChangeListener}.
//         */
//        public class SimpleOnPageChangeListener : IOnPageChangeListener
//        {
//            public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
//            {
//                // This space for rent
//            }

//            public void OnPageSelected(int position)
//            {
//                // This space for rent
//            }

//            public void OnPageScrollStateChanged(int state)
//            {
//                // This space for rent
//            }
//        }

//        /**
//            * A PageTransformer is invoked whenever a visible/attached page is scrolled. This offers an opportunity for the application to apply a custom transformation to the page views using animation properties.
//            * <p/>
//            * <p>
//            * As property animation is only supported as of Android 3.0 and forward, setting a PageTransformer on a ViewPager on earlier platform versions will be ignored.
//            * </p>
//            */
//        public interface IPageTransformer
//        {
//            /**
//                * Apply a property transformation to the given page.
//                *
//                * @param page     Apply the transformation to this page
//                * @param position Position of page relative to the current front-and-center position of the pager. 0 is front and center. 1 is one full page position to the right, and -1 is one page position to the left.
//                */
//            public void TransformPage(View page, float position);
//        }

//        /**
//            * Used internally to monitor when adapters are switched.
//            */
//        interface IOnAdapterChangeListener
//        {
//            public void OnAdapterChanged(SdkPagerAdapter oldAdapter, SdkPagerAdapter newAdapter);
//        }

//        /**
//            * Used internally to tag special types of child views that should be added as pager decorations by default.
//            */
//        interface IDecor
//        {
//        }

//        public SdkCenteredViewPager(Context context) : base(context)
//        {
//            InitViewPager();
//        }

//        public SdkCenteredViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
//        {
//            InitViewPager();
//        }

//        private void InitViewPager()
//        {
//            SetWillNotDraw(false);
//            DescendantFocusability = DescendantFocusability.AfterDescendants;
//            SetFocusable(ViewFocusability.Focusable);
//            mScroller = new Scroller(Context, sInterpolator);
//            ViewConfiguration configuration = ViewConfiguration.Get(Context);
//            float density = Context.Resources.DisplayMetrics.Density;

//            mTouchSlop = configuration.ScaledPagingTouchSlop;
//            mMinimumVelocity = (int)(MIN_FLING_VELOCITY * density);
//            mMaximumVelocity = configuration.ScaledMaximumFlingVelocity;
//            mLeftEdge = new EdgeEffect(Context);
//            mRightEdge = new EdgeEffect(Context);

//            mFlingDistance = (int)(MIN_DISTANCE_FOR_FLING * density);
//            mCloseEnough = (int)(CLOSE_ENOUGH * density);
//            mDefaultGutterSize = (int)(DEFAULT_GUTTER_SIZE * density);

//            SetAccessibilityDelegate(new MyAccessibilityDelegate());

//            if (ImportantForAccessibility == ImportantForAccessibilityAuto)
//            {
//                ImportantForAccessibility = ImportantForAccessibilityYes;
//            }
//        }

//        protected override void OnDetachedFromWindow()
//        {
//            RemoveCallbacks(mEndScrollRunnable);
//            base.OnDetachedFromWindow();
//        }

//        private void SetScrollState(int newState)
//        {
//            if (mScrollState == newState)
//            {
//                return;
//            }

//            mScrollState = newState;

//            if (mPageTransformer != null)
//            {
//                // PageTransformers can do complex things that benefit from hardware layers.
//                EnableLayers(newState != SCROLL_STATE_IDLE);
//            }
//            if (mOnPageChangeListener != null)
//            {
//                mOnPageChangeListener.OnPageScrollStateChanged(newState);
//            }
//        }

//        /**
//         * Set a SdkPagerAdapter that will supply views for this pager as needed.
//         *
//         * @param adapter Adapter to use
//         */
//        public void SetAdapter(SdkPagerAdapter adapter)
//        {
//            interPagePadding = (1 - adapter.GetPageWidth(0)) / 2; // Calculate even spacing between all pages

//            if (mAdapter != null)
//            {
//                mAdapter.UnregisterDataSetObserver(mObserver);
//                mAdapter.StartUpdate(this);

//                for (int i = 0; i < mItems.Count; i++)
//                {
//                    ItemInfo ii = mItems[i];
//                    mAdapter.DestroyItem(this, ii.position, ii.@object);
//                }

//                mAdapter.FinishUpdate(this);
//                mItems.Clear();
//                RemoveNonDecorViews();
//                mCurItem = 0;
//                ScrollTo(0, 0);
//            }

//            SdkPagerAdapter oldAdapter = mAdapter;
//            mAdapter = adapter;
//            mExpectedAdapterCount = 0;

//            if (mAdapter != null)
//            {
//                if (mObserver == null)
//                {
//                    mObserver = new PagerObserver();
//                }
//                mAdapter.RegisterDataSetObserver(mObserver);
//                mPopulatePending = false;
//                bool wasFirstLayout = mFirstLayout;
//                mFirstLayout = true;
//                mExpectedAdapterCount = mAdapter.GetCount();
//                if (mRestoredCurItem >= 0)
//                {
//                    mAdapter.RestoreState(mRestoredAdapterState, mRestoredClassLoader);
//                    SetCurrentItemInternal(mRestoredCurItem, false, true);
//                    mRestoredCurItem = -1;
//                    mRestoredAdapterState = null;
//                    mRestoredClassLoader = null;
//                }
//                else if (!wasFirstLayout)
//                {
//                    Populate();
//                }
//                else
//                {
//                    RequestLayout();
//                }
//            }

//            if (mAdapterChangeListener != null && oldAdapter != adapter)
//            {
//                mAdapterChangeListener.OnAdapterChanged(oldAdapter, adapter);
//            }
//        }

//        private void RemoveNonDecorViews()
//        {
//            for (int i = 0; i < ChildCount; i++)
//            {
//                View child = GetChildAt(i);
//                var lp = (ViewPager.LayoutParams)child.LayoutParameters;

//                if (!lp.IsDecor)
//                {
//                    RemoveViewAt(i);
//                    i--;
//                }
//            }
//        }

//        /**
//         * Retrieve the current adapter supplying pages.
//         *
//         * @return The currently registered SdkPagerAdapter
//         */
//        public SdkPagerAdapter GetAdapter()
//        {
//            return mAdapter;
//        }

//        void SetOnAdapterChangeListener(IOnAdapterChangeListener listener)
//        {
//            mAdapterChangeListener = listener;
//        }

//        private int GetClientWidth()
//        {
//            return MeasuredWidth - PaddingLeft - PaddingRight;
//        }

//        /**
//         * Set the currently selected page. If the ViewPager has already been through its first layout with its current adapter there will be a smooth animated transition between the current item and the specified item.
//         *
//         * @param item Item index to select
//         */
//        public void SetCurrentItem(int item)
//        {
//            mPopulatePending = false;
//            SetCurrentItemInternal(item, !mFirstLayout, false);
//        }

//        /**
//         * Set the currently selected page.
//         *
//         * @param item         Item index to select
//         * @param smoothScroll True to smoothly scroll to the new item, false to transition immediately
//         */
//        public void SetCurrentItem(int item, bool smoothScroll)
//        {
//            mPopulatePending = false;
//            SetCurrentItemInternal(item, smoothScroll, false);
//        }

//        public int GetCurrentItem()
//        {
//            return mCurItem;
//        }

//        void SetCurrentItemInternal(int item, bool smoothScroll, bool always)
//        {
//            SetCurrentItemInternal(item, smoothScroll, always, 0);
//        }

//        void SetCurrentItemInternal(int item, bool smoothScroll, bool always, int velocity)
//        {
//            if (mAdapter == null || mAdapter.GetCount() <= 0)
//            {
//                SetScrollingCacheEnabled(false);
//                return;
//            }
//            if (!always && mCurItem == item && mItems.Count != 0)
//            {
//                SetScrollingCacheEnabled(false);
//                return;
//            }

//            if (item < 0)
//            {
//                item = 0;
//            }
//            else if (item >= mAdapter.GetCount())
//            {
//                item = mAdapter.GetCount() - 1;
//            }

//            int pageLimit = mOffscreenPageLimit;

//            if (item > (mCurItem + pageLimit) || item < (mCurItem - pageLimit))
//            {
//                // We are doing a jump by more than one page. To avoid
//                // glitches, we want to keep all current pages in the view
//                // until the scroll ends.
//                for (int i = 0; i < mItems.Count; i++)
//                {
//                    mItems[i].scrolling = true;
//                }
//            }
//            bool dispatchSelected = mCurItem != item;

//            if (mFirstLayout)
//            {
//                // We don't have any idea how big we are yet and shouldn't have any pages either.
//                // Just set things up and let the pending layout handle things.
//                mCurItem = item;
//                if (dispatchSelected && mOnPageChangeListener != null)
//                {
//                    mOnPageChangeListener.OnPageSelected(item);
//                }
//                if (dispatchSelected && mInternalPageChangeListener != null)
//                {
//                    mInternalPageChangeListener.OnPageSelected(item);
//                }

//                RequestLayout();
//            }
//            else
//            {
//                Populate(item);
//                ScrollToItem(item, smoothScroll, velocity, dispatchSelected);
//            }
//        }

//        public void ScrollToItem(int item, bool smoothScroll, int velocity, bool dispatchSelected)
//        {
//            ItemInfo curInfo = InfoForPosition(item);
//            int destX = 0;

//            if (curInfo != null)
//            {
//                int width = GetClientWidth();

//                // Matt edit - Take the page margins into account
//                if (mLastOffset <= 0)
//                {
//                    mLastOffset = -(2 * interPagePadding);
//                }

//                destX = (int)(width * System.Math.Max(mFirstOffset, System.Math.Min(curInfo.offset - interPagePadding, mLastOffset))); // Adjust destX based on margins.

//                if (item == 0)
//                {
//                    // For the first view remove the initial left margin for scrolling.
//                    destX = (int)(width * System.Math.Min(curInfo.offset - interPagePadding, mLastOffset));
//                }
//                else if (item == mAdapter.GetCount() - 1)
//                {
//                    // Same but for last slide
//                    destX = (int)(width * (System.Math.Min(curInfo.offset, mLastOffset) + interPagePadding));
//                }
//                // End matt edit
//            }
//            if (smoothScroll)
//            {
//                SmoothScrollTo(destX, 0, velocity);

//                if (dispatchSelected && mOnPageChangeListener != null)
//                {
//                    mOnPageChangeListener.OnPageSelected(item);
//                }
//                if (dispatchSelected && mInternalPageChangeListener != null)
//                {
//                    mInternalPageChangeListener.OnPageSelected(item);
//                }
//            }
//            else
//            {
//                if (dispatchSelected && mOnPageChangeListener != null)
//                {
//                    mOnPageChangeListener.OnPageSelected(item);
//                }
//                if (dispatchSelected && mInternalPageChangeListener != null)
//                {
//                    mInternalPageChangeListener.OnPageSelected(item);
//                }

//                CompleteScroll(false);

//                if (AL_CENTER_LOCK_ENABLED)
//                {
//                    int _destX = destX;

//                    // If this is the first or last item, scroll to its position with page margin compensation
//                    if (item == 0 || item == mAdapter.GetCount() - 1)
//                    {
//                        Post(() =>
//                        {
//                            if (_destX == 0)
//                            {
//                                ScrollTo(_destX - (int)GetInterPagePadding(), 0);
//                            }
//                            else
//                            {
//                                ScrollTo(_destX, 0);
//                            }
//                        });
//                    }
//                    else
//                    {
//                        Post(() =>
//                        {
//                            if (_destX == 0)
//                            {
//                                ScrollTo(_destX - (int)GetInterPagePadding(), 0);
//                            }
//                            else
//                            {
//                                ScrollTo(_destX, 0);
//                            }
//                        });
//                    }
//                }
//                else
//                {
//                    // IF smooth scrolling is disabled then no need to compensate for that here.
//                    ScrollTo(destX, 0);
//                }

//                PageScrolled(destX);
//            }
//        }

//        /**
//        * Set a listener that will be invoked whenever the page changes or is incrementally scrolled. See {@link OnPageChangeListener}.
//        *
//        * @param listener Listener to set
//        */
//        public void SetOnPageChangeListener(IOnPageChangeListener listener)
//        {
//            mOnPageChangeListener = listener;
//        }

//        /**
//        * Set a {@link PageTransformer} that will be called for each attached page whenever the scroll position is changed. This allows the application to apply custom property transformations to each page, overriding the default sliding look and feel.
//        * <p/>
//        * <p>
//        * <em>Note:</em> Prior to Android 3.0 the property animation APIs did not exist. As a result, setting a PageTransformer prior to Android 3.0 (API 11) will have no effect.
//        * </p>
//        *
//        * @param reverseDrawingOrder true if the supplied PageTransformer requires page views to be drawn from last to first instead of first to last.
//        * @param transformer         PageTransformer that will modify each page's animation properties
//        */
//        public void SetPageTransformer(bool reverseDrawingOrder, IPageTransformer transformer)
//        {
//            if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
//            {
//                bool hasTransformer = transformer != null;
//                bool needsPopulate = hasTransformer == (mPageTransformer == null);
//                mPageTransformer = transformer;
//                SetChildrenDrawingOrderEnabledCompat(hasTransformer);

//                if (hasTransformer)
//                {
//                    mDrawingOrder = reverseDrawingOrder ? DRAW_ORDER_REVERSE : DRAW_ORDER_FORWARD;
//                }
//                else
//                {
//                    mDrawingOrder = DRAW_ORDER_DEFAULT;
//                }

//                if (needsPopulate)
//                {
//                    Populate();
//                }
//            }
//        }

//        void SetChildrenDrawingOrderEnabledCompat(bool enable)
//        {
//            if (Build.VERSION.SdkInt >= BuildVersionCodes.EclairMr1)
//            {
//                if (mSetChildrenDrawingOrderEnabled == null)
//                {
//                    try
//                    {
//                        mSetChildrenDrawingOrderEnabled = ViewGroup.GetDeclaredMethod("setChildrenDrawingOrderEnabled", new Class[] { Boolean.TYPE });
//                    }
//                    catch (NoSuchMethodException e)
//                    {
//                        Log.Error(TAG, "Can't find setChildrenDrawingOrderEnabled", e);
//                    }
//                }
//                try
//                {
//                    mSetChildrenDrawingOrderEnabled.Invoke(this, enable);
//                }
//                catch (System.Exception e)
//                {
//                    Log.Error(TAG, "Error changing children drawing order", e);
//                }
//            }
//        }

//        protected override int GetChildDrawingOrder(int childCount, int i)
//        {
//            int index = mDrawingOrder == DRAW_ORDER_REVERSE ? childCount - 1 - i : i;
//            int result = ((MyLayoutParams)mDrawingOrderedChildren[index].LayoutParameters).ChildIndex;
//            return result;
//        }

//        /**
//        * Set a separate OnPageChangeListener for internal use by the support library.
//        *
//        * @param listener Listener to set
//        *
//        * @return The old listener that was set, if any.
//        */
//        IOnPageChangeListener SetInternalPageChangeListener(IOnPageChangeListener listener)
//        {
//            IOnPageChangeListener oldListener = mInternalPageChangeListener;
//            mInternalPageChangeListener = listener;
//            return oldListener;
//        }

//        /**
//        * Returns the number of pages that will be retained to either side of the current page in the view hierarchy in an idle state. Defaults to 1.
//        *
//        * @return How many pages will be kept offscreen on either side
//        * @see #setOffscreenPageLimit(int)
//        */
//        public int GetOffscreenPageLimit()
//        {
//            return mOffscreenPageLimit;
//        }

//        /**
//        * Set the number of pages that should be retained to either side of the current page in the view hierarchy in an idle state. Pages beyond this limit will be recreated from the adapter when needed.
//        * <p/>
//        * <p>
//        * This is offered as an optimization. If you know in advance the number of pages you will need to support or have lazy-loading mechanisms in place on your pages, tweaking this setting can have benefits in perceived smoothness of paging animations and interaction. If you have a small number of
//        * pages (3-4) that you can keep active all at once, less time will be spent in layout for newly created view subtrees as the user pages back and forth.
//        * </p>
//        * <p/>
//        * <p>
//        * You should keep this limit low, especially if your pages have complex layouts. This setting defaults to 1.
//        * </p>
//        *
//        * @param limit How many pages will be kept offscreen in an idle state.
//        */
//        public void SetOffscreenPageLimit(int limit)
//        {
//            if (limit < DEFAULT_OFFSCREEN_PAGES)
//            {
//                Log.Warn(TAG, "Requested offscreen page limit " + limit + " too small; defaulting to " + DEFAULT_OFFSCREEN_PAGES);
//                limit = DEFAULT_OFFSCREEN_PAGES;
//            }

//            if (limit != mOffscreenPageLimit)
//            {
//                mOffscreenPageLimit = limit;
//                Populate();
//            }
//        }

//        /**
//        * Set the margin between pages.
//        *
//        * @param marginPixels Distance between adjacent pages in pixels
//        *
//        * @see #getPageMargin()
//        * @see #setPageMarginDrawable(Drawable)
//        * @see #setPageMarginDrawable(int)
//        */
//        public void SetPageMargin(int marginPixels)
//        {
//            int oldMargin = mPageMargin;
//            mPageMargin = marginPixels;

//            RecomputeScrollPosition(Width, Width, marginPixels, oldMargin);

//            RequestLayout();
//        }

//        /**
//        * Return the margin between pages.
//        *
//        * @return The size of the margin in pixels
//        */
//        public int GetPageMargin()
//        {
//            return mPageMargin;
//        }

//        /**
//        * Set a drawable that will be used to fill the margin between pages.
//        *
//        * @param d Drawable to display between pages
//        */
//        public void SetPageMarginDrawable(Drawable d)
//        {
//            mMarginDrawable = d;
//            if (d != null)
//            {
//                RefreshDrawableState();
//            }

//            SetWillNotDraw(d == null);
//            Invalidate();
//        }

//        /**
//        * Set a drawable that will be used to fill the margin between pages.
//        *
//        * @param resId Resource ID of a drawable to display between pages
//        */
//        public void SetPageMarginDrawable(int resId)
//        {
//            SetPageMarginDrawable(Context.Resources.GetDrawable(resId));
//        }

//        protected override bool VerifyDrawable(Drawable who)
//        {
//            return base.VerifyDrawable(who) || who == mMarginDrawable;
//        }

//        protected override void DrawableStateChanged()
//        {
//            base.DrawableStateChanged();
//            Drawable d = mMarginDrawable;

//            if (d != null && d.IsStateful)
//            {
//                d.SetState(GetDrawableState());
//            }
//        }

//        // We want the duration of the page snap animation to be influenced by the distance that
//        // the screen has to travel, however, we don't want this duration to be effected in a
//        // purely linear fashion. Instead, we use this method to moderate the effect that the distance
//        // of travel has on the overall snap duration.
//        float DistanceInfluenceForSnapDuration(float f)
//        {
//            f -= 0.5f; // center the values about 0.
//            f *= 0.3f * (float)System.Math.PI / 2.0f;

//            return (float)System.Math.Sin(f);
//        }

//        /**
//        * Like {@link View#scrollBy}, but scroll smoothly instead of immediately.
//        *
//        * @param x the number of pixels to scroll by on the X axis
//        * @param y the number of pixels to scroll by on the Y axis
//        */
//        void SmoothScrollTo(int x, int y)
//        {
//            SmoothScrollTo(x, y, 0);
//        }

//        /**
//        * Like {@link View#scrollBy}, but scroll smoothly instead of immediately.
//        *
//        * @param x        the number of pixels to scroll by on the X axis
//        * @param y        the number of pixels to scroll by on the Y axis
//        * @param velocity the velocity associated with a fling, if applicable. (0 otherwise)
//        */
//        void SmoothScrollTo(int x, int y, int velocity)
//        {
//            if (ChildCount == 0)
//            {
//                // Nothing to do.
//                SetScrollingCacheEnabled(false);
//                return;
//            }

//            int sx = ScrollX;
//            int sy = ScrollY;
//            int dx = x - sx;
//            int dy = y - sy;

//            if (dx == 0 && dy == 0)
//            {
//                CompleteScroll(false);
//                Populate();
//                SetScrollState(SCROLL_STATE_IDLE);
//                return;
//            }

//            SetScrollingCacheEnabled(true);
//            SetScrollState(SCROLL_STATE_SETTLING);

//            int width = GetClientWidth();
//            int halfWidth = width / 2;
//            float distanceRatio = System.Math.Min(1f, 1.0f * System.Math.Abs(dx) / width);
//            float distance = halfWidth + halfWidth * DistanceInfluenceForSnapDuration(distanceRatio);

//            int duration = 0;
//            velocity = System.Math.Abs(velocity);

//            if (velocity > 0)
//            {
//                duration = 4 * (int)System.Math.Round(1000 * System.Math.Abs(distance / velocity));
//            }
//            else
//            {
//                float pageWidth = width * mAdapter.GetPageWidth(mCurItem);
//                float pageDelta = (float)System.Math.Abs(dx) / (pageWidth + mPageMargin);
//                duration = (int)((pageDelta + 1) * 100);
//            }
//            duration = System.Math.Min(duration, MAX_SETTLE_DURATION);

//            mScroller.StartScroll(sx, sy, dx, dy, duration);
//            PostInvalidateOnAnimation();
//        }

//        ItemInfo AddNewItem(int position, int index)
//        {
//            ItemInfo ii = new ItemInfo
//            {
//                position = position,
//                @object = mAdapter.InstantiateItem(this, position),
//                widthFactor = mAdapter.GetPageWidth(position)
//            };

//            if (index < 0 || index >= mItems.Count)
//            {
//                mItems.Add(ii);
//            }
//            else
//            {
//                mItems.Insert(index, ii);
//            }
//            return ii;
//        }

//        void DataSetChanged()
//        {
//            // This method only gets called if our observer is attached, so mAdapter is non-null.

//            int adapterCount = mAdapter.GetCount();
//            mExpectedAdapterCount = adapterCount;
//            bool needPopulate = mItems.Count < mOffscreenPageLimit * 2 + 1 && mItems.Count < adapterCount;
//            int newCurrItem = mCurItem;

//            bool isUpdating = false;
//            for (int i = 0; i < mItems.Count; i++)
//            {
//                ItemInfo ii = mItems[i];
//                int newPos = mAdapter.GetItemPosition(ii.@object);

//                if (newPos == SdkPagerAdapter.POSITION_UNCHANGED)
//                {
//                    continue;
//                }

//                if (newPos == SdkPagerAdapter.POSITION_NONE)
//                {
//                    mItems.RemoveAt(i);
//                    i--;

//                    if (!isUpdating)
//                    {
//                        mAdapter.StartUpdate(this);
//                        isUpdating = true;
//                    }

//                    mAdapter.DestroyItem(this, ii.position, ii.@object);
//                    needPopulate = true;

//                    if (mCurItem == ii.position)
//                    {
//                        // Keep the current item in the valid range
//                        newCurrItem = System.Math.Max(0, System.Math.Min(mCurItem, adapterCount - 1));
//                        needPopulate = true;
//                    }
//                    continue;
//                }

//                if (ii.position != newPos)
//                {
//                    if (ii.position == mCurItem)
//                    {
//                        // Our current item changed position. Follow it.
//                        newCurrItem = newPos;
//                    }

//                    ii.position = newPos;
//                    needPopulate = true;
//                }
//            }

//            if (isUpdating)
//            {
//                mAdapter.FinishUpdate(this);
//            }

//            Collections.sort(mItems, COMPARATOR);

//            if (needPopulate)
//            {
//                // Reset our known page widths; populate will recompute them.
//                int childCount = ChildCount;
//                for (int i = 0; i < childCount; i++)
//                {
//                    View child = GetChildAt(i);
//                    var lp = (MyLayoutParams)child.LayoutParameters;
//                    if (!lp.IsDecor)
//                    {
//                        lp.WidthFactor = 0.0f;
//                    }
//                }

//                SetCurrentItemInternal(newCurrItem, false, true);
//                RequestLayout();
//            }
//        }

//        void Populate()
//        {
//            Populate(mCurItem);
//        }

//        void Populate(int newCurrentItem)
//        {
//            ItemInfo oldCurInfo = null;
//            FocusSearchDirection focusDirection = FocusSearchDirection.Forward;
//            if (mCurItem != newCurrentItem)
//            {
//                focusDirection = mCurItem < newCurrentItem ? FocusSearchDirection.Right : FocusSearchDirection.Left;
//                oldCurInfo = InfoForPosition(mCurItem);
//                mCurItem = newCurrentItem;
//            }

//            if (mAdapter == null)
//            {
//                SortChildDrawingOrder();
//                return;
//            }

//            // Bail now if we are waiting to populate. This is to hold off
//            // on creating views from the time the user releases their finger to
//            // fling to a new position until we have finished the scroll to
//            // that position, avoiding glitches from happening at that point.
//            if (mPopulatePending)
//            {
//                if (DEBUG) Log.Info(TAG, "populate is pending, skipping for now...");
//                SortChildDrawingOrder();
//                return;
//            }

//            // Also, don't populate until we are attached to a window. This is to
//            // avoid trying to populate before we have restored our view hierarchy
//            // state and conflicting with what is restored.
//            if (WindowToken == null)
//            {
//                return;
//            }

//            mAdapter.StartUpdate(this);

//            int pageLimit = mOffscreenPageLimit;
//            int startPos = System.Math.Max(0, mCurItem - pageLimit);
//            int N = mAdapter.GetCount();
//            int endPos = System.Math.Min(N - 1, mCurItem + pageLimit);

//            if (N != mExpectedAdapterCount)
//            {
//                string resName;

//                try
//                {
//                    resName = Resources.GetResourceName(Id);
//                }
//                catch (Resources.NotFoundException e)
//                {
//                    resName = Integer.ToHexString(Id);
//                }

//                throw new IllegalStateException("The application's SdkPagerAdapter changed the adapter's" +
//                    " contents without calling SdkPagerAdapter#notifyDataSetChanged!" +
//                    " Expected adapter item count: " + mExpectedAdapterCount + ", found: " + N +
//                    " Pager id: " + resName +
//                    " Pager class: " + GetType() +
//                    " Problematic adapter: " + mAdapter.GetType());
//            }

//            // Locate the currently focused item or add it if needed.
//            int curIndex = -1;
//            ItemInfo curItem = null;

//            for (curIndex = 0; curIndex < mItems.Count; curIndex++)
//            {
//                ItemInfo ii = mItems[curIndex];

//                if (ii.position >= mCurItem)
//                {
//                    if (ii.position == mCurItem)
//                    {
//                        curItem = ii;
//                    }

//                    break;
//                }
//            }

//            if (curItem == null && N > 0)
//            {
//                curItem = AddNewItem(mCurItem, curIndex);
//            }

//            // Fill 3x the available width or up to the number of offscreen
//            // pages requested to either side, whichever is larger.
//            // If we have no current item we have no work to do.
//            if (curItem != null)
//            {
//                float extraWidthLeft = 0.0f;
//                int itemIndex = curIndex - 1;
//                ItemInfo ii = itemIndex >= 0 ? mItems[itemIndex] : null;
//                int clientWidth = GetClientWidth();
//                float leftWidthNeeded = clientWidth <= 0 ? 0 : 2.0f - curItem.widthFactor + (float)PaddingLeft / (float)clientWidth;

//                for (int pos = mCurItem - 1; pos >= 0; pos--)
//                {
//                    if (extraWidthLeft >= leftWidthNeeded && pos < startPos)
//                    {
//                        if (ii == null)
//                        {
//                            break;
//                        }

//                        if (pos == ii.position && !ii.scrolling)
//                        {
//                            mItems.RemoveAt(itemIndex);
//                            mAdapter.DestroyItem(this, pos, ii.@object);

//                            if (DEBUG)
//                            {
//                                Log.Info(TAG, "populate() - destroyItem() with pos: " + pos + " view: " + ((View)ii.@object));
//                            }

//                            itemIndex--;
//                            curIndex--;
//                            ii = itemIndex >= 0 ? mItems[itemIndex] : null;
//                        }
//                    }
//                    else if (ii != null && pos == ii.position)
//                    {
//                        extraWidthLeft += ii.widthFactor;
//                        itemIndex--;
//                        ii = itemIndex >= 0 ? mItems[itemIndex] : null;
//                    }
//                    else
//                    {
//                        ii = AddNewItem(pos, itemIndex + 1);
//                        extraWidthLeft += ii.widthFactor;
//                        curIndex++;
//                        ii = itemIndex >= 0 ? mItems[itemIndex] : null;
//                    }
//                }

//                float extraWidthRight = curItem.widthFactor;
//                itemIndex = curIndex + 1;

//                if (extraWidthRight < 2.0f)
//                {
//                    ii = itemIndex < mItems.Count ? mItems[itemIndex] : null;
//                    float rightWidthNeeded = clientWidth <= 0 ? 0 : (float)PaddingRight / (float)clientWidth + 2.0f;

//                    for (int pos = mCurItem + 1; pos < N; pos++)
//                    {
//                        if (extraWidthRight >= rightWidthNeeded && pos > endPos)
//                        {
//                            if (ii == null)
//                            {
//                                break;
//                            }

//                            if (pos == ii.position && !ii.scrolling)
//                            {
//                                mItems.RemoveAt(itemIndex);
//                                mAdapter.DestroyItem(this, pos, ii.@object);

//                                if (DEBUG)
//                                {
//                                    Log.Info(TAG, "populate() - destroyItem() with pos: " + pos + " view: " + ((View)ii.@object));
//                                }

//                                ii = itemIndex < mItems.Count ? mItems[itemIndex] : null;
//                            }
//                        }
//                        else if (ii != null && pos == ii.position)
//                        {
//                            extraWidthRight += ii.widthFactor;
//                            itemIndex++;
//                            ii = itemIndex < mItems.Count ? mItems[itemIndex] : null;
//                        }
//                        else
//                        {
//                            ii = AddNewItem(pos, itemIndex);
//                            itemIndex++;
//                            extraWidthRight += ii.widthFactor;
//                            ii = itemIndex < mItems.Count ? mItems[itemIndex] : null;
//                        }
//                    }
//                }

//                CalculatePageOffsets(curItem, curIndex, oldCurInfo);
//            }

//            if (DEBUG)
//            {
//                Log.Info(TAG, "Current page list:");

//                for (int i = 0; i < mItems.Count; i++)
//                {
//                    Log.Info(TAG, "#" + i + ": page " + mItems[i].position);
//                }
//            }

//            mAdapter.SetPrimaryItem(this, mCurItem, curItem != null ? curItem.@object : null);

//            mAdapter.FinishUpdate(this);

//            // Check width measurement of current pages and drawing sort order.
//            // Update LayoutParams as needed.
//            int childCount = ChildCount;

//            for (int i = 0; i < childCount; i++)
//            {
//                View child = GetChildAt(i);
//                var lp = (MyLayoutParams)child.LayoutParameters;
//                lp.ChildIndex = i;

//                if (!lp.IsDecor && lp.WidthFactor == 0.0f)
//                {
//                    // 0 means requery the adapter for this, it doesn't have a valid width.
//                    ItemInfo ii = InfoForChild(child);

//                    if (ii != null)
//                    {
//                        lp.WidthFactor = ii.widthFactor;
//                        lp.Position = ii.position;
//                    }
//                }
//            }

//            SortChildDrawingOrder();

//            if (HasFocus)
//            {
//                View currentFocused = FindFocus();
//                ItemInfo ii = currentFocused != null ? InfoForAnyChild(currentFocused) : null;

//                if (ii == null || ii.position != mCurItem)
//                {
//                    for (int i = 0; i < ChildCount; i++)
//                    {
//                        View child = GetChildAt(i);
//                        ii = InfoForChild(child);

//                        if (ii != null && ii.position == mCurItem)
//                        {
//                            if (child.RequestFocus(focusDirection))
//                            {
//                                break;
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        private void SortChildDrawingOrder()
//        {
//            if (mDrawingOrder != DRAW_ORDER_DEFAULT)
//            {
//                if (mDrawingOrderedChildren == null)
//                {
//                    mDrawingOrderedChildren = new List<View>();
//                }
//                else
//                {
//                    mDrawingOrderedChildren.Clear();
//                }

//                int childCount = ChildCount;

//                for (int i = 0; i < childCount; i++)
//                {
//                    View child = GetChildAt(i);
//                    mDrawingOrderedChildren.Add(child);
//                }

//                Collections.sort(mDrawingOrderedChildren, sPositionComparator);
//            }
//        }

//        private void CalculatePageOffsets(ItemInfo curItem, int curIndex, ItemInfo oldCurInfo)
//        {
//            int N = mAdapter.GetCount();
//            int width = GetClientWidth();
//            float marginOffset = width > 0 ? (float)mPageMargin / width : 0;

//            // Fix up offsets for later layout.
//            if (oldCurInfo != null)
//            {
//                int oldCurPosition = oldCurInfo.position;

//                // Base offsets off of oldCurInfo.
//                if (oldCurPosition < curItem.position)
//                {
//                    int itemIndex = 0;
//                    ItemInfo ii = null;
//                    float _offset = oldCurInfo.offset + oldCurInfo.widthFactor + marginOffset;

//                    for (int _pos = oldCurPosition + 1; _pos <= curItem.position && itemIndex < mItems.Count; _pos++)
//                    {
//                        ii = mItems[itemIndex];

//                        while (_pos > ii.position && itemIndex < mItems.Count - 1)
//                        {
//                            itemIndex++;
//                            ii = mItems[itemIndex];
//                        }

//                        while (_pos < ii.position)
//                        {
//                            // We don't have an item populated for this,
//                            // ask the adapter for an offset.
//                            _offset += mAdapter.GetPageWidth(_pos) + marginOffset;
//                            _pos++;
//                        }

//                        ii.offset = _offset;
//                        _offset += ii.widthFactor + marginOffset;
//                    }
//                }
//                else if (oldCurPosition > curItem.position)
//                {
//                    int itemIndex = mItems.Count - 1;
//                    ItemInfo ii = null;
//                    float _offset = oldCurInfo.offset;

//                    for (int _pos = oldCurPosition - 1; _pos >= curItem.position && itemIndex >= 0; _pos--)
//                    {
//                        ii = mItems[itemIndex];

//                        while (_pos < ii.position && itemIndex > 0)
//                        {
//                            itemIndex--;
//                            ii = mItems[itemIndex];
//                        }

//                        while (_pos > ii.position)
//                        {
//                            // We don't have an item populated for this,
//                            // ask the adapter for an offset.
//                            _offset -= mAdapter.GetPageWidth(_pos) + marginOffset;
//                            _pos--;
//                        }

//                        _offset -= ii.widthFactor + marginOffset;
//                        ii.offset = _offset;
//                    }
//                }
//            }

//            // Base all offsets off of curItem.
//            int itemCount = mItems.Count;
//            float offset = curItem.offset;
//            int pos = curItem.position - 1;
//            mFirstOffset = curItem.position == 0 ? curItem.offset : -float.MaxValue;
//            mLastOffset = curItem.position == N - 1 ? curItem.offset + curItem.widthFactor - 1 : float.MaxValue;

//            // Previous pages
//            for (int i = curIndex - 1; i >= 0; i--, pos--)
//            {
//                ItemInfo ii = mItems[i];

//                while (pos > ii.position)
//                {
//                    offset -= mAdapter.GetPageWidth(pos--) + marginOffset;
//                }

//                offset -= ii.widthFactor + marginOffset;
//                ii.offset = offset;

//                if (ii.position == 0)
//                {
//                    mFirstOffset = offset;
//                }
//            }

//            offset = curItem.offset + curItem.widthFactor + marginOffset;
//            pos = curItem.position + 1;

//            // Next pages
//            for (int i = curIndex + 1; i < itemCount; i++, pos++)
//            {
//                ItemInfo ii = mItems[i];

//                while (pos < ii.position)
//                {
//                    offset += mAdapter.GetPageWidth(pos++) + marginOffset;
//                }

//                if (ii.position == N - 1)
//                {
//                    mLastOffset = offset + ii.widthFactor - 1;
//                }

//                ii.offset = offset;
//                offset += ii.widthFactor + marginOffset;
//            }
//        }

//        /**
//            * This is the persistent state that is saved by ViewPager. Only needed if you are creating a sublass of ViewPager that must save its own state, in which case it should implement a subclass of this which contains that state.
//            */
//        public class SavedState : BaseSavedState
//        {
//            public int position;
//            public IParcelable adapterState;
//            public ClassLoader loader;

//            public SavedState(IParcelable superState) : base(superState)
//            {
//            }

//            public override void WriteToParcel(Parcel @out, ParcelableWriteFlags flags)
//            {
//                base.WriteToParcel(@out, flags);
//                @out.WriteInt(position);
//                @out.WriteParcelable(adapterState, flags);
//            }

//            public override string ToString()
//            {
//                return "FragmentPager.SavedState{"
//                    + Integer.ToHexString(JavaSystem.IdentityHashCode(this))
//                    + " position=" + position + "}";
//            }

//            //public Creator<SavedState> CREATOR = new Creator<SavedState>()
//            //{
//            //    @Override
//            //    public SavedState createFromParcel(Parcel source)
//            //    {
//            //        return new SavedState(source, ClassLoader.getSystemClassLoader());
//            //    }

//            //    @Override
//            //        public SavedState[] newArray(int size)
//            //    {
//            //        return new SavedState[size];
//            //    }
//            //};

//            public SavedState(Parcel @in, ClassLoader loader) : base(@in)
//            {
//                if (loader == null)
//                {
//                    loader = Class.ClassLoader;
//                }

//                position = @in.ReadInt();
//                adapterState = (IParcelable)@in.ReadParcelable(loader);
//                this.loader = loader;
//            }
//        }

//        protected override IParcelable OnSaveInstanceState()
//        {
//            IParcelable superState = base.OnSaveInstanceState();
//            SavedState ss = new SavedState(superState);
//            ss.position = mCurItem;
//            if (mAdapter != null)
//            {
//                ss.adapterState = mAdapter.SaveState();
//            }
//            return ss;
//        }

//        protected override void OnRestoreInstanceState(IParcelable state)
//        {
//            if (!(state is SavedState ) )
//            {
//                base.OnRestoreInstanceState(state);
//                return;
//            }

//            SavedState ss = (SavedState)state;
//            base.OnRestoreInstanceState(ss.SuperState);

//            if (mAdapter != null)
//            {
//                mAdapter.RestoreState(ss.adapterState, ss.loader);
//                SetCurrentItemInternal(ss.position, false, true);
//            }
//            else
//            {
//                mRestoredCurItem = ss.position;
//                mRestoredAdapterState = ss.adapterState;
//                mRestoredClassLoader = ss.loader;
//            }
//        }

//        public override void AddView(View child, int index, ViewGroup.LayoutParams @params)
//        {
//            if (!CheckLayoutParams(@params))
//            {
//                @params = GenerateLayoutParams(@params);
//            }
//            var lp = (MyLayoutParams)@params;
//            lp.IsDecor |= child is IDecor;
//            if (mInLayout)
//            {
//                if (lp != null && lp.IsDecor)
//                {
//                    throw new IllegalStateException("Cannot add pager decor view during layout");
//                }
//                lp.NeedsMeasure = true;
//                AddViewInLayout(child, index, @params);
//            }
//            else
//            {
//                base.AddView(child, index, @params);
//            }

//            if (USE_CACHE)
//            {
//                if (child.Visibility != ViewStates.Gone)
//                {
//                    child.DrawingCacheEnabled = mScrollingCacheEnabled;
//                }
//                else
//                {
//                    child.DrawingCacheEnabled = false;
//                }
//            }
//        }

//        public override void RemoveView(View view)
//        {
//            if (mInLayout)
//            {
//                RemoveViewInLayout(view);
//            }
//            else
//            {
//                base.RemoveView(view);
//            }
//        }

//        ItemInfo InfoForChild(View child)
//        {
//            for (int i = 0; i < mItems.Count; i++)
//            {
//                ItemInfo ii = mItems[i];

//                if (mAdapter.IsViewFromObject(child, ii.@object))
//                {
//                    return ii;
//                }
//            }

//            return null;
//        }

//        ItemInfo InfoForAnyChild(View child)
//        {
//            IViewParent parent;

//            while ((parent = child.Parent) != this)
//            {
//                if (parent == null || !(parent is View))
//                {
//                    return null;
//                }

//                child = (View)parent;
//            }

//            return InfoForChild(child);
//        }

//        ItemInfo InfoForPosition(int position)
//        {
//            for (int i = 0; i < mItems.Count; i++)
//            {
//                ItemInfo ii = mItems[i];

//                if (ii.position == position)
//                {
//                    return ii;
//                }
//            }

//            return null;
//        }

//        protected override void OnAttachedToWindow()
//        {
//            base.OnAttachedToWindow();
//            mFirstLayout = true;
//        }

//        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
//        {
//            // For simple implementation, our internal size is always 0.
//            // We depend on the container to specify the layout size of
//            // our view. We can't really know what it is since we will be
//            // adding and removing different arbitrary views and do not
//            // want the layout to change as this happens.
//            SetMeasuredDimension(GetDefaultSize(0, widthMeasureSpec), GetDefaultSize(0, heightMeasureSpec));

//            int measuredWidth = MeasuredWidth;
//            int maxGutterSize = measuredWidth / 10;
//            mGutterSize = System.Math.Min(maxGutterSize, mDefaultGutterSize);

//            // Children are just made to fill our space.
//            int childWidthSize = measuredWidth - PaddingLeft - PaddingRight;
//            int childHeightSize = MeasuredHeight - PaddingTop - PaddingBottom;

//            /*
//             * Make sure all children have been properly measured. Decor views first. Right now we cheat and make this less complicated by assuming decor views won't intersect. We will pin to edges based on gravity.
//             */
//            int size = ChildCount;
//            for (int i = 0; i < size; ++i)
//            {
//                View child = GetChildAt(i);
//                if (child.Visibility != ViewStates.Gone)
//                {
//                    var lp = (MyLayoutParams)child.LayoutParameters;
//                    if (lp != null && lp.IsDecor)
//                    {
//                        GravityFlags hgrav = lp.Gravity & GravityFlags.HorizontalGravityMask;
//                        GravityFlags vgrav = lp.Gravity & GravityFlags.VerticalGravityMask;
//                        MeasureSpecMode widthMode = MeasureSpecMode.AtMost;
//                        MeasureSpecMode heightMode = MeasureSpecMode.AtMost;
//                        bool consumeVertical = vgrav == GravityFlags.Top || vgrav == GravityFlags.Bottom;
//                        bool consumeHorizontal = hgrav == GravityFlags.Left|| hgrav == GravityFlags.Right;

//                        if (consumeVertical)
//                        {
//                            widthMode = MeasureSpecMode.Exactly;
//                        }
//                        else if (consumeHorizontal)
//                        {
//                            heightMode = MeasureSpecMode.Exactly;
//                        }

//                        int widthSize = childWidthSize;
//                        int heightSize = childHeightSize;
//                        if (lp.Width != LayoutParams.WrapContent)
//                        {
//                            widthMode = MeasureSpecMode.Exactly;
//                            if (lp.Width != LayoutParams.FillParent)
//                            {
//                                widthSize = lp.Width;
//                            }
//                        }
//                        if (lp.Height != LayoutParams.WrapContent)
//                        {
//                            heightMode = MeasureSpecMode.Exactly;
//                            if (lp.Height != LayoutParams.FillParent)
//                            {
//                                heightSize = lp.Height;
//                            }
//                        }
//                        int widthSpec = MeasureSpec.MakeMeasureSpec(widthSize, widthMode);
//                        int heightSpec = MeasureSpec.MakeMeasureSpec(heightSize, heightMode);
//                        child.Measure(widthSpec, heightSpec);

//                        if (consumeVertical)
//                        {
//                            childHeightSize -= child.MeasuredHeight;
//                        }
//                        else if (consumeHorizontal)
//                        {
//                            childWidthSize -= child.MeasuredWidth;
//                        }
//                    }
//                }
//            }

//            mChildWidthMeasureSpec = MeasureSpec.MakeMeasureSpec(childWidthSize, MeasureSpecMode.Exactly);
//            mChildHeightMeasureSpec = MeasureSpec.MakeMeasureSpec(childHeightSize, MeasureSpecMode.Exactly);

//            // Make sure we have created all fragments that we need to have shown.
//            mInLayout = true;
//            Populate();
//            mInLayout = false;

//            // Page views next.
//            size = ChildCount;
//            for (int i = 0; i < size; ++i)
//            {
//                View child = GetChildAt(i);
//                if (child.Visibility != ViewStates.Gone)
//                {
//                    if (DEBUG) Log.Verbose(TAG, "Measuring #" + i + " " + child + ": " + mChildWidthMeasureSpec);

//                    var lp = (MyLayoutParams)child.LayoutParameters;
//                    if (lp == null || !lp.IsDecor)
//                    {
//                        int widthSpec = MeasureSpec.MakeMeasureSpec((int)(childWidthSize * lp.WidthFactor), MeasureSpecMode.Exactly);
//                        child.Measure(widthSpec, mChildHeightMeasureSpec);
//                    }
//                }
//            }
//        }

//        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
//        {
//            base.OnSizeChanged(w, h, oldw, oldh);

//            // Make sure scroll position is set correctly.
//            if (w != oldw)
//            {
//                RecomputeScrollPosition(w, oldw, mPageMargin, mPageMargin);
//            }
//        }

//        private void RecomputeScrollPosition(int width, int oldWidth, int margin, int oldMargin)
//        {
//            if (oldWidth > 0 && !mItems.Any())
//            {
//                int widthWithMargin = width - PaddingLeft - PaddingRight + margin;
//                int oldWidthWithMargin = oldWidth - PaddingLeft - PaddingRight
//                       + oldMargin;
//                int xpos = ScrollX;
//                float pageOffset = (float)xpos / oldWidthWithMargin;
//                int newOffsetPixels = (int)(pageOffset * widthWithMargin);

//                ScrollTo(newOffsetPixels, ScrollY);
//                if (!mScroller.IsFinished)
//                {
//                    // We now return to your regularly scheduled scroll, already in progress.
//                    int newDuration = mScroller.Duration - mScroller.TimePassed();
//                    ItemInfo targetInfo = InfoForPosition(mCurItem);
//                    mScroller.StartScroll(newOffsetPixels, 0,
//                                           (int)(targetInfo.offset * width), 0, newDuration);
//                }
//            }
//            else
//            {
//                ItemInfo ii = InfoForPosition(mCurItem);
//                float scrollOffset = ii != null ? System.Math.Min(ii.offset, mLastOffset) : 0;
//                int scrollPos = (int)(scrollOffset *
//                       (width - PaddingLeft - PaddingRight));
//                if (scrollPos != ScrollX)
//                {
//                    CompleteScroll(false);
//                    ScrollTo(scrollPos, ScrollY);
//                }
//            }
//        }

//        protected override void OnLayout(bool changed, int l, int t, int r, int b)
//        {
//            int count = ChildCount;
//            int width = r - l;
//            int height = b - t;
//            int paddingLeft = PaddingLeft;
//            int paddingTop = PaddingTop;
//            int paddingRight = PaddingRight;
//            int paddingBottom = PaddingBottom;
//            int scrollX = ScrollX;

//            int decorCount = 0;

//            // First pass - decor views. We need to do this in two passes so that
//            // we have the proper offsets for non-decor views later.
//            for (int i = 0; i < count; i++)
//            {
//                View child = GetChildAt(i);

//                if (child.Visibility != ViewStates.Gone)
//                {
//                    var lp = (MyLayoutParams)child.LayoutParameters;
//                    int childLeft = 0;
//                    int childTop = 0;

//                    if (lp.IsDecor)
//                    {
//                        GravityFlags hgrav = (GravityFlags)lp.Gravity & GravityFlags.HorizontalGravityMask;
//                        GravityFlags vgrav = (GravityFlags)lp.Gravity & GravityFlags.VerticalGravityMask;

//                        switch (hgrav)
//                        {
//                            default:
//                                childLeft = paddingLeft;
//                                break;
//                            case GravityFlags.Left:
//                                childLeft = paddingLeft;
//                                paddingLeft += child.MeasuredWidth;
//                                break;
//                            case GravityFlags.CenterHorizontal:
//                                childLeft = System.Math.Max((width - child.MeasuredWidth) / 2,
//                                                      paddingLeft);
//                                break;
//                            case GravityFlags.Right:
//                                childLeft = width - paddingRight - child.MeasuredWidth;
//                                paddingRight += child.MeasuredWidth;
//                                break;
//                        }

//                        switch (vgrav)
//                        {
//                            default:
//                                childTop = paddingTop;
//                                break;
//                            case GravityFlags.Top:
//                                childTop = paddingTop;
//                                paddingTop += child.MeasuredHeight;
//                                break;
//                            case GravityFlags.CenterVertical:
//                                childTop = System.Math.Max((height - child.MeasuredHeight) / 2,
//                                                     paddingTop);
//                                break;
//                            case GravityFlags.Bottom:
//                                childTop = height - paddingBottom - child.MeasuredHeight;
//                                paddingBottom += child.MeasuredHeight;
//                                break;
//                        }

//                        childLeft += scrollX;
//                        child.Layout(childLeft, childTop, childLeft + child.MeasuredWidth, childTop + child.MeasuredHeight);
//                        decorCount++;
//                    }
//                }
//            }

//            int childWidth = width - paddingLeft - paddingRight;

//            // Page views. Do this once we have the right padding offsets from above.
//            for (int i = 0; i < count; i++)
//            {
//                View child = GetChildAt(i);

//                if (child.Visibility != ViewStates.Gone)
//                {
//                    var lp = (MyLayoutParams)child.LayoutParameters;
//                    ItemInfo ii;

//                    if (!lp.IsDecor && (ii = InfoForChild(child)) != null)
//                    {
//                        int loff = (int)(childWidth * ii.offset);
//                        int childLeft = paddingLeft + loff;
//                        int childTop = paddingTop;

//                        if (lp.NeedsMeasure)
//                        {
//                            // This was added during layout and needs measurement.
//                            // Do it now that we know what we're working with.
//                            lp.NeedsMeasure = false;
//                            int widthSpec = MeasureSpec.MakeMeasureSpec((int)(childWidth * lp.WidthFactor), MeasureSpecMode.Exactly);
//                            int heightSpec = MeasureSpec.MakeMeasureSpec((int)(height - paddingTop - paddingBottom), MeasureSpecMode.Exactly);
//                            child.Measure(widthSpec, heightSpec);
//                        }

//                        if (DEBUG) Log.Verbose(TAG, "Positioning #" + i + " " + child + " f=" + ii.@object
//                              + ":" + childLeft + "," + childTop + " " + child.MeasuredWidth
//                              + "x" + child.MeasuredHeight);

//                        child.Layout(childLeft, childTop, childLeft + child.MeasuredWidth, childTop + child.MeasuredHeight);
//                    }
//                }
//            }

//            mTopPageBounds = paddingTop;
//            mBottomPageBounds = height - paddingBottom;
//            mDecorChildCount = decorCount;

//            if (mFirstLayout)
//            {
//                ScrollToItem(mCurItem, false, 0, false);
//            }

//            mFirstLayout = false;
//        }

//        public override void ComputeScroll()
//        {
//            if (!mScroller.IsFinished && mScroller.ComputeScrollOffset())
//            {
//                int oldX = ScrollX;
//                int oldY = ScrollY;
//                int x = mScroller.CurrX;
//                int y = mScroller.CurrY;

//                if (oldX != x || oldY != y)
//                {
//                    ScrollTo(x, y);
//                    if (!PageScrolled(x))
//                    {
//                        mScroller.AbortAnimation();
//                        ScrollTo(0, y);
//                    }
//                }

//                // Keep on drawing until the animation has finished.
//                PostInvalidateOnAnimation();
//                return;
//            }

//            // Done with scroll, clean up state.
//            CompleteScroll(true);
//        }

//        private bool PageScrolled(int xpos)
//        {
//            if (mItems.Count == 0)
//            {
//                mCalledSuper = false;
//                OnPageScrolled(0, 0, 0);

//                if (!mCalledSuper)
//                {
//                    throw new IllegalStateException(
//                            "onPageScrolled did not call superclass implementation");
//                }
//                return false;
//            }

//            ItemInfo ii = InfoForCurrentScrollPosition();
//            int width = GetClientWidth();
//            int widthWithMargin = width + mPageMargin;
//            float marginOffset = (float)mPageMargin / width;
//            int currentPage = ii.position;
//            float pageOffset = (((float)xpos / width) - ii.offset) /
//                   (ii.widthFactor + marginOffset);
//            int offsetPixels = (int)(pageOffset * widthWithMargin);

//            mCalledSuper = false;
//            OnPageScrolled(currentPage, pageOffset, offsetPixels);

//            if (!mCalledSuper)
//            {
//                throw new IllegalStateException("onPageScrolled did not call superclass implementation");
//            }

//            return true;
//        }

//        /**
//         * This method will be invoked when the current page is scrolled, either as part of a programmatically initiated smooth scroll or a user initiated touch scroll. If you override this method you must call through to the superclass implementation (e.g. super.onPageScrolled(position, offset,
//         * offsetPixels)) before onPageScrolled returns.
//         *
//         * @param position     Position index of the first page currently being displayed. Page position+1 will be visible if positionOffset is nonzero.
//         * @param offset       Value from [0, 1) indicating the offset from the page at position.
//         * @param offsetPixels Value in pixels indicating the offset from position.
//         */
//        protected void OnPageScrolled(int position, float offset, int offsetPixels)
//        {
//            // Offset any decor views if needed - keep them on-screen at all times.
//            if (mDecorChildCount > 0)
//            {
//                int scrollX = ScrollX;
//                int paddingLeft = PaddingLeft;
//                int paddingRight = PaddingRight;
//                int width = Width;
//                int childCount = ChildCount;

//                for (int i = 0; i < childCount; i++)
//                {
//                    View child = GetChildAt(i);
//                    var lp = (MyLayoutParams)child.LayoutParameters;
//                    if (!lp.IsDecor)
//                    {
//                        continue;
//                    }

//                    GravityFlags hgrav = lp.Gravity & GravityFlags.HorizontalGravityMask;
//                    int childLeft = 0;
//                    switch (hgrav)
//                    {
//                        default:
//                            childLeft = paddingLeft;
//                            break;
//                        case GravityFlags.Left:
//                            childLeft = paddingLeft;
//                            paddingLeft += child.Width;
//                            break;
//                        case GravityFlags.CenterHorizontal:
//                            childLeft = System.Math.Max((width - child.MeasuredWidth) / 2,
//                                                  paddingLeft);
//                            break;
//                        case GravityFlags.Right:
//                            childLeft = width - paddingRight - child.MeasuredWidth;
//                            paddingRight += child.MeasuredWidth;
//                            break;
//                    }
//                    childLeft += scrollX;

//                    int childOffset = childLeft - child.Left;
//                    if (childOffset != 0)
//                    {
//                        child.OffsetLeftAndRight(childOffset);
//                    }
//                }
//            }

//            if (mOnPageChangeListener != null)
//            {
//                mOnPageChangeListener.OnPageScrolled(position, offset, offsetPixels);
//            }
//            if (mInternalPageChangeListener != null)
//            {
//                mInternalPageChangeListener.OnPageScrolled(position, offset, offsetPixels);
//            }

//            if (mPageTransformer != null)
//            {
//                int scrollX = ScrollX;
//                int childCount = ChildCount;
//                for (int i = 0; i < childCount; i++)
//                {
//                    View child = GetChildAt(i);
//                    var lp = (MyLayoutParams)child.LayoutParameters;

//                    if (lp.IsDecor) continue;

//                    float transformPos = (float)(child.Left - scrollX) / GetClientWidth();
//                    mPageTransformer.TransformPage(child, transformPos);
//                }
//            }

//            mCalledSuper = true;
//        }

//        private void VompleteScroll(bool postEvents)
//        {
//            bool needPopulate = mScrollState == SCROLL_STATE_SETTLING;

//            if (needPopulate)
//            {
//                // Done with scroll, no longer want to cache view drawing.
//                SetScrollingCacheEnabled(false);
//                mScroller.AbortAnimation();
//                int oldX = ScrollX;
//                int oldY = ScrollY;
//                int x = mScroller.CurrX;
//                int y = mScroller.CurrY;
//                if (oldX != x || oldY != y)
//                {
//                    ScrollTo(x, y);
//                }
//            }

//            mPopulatePending = false;

//            for (int i = 0; i < mItems.Count; i++)
//            {
//                ItemInfo ii = mItems[i];

//                if (ii.scrolling)
//                {
//                    needPopulate = true;
//                    ii.scrolling = false;
//                }
//            }
//            if (needPopulate)
//            {
//                if (postEvents)
//                {
//                    PostOnAnimation(mEndScrollRunnable);
//                }
//                else
//                {
//                    mEndScrollRunnable.Run();
//                }
//            }
//        }

//        private bool IsGutterDrag(float x, float dx)
//        {
//            return (x < mGutterSize && dx > 0) || (x > Width - mGutterSize && dx < 0);
//        }

//        private void EnableLayers(bool enable)
//        {
//            int childCount = ChildCount;

//            for (int i = 0; i < childCount; i++)
//            {
//                var layerType = enable ? LayerType.Hardware : LayerType.None;
//                GetChildAt(i).SetLayerType(layerType, null);
//            }
//        }

//        public override bool OnInterceptTouchEvent(MotionEvent ev)
//        {
//            /*
//             * This method JUST determines whether we want to intercept the motion. If we return true, onMotionEvent will be called and we do the actual scrolling there.
//             */

//            MotionEventActions action = ev.Action & MotionEventActions.Mask;

//            // Always take care of the touch gesture being complete.
//            if (action == MotionEventActions.Cancel || action == MotionEventActions.Up)
//            {
//                // Release the drag.
//                if (DEBUG) Log.Verbose(TAG, "Intercept done!");
//                mIsBeingDragged = false;
//                mIsUnableToDrag = false;
//                mActivePointerId = INVALID_POINTER;
//                if (mVelocityTracker != null)
//                {
//                    mVelocityTracker.Recycle();
//                    mVelocityTracker = null;
//                }
//                return false;
//            }

//            // Nothing more to do here if we have decided whether or not we
//            // are dragging.
//            if (action != MotionEventActions.Down)
//            {
//                if (mIsBeingDragged)
//                {
//                    if (DEBUG) Log.Verbose(TAG, "Intercept returning true!");
//                    return true;
//                }
//                if (mIsUnableToDrag)
//                {
//                    if (DEBUG) Log.Verbose(TAG, "Intercept returning false!");
//                    return false;
//                }
//            }

//            switch (action)
//            {
//                case MotionEventActions.Move:
//                    {
//                        /*
//                         * mIsBeingDragged == false, otherwise the shortcut would have caught it. Check whether the user has moved far enough from his original down touch.
//                         */

//                        /*
//                         * Locally do absolute value. mLastMotionY is set to the y value of the down event.
//                         */
//                        int activePointerId = mActivePointerId;
//                        if (activePointerId == INVALID_POINTER)
//                        {
//                            // If we don't have a valid id, the touch down wasn't on content.
//                            break;
//                        }

//                        int pointerIndex = ev.FindPointerIndex(activePointerId);
//                        float x = ev.GetX(pointerIndex);
//                        float dx = x - mLastMotionX;
//                        float xDiff = System.Math.Abs(dx);
//                        float y = ev.GetY(pointerIndex);
//                        float yDiff = System.Math.Abs(y - mInitialMotionY);
//                        if (DEBUG) Log.Verbose(TAG, "Moved x to " + x + "," + y + " diff=" + xDiff + "," + yDiff);

//                        if (dx != 0 && !IsGutterDrag(mLastMotionX, dx) && CanScroll(this, false, (int)dx, (int)x, (int)y))
//                        {
//                            // Nested view has scrollable area under this point. Let it be handled there.
//                            mLastMotionX = x;
//                            mLastMotionY = y;
//                            mIsUnableToDrag = true;
//                            return false;
//                        }
//                        if (xDiff > mTouchSlop && xDiff * 0.5f > yDiff)
//                        {
//                            if (DEBUG) Log.Verbose(TAG, "Starting drag!");
//                            mIsBeingDragged = true;
//                            RequestParentDisallowInterceptTouchEvent(true);
//                            SetScrollState(SCROLL_STATE_DRAGGING);
//                            mLastMotionX = dx > 0 ? mInitialMotionX + mTouchSlop :
//                                    mInitialMotionX - mTouchSlop;
//                            mLastMotionY = y;
//                            SetScrollingCacheEnabled(true);
//                        }
//                        else if (yDiff > mTouchSlop)
//                        {
//                            // The finger has moved enough in the vertical
//                            // direction to be counted as a drag... abort
//                            // any attempt to drag horizontally, to work correctly
//                            // with children that have scrolling containers.
//                            if (DEBUG) Log.Verbose(TAG, "Starting unable to drag!");
//                            mIsUnableToDrag = true;
//                        }
//                        if (mIsBeingDragged)
//                        {
//                            // Scroll to follow the motion event
//                            if (PerformDrag(x))
//                            {
//                                PostInvalidateOnAnimation();
//                            }
//                        }
//                        break;
//                    }

//                case MotionEventActions.Down:
//                    {
//                        /*
//                         * Remember location of down touch. ACTION_DOWN always refers to pointer index 0.
//                         */
//                        mLastMotionX = mInitialMotionX = ev.GetX();
//                        mLastMotionY = mInitialMotionY = ev.GetY();
//                        mActivePointerId = ev.GetPointerId(0);
//                        mIsUnableToDrag = false;

//                        mScroller.ComputeScrollOffset();
//                        if (mScrollState == SCROLL_STATE_SETTLING &&
//                                System.Math.Abs(mScroller.CurrX - mScroller.CurrX) > mCloseEnough)
//                        {
//                            // Let the user 'catch' the pager as it animates.
//                            mScroller.AbortAnimation();
//                            mPopulatePending = false;
//                            Populate();
//                            mIsBeingDragged = true;
//                            RequestParentDisallowInterceptTouchEvent(true);
//                            SetScrollState(SCROLL_STATE_DRAGGING);
//                        }
//                        else
//                        {
//                            CompleteScroll(false);
//                            mIsBeingDragged = false;
//                        }

//                        if (DEBUG) Log.Verbose(TAG, "Down at " + mLastMotionX + "," + mLastMotionY
//                              + " mIsBeingDragged=" + mIsBeingDragged
//                              + "mIsUnableToDrag=" + mIsUnableToDrag);
//                        break;
//                    }

//                case MotionEventActions.PointerUp:
//                    OnSecondaryPointerUp(ev);
//                    break;
//            }

//            if (mVelocityTracker == null)
//            {
//                mVelocityTracker = VelocityTracker.Obtain();
//            }
//            mVelocityTracker.AddMovement(ev);

//            /*
//             * The only time we want to intercept motion events is if we are in the drag mode.
//             */
//            return mIsBeingDragged;
//        }

//        public override bool OnTouchEvent(MotionEvent ev)
//        {
//            if (mFakeDragging)
//            {
//                // A fake drag is in progress already, ignore this real one
//                // but still eat the touch events.
//                // (It is likely that the user is multi-touching the screen.)
//                return true;
//            }

//            if (ev.Action == MotionEventActions.Down && ev.EdgeFlags != 0)
//            {
//                // Don't handle edge touches immediately -- they may actually belong to one of our
//                // descendants.
//                return false;
//            }

//            if (mAdapter == null || mAdapter.GetCount() == 0)
//            {
//                // Nothing to present or scroll; nothing to touch.
//                return false;
//            }

//            if (mVelocityTracker == null)
//            {
//                mVelocityTracker = VelocityTracker.Obtain();
//            }
//            mVelocityTracker.AddMovement(ev);

//            MotionEventActions action = ev.Action;
//            bool needsInvalidate = false;

//            switch (action & MotionEventActions.Mask)
//            {
//                case MotionEventActions.Down:
//                    {
//                        mScroller.AbortAnimation();
//                        mPopulatePending = false;
//                        Populate();

//                        // Remember where the motion event started
//                        mLastMotionX = mInitialMotionX = ev.GetX();
//                        mLastMotionY = mInitialMotionY = ev.GetY();
//                        mActivePointerId = ev.GetPointerId(0);
//                        break;
//                    }
//                case MotionEventActions.Move:
//                    if (!mIsBeingDragged)
//                    {
//                        int pointerIndex = ev.FindPointerIndex(mActivePointerId);
//                        float x = ev.GetX(pointerIndex);
//                        float xDiff = System.Math.Abs(x - mLastMotionX);
//                        float y = ev.GetY(pointerIndex);
//                        float yDiff = System.Math.Abs(y - mLastMotionY);
//                        if (DEBUG)
//                        {
//                            Log.Verbose(TAG, "Moved x to " + x + "," + y + " diff=" + xDiff + "," + yDiff);
//                        }
//                        if (xDiff > mTouchSlop && xDiff > yDiff)
//                        {
//                            if (DEBUG) Log.Verbose(TAG, "Starting drag!");
//                            mIsBeingDragged = true;
//                            RequestParentDisallowInterceptTouchEvent(true);
//                            mLastMotionX = x - mInitialMotionX > 0 ? mInitialMotionX + mTouchSlop :
//                                    mInitialMotionX - mTouchSlop;
//                            mLastMotionY = y;
//                            SetScrollState(SCROLL_STATE_DRAGGING);
//                            SetScrollingCacheEnabled(true);

//                            // Disallow Parent Intercept, just in case
//                            if (Parent != null)
//                            {
//                                Parent.RequestDisallowInterceptTouchEvent(true);
//                            }
//                        }
//                    }
//                    // Not else! Note that mIsBeingDragged can be set above.
//                    if (mIsBeingDragged)
//                    {
//                        // Scroll to follow the motion event
//                        int activePointerIndex = ev.FindPointerIndex(mActivePointerId);
//                        float x = ev.GetX(activePointerIndex);
//                        needsInvalidate |= PerformDrag(x);
//                    }
//                    break;
//                case MotionEventActions.Up:
//                    if (mIsBeingDragged)
//                    {
//                        VelocityTracker velocityTracker = mVelocityTracker;
//                        velocityTracker.ComputeCurrentVelocity(1000, mMaximumVelocity);
//                        int initialVelocity = (int)velocityTracker.GetXVelocity(mActivePointerId);
//                        mPopulatePending = true;
//                        int width = GetClientWidth();
//                        int scrollX = ScrollX;
//                        ItemInfo ii = InfoForCurrentScrollPosition();
//                        int currentPage = ii.position;
//                        float pageOffset = (((float)scrollX / width) - ii.offset) / ii.widthFactor;
//                        int activePointerIndex =
//                               ev.FindPointerIndex(mActivePointerId);
//                        float x = ev.GetX(activePointerIndex);
//                        int totalDelta = (int)(x - mInitialMotionX);
//                        int nextPage = DetermineTargetPage(currentPage, pageOffset, initialVelocity,
//                                                            totalDelta);
//                        SetCurrentItemInternal(nextPage, true, true, initialVelocity);

//                        mActivePointerId = INVALID_POINTER;
//                        EndDrag();

//                        mLeftEdge.OnRelease();
//                        mRightEdge.OnRelease();

//                        needsInvalidate = mLeftEdge.IsFinished | mRightEdge.IsFinished;
//                    }
//                    break;
//                case MotionEventActions.Cancel:
//                    if (mIsBeingDragged)
//                    {
//                        ScrollToItem(mCurItem, true, 0, false);
//                        mActivePointerId = INVALID_POINTER;
//                        EndDrag();

//                        mLeftEdge.OnRelease();
//                        mRightEdge.OnRelease();

//                        needsInvalidate = mLeftEdge.IsFinished | mRightEdge.IsFinished;
//                    }
//                    break;
//                case MotionEventActions.PointerDown:
//                    {
//                        int index = ev.ActionIndex;
//                        float x = ev.GetX(index);
//                        mLastMotionX = x;
//                        mActivePointerId = ev.GetPointerId(index);
//                        break;
//                    }
//                case MotionEventActions.PointerUp:
//                    OnSecondaryPointerUp(ev);
//                    mLastMotionX = ev.GetX(ev.FindPointerIndex(mActivePointerId));
//                    break;
//            }
//            if (needsInvalidate)
//            {
//                PostInvalidateOnAnimation();
//            }
//            return true;
//        }

//        private void RequestParentDisallowInterceptTouchEvent(bool disallowIntercept)
//        {
//            if (Parent != null)
//            {
//                Parent.RequestDisallowInterceptTouchEvent(disallowIntercept);
//            }
//        }

//        private bool PerformDrag(float x)
//        {
//            bool needsInvalidate = false;

//            float deltaX = mLastMotionX - x;
//            mLastMotionX = x;

//            float oldScrollX = ScrollX;
//            float scrollX = oldScrollX + deltaX;
//            int width = GetClientWidth();

//            float leftBound = width * mFirstOffset;
//            float rightBound = width * mLastOffset;
//            bool leftAbsolute = true;
//            bool rightAbsolute = true;

//            ItemInfo firstItem = mItems[0];
//            ItemInfo lastItem = mItems[mItems.Count - 1];
//            if (firstItem.position != 0)
//            {
//                leftAbsolute = false;
//                leftBound = firstItem.offset * width;
//            }
//            if (lastItem.position != mAdapter.GetCount() - 1)
//            {
//                rightAbsolute = false;
//                rightBound = lastItem.offset * width;
//            }

//            // Matt edit: If we're center locked...
//            if (AL_CENTER_LOCK_ENABLED)
//            {
//                if (mCurItem == 0)
//                {
//                    // And we're the left page, adjust the bound.
//                    leftBound -= GetInterPagePadding();
//                }
//                else if (mCurItem == mAdapter.GetCount() - 1)
//                {
//                    // If we're the right page adjust the right hand bound.
//                    rightBound += GetInterPagePadding();
//                }
//            }

//            if (scrollX < leftBound)
//            {
//                if (leftAbsolute)
//                {
//                    float over = leftBound - scrollX;
//                    mLeftEdge.OnPull(System.Math.Abs(over) / width);
//                    needsInvalidate = true;
//                }
//                scrollX = leftBound;
//            }
//            else if (scrollX > rightBound)
//            {
//                if (rightAbsolute)
//                {
//                    float over = scrollX - rightBound;
//                    mRightEdge.OnPull(System.Math.Abs(over) / width);
//                    needsInvalidate = true;
//                }
//                scrollX = rightBound;
//            }
//            // Don't lose the rounded component
//            mLastMotionX += scrollX - (int)scrollX;
//            ScrollTo((int)scrollX, ScrollY);
//            PageScrolled((int)scrollX);

//            return needsInvalidate;
//        }

//        /**
//         * @return Info about the page at the current scroll position. This can be synthetic for a missing middle page; the 'object' field can be null.
//         */
//        private ItemInfo InfoForCurrentScrollPosition()
//        {
//            int width = GetClientWidth();
//            float scrollOffset = width > 0 ? (float)ScrollX / width : 0;
//            float marginOffset = width > 0 ? (float)mPageMargin / width : 0;
//            int lastPos = -1;
//            float lastOffset = 0.0f;
//            float lastWidth = 0.0f;
//            bool first = true;

//            ItemInfo lastItem = null;
//            for (int i = 0; i < mItems.Count; i++)
//            {
//                ItemInfo ii = mItems[i];
//                float offset;
//                if (!first && ii.position != lastPos + 1)
//                {
//                    // Create a synthetic item for a missing page.
//                    ii = mTempItem;
//                    ii.offset = lastOffset + lastWidth + marginOffset;
//                    ii.position = lastPos + 1;
//                    ii.widthFactor = mAdapter.GetPageWidth(ii.position);
//                    i--;
//                }
//                offset = ii.offset;

//                float leftBound = offset;
//                float rightBound = offset + ii.widthFactor + marginOffset;
//                if (first || scrollOffset >= leftBound)
//                {
//                    if (scrollOffset < rightBound || i == mItems.Count - 1)
//                    {
//                        return ii;
//                    }
//                }
//                else
//                {
//                    return lastItem;
//                }
//                first = false;
//                lastPos = ii.position;
//                lastOffset = offset;
//                lastWidth = ii.widthFactor;
//                lastItem = ii;
//            }

//            return lastItem;
//        }

//        private int DetermineTargetPage(int currentPage, float pageOffset, int velocity, int deltaX)
//        {
//            int targetPage;
//            if (System.Math.Abs(deltaX) > mFlingDistance && System.Math.Abs(velocity) > mMinimumVelocity)
//            {
//                targetPage = velocity > 0 ? currentPage : currentPage + 1;
//            }
//            else
//            {
//                float truncator = currentPage >= mCurItem ? 0.4f : 0.6f;
//                targetPage = (int)(currentPage + pageOffset + truncator);
//            }

//            if (mItems.Count > 0)
//            {
//                ItemInfo firstItem = mItems[0];
//                ItemInfo lastItem = mItems[mItems.Count - 1];

//                // Only let the user target pages we have items for
//                targetPage = System.Math.Max(firstItem.position, System.Math.Min(targetPage, lastItem.position));
//            }

//            return targetPage;
//        }

//        public override void Draw(Canvas canvas)
//        {
//            base.Draw(canvas);
//            bool needsInvalidate = false;

//            OverScrollMode overScrollMode = OverScrollMode;

//            if (overScrollMode == OverScrollMode.Always || (overScrollMode == OverScrollMode.IfContentScrolls && mAdapter != null && mAdapter.GetCount() > 1))
//            {
//                if (!mLeftEdge.IsFinished)
//                {
//                    int restoreCount = canvas.Save();
//                    int height = Height - PaddingTop - PaddingBottom;
//                    int width = Width;

//                    canvas.Rotate(270);
//                    canvas.Translate(-height + PaddingTop, mFirstOffset * width);
//                    mLeftEdge.SetSize(height, width);
//                    needsInvalidate |= mLeftEdge.Draw(canvas);
//                    canvas.RestoreToCount(restoreCount);
//                }
//                if (!mRightEdge.IsFinished)
//                {
//                    int restoreCount = canvas.Save();
//                    int width = Width;
//                    int height = Height - PaddingTop - PaddingBottom;

//                    canvas.Rotate(90);
//                    canvas.Translate(-PaddingTop, -(mLastOffset + 1) * width);
//                    mRightEdge.SetSize(height, width);
//                    needsInvalidate |= mRightEdge.Draw(canvas);
//                    canvas.RestoreToCount(restoreCount);
//                }
//            }
//            else
//            {
//                mLeftEdge.Finish();
//                mRightEdge.Finish();
//            }

//            if (needsInvalidate)
//            {
//                // Keep animating
//                PostInvalidateOnAnimation();
//            }
//        }

//        protected override void OnDraw(Canvas canvas)
//        {
//            base.OnDraw(canvas);

//            // Draw the margin drawable between pages if needed.
//            if (mPageMargin > 0 && mMarginDrawable != null && mItems.Count > 0 && mAdapter != null)
//            {
//                int scrollX = ScrollX;
//                int width = Width;

//                float marginOffset = (float)mPageMargin / width;
//                int itemIndex = 0;
//                ItemInfo ii = mItems[0];
//                float offset = ii.offset;
//                int itemCount = mItems.Count;
//                int firstPos = ii.position;
//                int lastPos = mItems[itemCount - 1].position;
//                for (int pos = firstPos; pos < lastPos; pos++)
//                {
//                    while (pos > ii.position && itemIndex < itemCount)
//                    {
//                        ii = mItems[++itemIndex];
//                    }

//                    float drawAt;
//                    if (pos == ii.position)
//                    {
//                        drawAt = (ii.offset + ii.widthFactor) * width;
//                        offset = ii.offset + ii.widthFactor + marginOffset;
//                    }
//                    else
//                    {
//                        float widthFactor = mAdapter.GetPageWidth(pos);
//                        drawAt = (offset + widthFactor) * width;
//                        offset += widthFactor + marginOffset;
//                    }

//                    if (drawAt + mPageMargin > scrollX)
//                    {
//                        mMarginDrawable.SetBounds((int)drawAt, mTopPageBounds, (int)(drawAt + mPageMargin + 0.5f), mBottomPageBounds);
//                        mMarginDrawable.Draw(canvas);
//                    }

//                    if (drawAt > scrollX + width)
//                    {
//                        break; // No more visible, no sense in continuing
//                    }
//                }
//            }
//        }

//        /**
//         * Start a fake drag of the pager.
//         * <p/>
//         * <p>
//         * A fake drag can be useful if you want to synchronize the motion of the ViewPager with the touch scrolling of another view, while still letting the ViewPager control the snapping motion and fling behavior. (e.g. parallax-scrolling tabs.) Call {@link #fakeDragBy(float)} to simulate the actual
//         * drag motion. Call {@link #endFakeDrag()} to complete the fake drag and fling as necessary.
//         * <p/>
//         * <p>
//         * During a fake drag the ViewPager will ignore all touch events. If a real drag is already in progress, this method will return false.
//         *
//         * @return true if the fake drag began successfully, false if it could not be started.
//         * @see #fakeDragBy(float)
//         * @see #endFakeDrag()
//         */
//        public bool BeginFakeDrag()
//        {
//            if (mIsBeingDragged)
//            {
//                return false;
//            }
//            mFakeDragging = true;
//            SetScrollState(SCROLL_STATE_DRAGGING);
//            mInitialMotionX = mLastMotionX = 0;
//            if (mVelocityTracker == null)
//            {
//                mVelocityTracker = VelocityTracker.Obtain();
//            }
//            else
//            {
//                mVelocityTracker.Clear();
//            }
//            long time = SystemClock.UptimeMillis();
//            MotionEvent ev = MotionEvent.Obtain(time, time, MotionEventActions.Down, 0, 0, 0);
//            mVelocityTracker.AddMovement(ev);
//            ev.Recycle();
//            mFakeDragBeginTime = time;
//            return true;
//        }

//        /**
//         * End a fake drag of the pager.
//         *
//         * @see #beginFakeDrag()
//         * @see #fakeDragBy(float)
//         */
//        public void EndFakeDrag()
//        {
//            if (!mFakeDragging)
//            {
//                throw new IllegalStateException("No fake drag in progress. Call beginFakeDrag first.");
//            }

//            VelocityTracker velocityTracker = mVelocityTracker;
//            velocityTracker.ComputeCurrentVelocity(1000, mMaximumVelocity);
//            int initialVelocity = (int)velocityTracker.GetXVelocity(mActivePointerId);
//            mPopulatePending = true;
//            int width = GetClientWidth();
//            int scrollX = ScrollX;
//            ItemInfo ii = InfoForCurrentScrollPosition();
//            int currentPage = ii.position;
//            float pageOffset = (((float)scrollX / width) - ii.offset) / ii.widthFactor;
//            int totalDelta = (int)(mLastMotionX - mInitialMotionX);
//            int nextPage = DetermineTargetPage(currentPage, pageOffset, initialVelocity, totalDelta);
//            SetCurrentItemInternal(nextPage, true, true, initialVelocity);
//            EndDrag();

//            mFakeDragging = false;
//        }

//        /**
//         * Fake drag by an offset in pixels. You must have called {@link #beginFakeDrag()} first.
//         *
//         * @param xOffset Offset in pixels to drag by.
//         *
//         * @see #beginFakeDrag()
//         * @see #endFakeDrag()
//         */
//        public void FakeDragBy(float xOffset)
//        {
//            if (!mFakeDragging)
//            {
//                throw new IllegalStateException("No fake drag in progress. Call beginFakeDrag first.");
//            }

//            mLastMotionX += xOffset;

//            float oldScrollX = ScrollX;
//            float scrollX = oldScrollX - xOffset;
//            int width = GetClientWidth();

//            float leftBound = width * mFirstOffset;
//            float rightBound = width * mLastOffset;

//            ItemInfo firstItem = mItems[0];
//            ItemInfo lastItem = mItems[mItems.Count - 1];
//            if (firstItem.position != 0)
//            {
//                leftBound = firstItem.offset * width;
//            }
//            if (lastItem.position != mAdapter.GetCount() - 1)
//            {
//                rightBound = lastItem.offset * width;
//            }

//            if (scrollX<leftBound)
//            {
//                scrollX = leftBound;
//            }
//            else if (scrollX > rightBound)
//            {
//                scrollX = rightBound;
//            }
//            // Don't lose the rounded component
//            mLastMotionX += scrollX - (int) scrollX;
//            ScrollTo((int) scrollX, ScrollY);
//            PageScrolled((int) scrollX);

//            // Synthesize an event for the VelocityTracker.
//            long time = SystemClock.UptimeMillis();
//            MotionEvent ev = MotionEvent.Obtain(mFakeDragBeginTime, time, MotionEventActions.Move, mLastMotionX, 0, 0);
//            mVelocityTracker.AddMovement(ev);
//            ev.Recycle();
//        }

//        /**
//            * Returns true if a fake drag is in progress.
//            *
//            * @return true if currently in a fake drag, false otherwise.
//            * @see #beginFakeDrag()
//            * @see #fakeDragBy(float)
//            * @see #endFakeDrag()
//            */
//        public bool IsFakeDragging()
//        {
//            return mFakeDragging;
//        }

//        private void OnSecondaryPointerUp(MotionEvent ev)
//        {
//            int pointerIndex = ev.ActionIndex;
//            int pointerId = ev.GetPointerId(pointerIndex);

//            if (pointerId == mActivePointerId)
//            {
//                // This was our active pointer going up. Choose a new
//                // active pointer and adjust accordingly.
//                int newPointerIndex = pointerIndex == 0 ? 1 : 0;
//                mLastMotionX = ev.GetX(newPointerIndex);
//                mActivePointerId = ev.GetPointerId(newPointerIndex);

//                if (mVelocityTracker != null)
//                {
//                    mVelocityTracker.Clear();
//                }
//            }
//        }

//        private void EndDrag()
//        {
//            mIsBeingDragged = false;
//            mIsUnableToDrag = false;

//            if (mVelocityTracker != null)
//            {
//                mVelocityTracker.Recycle();
//                mVelocityTracker = null;
//            }
//        }

//        private void SetScrollingCacheEnabled(bool enabled)
//        {
//            if (mScrollingCacheEnabled != enabled)
//            {
//                mScrollingCacheEnabled = enabled;

//                if (USE_CACHE)
//                {
//                    int size = ChildCount;

//                    for (int i = 0; i < size; ++i)
//                    {
//                        View child = GetChildAt(i);

//                        if (child.Visibility != ViewStates.Gone)
//                        {
//                            child.DrawingCacheEnabled = enabled;
//                        }
//                    }
//                }
//            }
//        }

//        public override bool CanScrollHorizontally(int direction)
//        {
//            if (mAdapter == null)
//            {
//                return false;
//            }

//            int width = GetClientWidth();
//            int scrollX = ScrollX;

//            if (direction < 0)
//            {
//                return (scrollX > (int)(width * mFirstOffset));
//            }
//            else if (direction > 0)
//            {
//                return (scrollX < (int)(width * mLastOffset));
//            }
//            else
//            {
//                return false;
//            }
//        }

//        /**
//         * Tests scrollability within child views of v given a delta of dx.
//         *
//         * @param v      View to test for horizontal scrollability
//         * @param checkV Whether the view v passed should itself be checked for scrollability (true), or just its children (false).
//         * @param dx     Delta scrolled in pixels
//         * @param x      X coordinate of the active touch point
//         * @param y      Y coordinate of the active touch point
//         *
//         * @return true if child views of v can be scrolled by delta of dx.
//         */
//        protected bool CanScroll(View v, bool checkV, int dx, int x, int y)
//        {
//            if (v is ViewGroup)
//            {
//                ViewGroup group = (ViewGroup)v;
//                int scrollX = v.ScrollX;
//                int scrollY = v.ScrollY;
//                int count = group.ChildCount;
//                // Count backwards - let topmost views consume scroll distance first.
//                for (int i = count - 1; i >= 0; i--)
//                {
//                    // to do Add versioned support here for transformed views.
//                    // This will not work for transformed views in Honeycomb+
//                    View child = group.GetChildAt(i);
//                    if (x + scrollX >= child.Left && x + scrollX < child.Right &&
//                            y + scrollY >= child.Top && y + scrollY < child.Bottom &&
//                            CanScroll(child, true, dx, x + scrollX - child.Left,
//                                       y + scrollY - child.Top))
//                    {
//                        return true;
//                    }
//                }
//            }

//            return checkV && CanScrollHorizontally(-dx);
//        }

//        public override bool DispatchKeyEvent(KeyEvent @event)
//        {
//            // Let the focused view and/or our descendants get the key first
//            return base.DispatchKeyEvent(@event) || ExecuteKeyEvent(@event);
//        }

//        /**
//         * You can call this function yourself to have the scroll view perform scrolling from a key event, just as if the event had been dispatched to it by the view hierarchy.
//         *
//         * @param event The key event to execute.
//         *
//         * @return Return true if the event was handled, else false.
//         */
//        public bool ExecuteKeyEvent(KeyEvent @event)
//        {
//            bool handled = false;

//            if (@event.Action == KeyEventActions.Down)
//            {
//                switch (@event.KeyCode)
//                {
//                    case Keycode.DpadLeft:
//                        handled = ArrowScroll(FocusSearchDirection.Left);
//                    break;
//                    case Keycode.DpadRight:
//                        handled = ArrowScroll(FocusSearchDirection.Right);
//                    break;
//                    case Keycode.Tab:
//                        if ((int)Build.VERSION.SdkInt >= 11)
//                        {
//                            // The focus finder had a bug handling FOCUS_FORWARD and FOCUS_BACKWARD
//                            // before Android 3.0. Ignore the tab key on those devices.
//                            if (@event.HasNoModifiers)
//                            {
//                                handled = ArrowScroll(FocusSearchDirection.Forward);
//                            }
//                            else if (@event.HasModifiers(MetaKeyStates.ShiftOn))
//                            {
//                                handled = ArrowScroll(FocusSearchDirection.Backward);
//                            }
//                        }
//                    break;
//                }
//            }

//            return handled;
//        }

//        public bool ArrowScroll(FocusSearchDirection direction)
//        {
//            View currentFocused = FindFocus();

//            if (currentFocused == this)
//            {
//                currentFocused = null;
//            }
//            else if (currentFocused != null)
//            {
//                bool isChild = false;

//                for (var parent = currentFocused.Parent; parent is ViewGroup; parent = parent.Parent)
//                {
//                    if (parent == this)
//                    {
//                        isChild = true;
//                        break;
//                    }
//                }

//                if (!isChild)
//                {
//                    // This would cause the focus search down below to fail in fun ways.
//                    var sb = new System.Text.StringBuilder();
//                    sb.Append(currentFocused.Class.SimpleName);

//                    for (var parent = currentFocused.Parent; parent is ViewGroup; parent = parent.Parent)
//                    {
//                        sb.Append(" => ").Append(parent.GetType().Name);
//                    }

//                    Log.Error(TAG, "arrowScroll tried to find focus based on non-child " + "current focused view " + sb.ToString());
//                    currentFocused = null;
//                }
//            }

//            bool handled = false;

//            View nextFocused = FocusFinder.Instance.FindNextFocus(this, currentFocused, direction);

//            if (nextFocused != null && nextFocused != currentFocused)
//            {
//                if (direction == FocusSearchDirection.Left)
//                {
//                    // If there is nothing to the left, or this is causing us to
//                    // jump to the right, then what we really want to do is page left.
//                    int nextLeft = GetChildRectInPagerCoordinates(mTempRect, nextFocused).Left;
//                    int currLeft = GetChildRectInPagerCoordinates(mTempRect, currentFocused).Left;
//                    if (currentFocused != null && nextLeft >= currLeft)
//                    {
//                        handled = PageLeft();
//                    }
//                    else
//                    {
//                        handled = nextFocused.RequestFocus();
//                    }
//                }
//                else if (direction == FocusSearchDirection.Right)
//                {
//                    // If there is nothing to the right, or this is causing us to
//                    // jump to the left, then what we really want to do is page right.
//                    int nextLeft = GetChildRectInPagerCoordinates(mTempRect, nextFocused).Left;
//                    int currLeft = GetChildRectInPagerCoordinates(mTempRect, currentFocused).Left;
//                    if (currentFocused != null && nextLeft <= currLeft)
//                    {
//                        handled = PageRight();
//                    }
//                    else
//                    {
//                        handled = nextFocused.RequestFocus();
//                    }
//                }
//            }
//            else if (direction == FocusSearchDirection.Left || direction == FocusSearchDirection.Backward)
//            {
//                // Trying to move left and nothing there; try to page.
//                handled = PageLeft();
//            }
//            else if (direction == FocusSearchDirection.Right || direction == FocusSearchDirection.Forward)
//            {
//                // Trying to move right and nothing there; try to page.
//                handled = PageRight();
//            }
//            if (handled)
//            {
//                PlaySoundEffect(SoundEffectConstants.GetContantForFocusDirection(direction));
//            }
//            return handled;
//        }

//        private Rect GetChildRectInPagerCoordinates(Rect outRect, View child)
//        {
//            if (outRect == null)
//            {
//                outRect = new Rect();
//            }
//            if (child == null)
//            {
//                outRect.Set(0, 0, 0, 0);
//                return outRect;
//            }

//            outRect.Left = child.Left;
//            outRect.Right = child.Right;
//            outRect.Top = child.Top;
//            outRect.Bottom = child.Bottom;

//            IViewParent parent = child.Parent;

//            while (parent is ViewGroup && parent != this)
//            {
//                ViewGroup group = (ViewGroup)parent;
//                outRect.Left += group.Left;
//                outRect.Right += group.Right;
//                outRect.Top += group.Top;
//                outRect.Bottom += group.Bottom;

//                parent = group.Parent;
//            }

//            return outRect;
//        }

//        bool PageLeft()
//        {
//            if (mCurItem > 0)
//            {
//                SetCurrentItem(mCurItem - 1, true);
//                return true;

//            }
//            return false;
//        }

//        bool PageRight()
//        {
//            if (mAdapter != null && mCurItem < (mAdapter.GetCount() - 1))
//            {
//                SetCurrentItem(mCurItem + 1, true);
//                return true;
//            }
//            return false;
//        }

//        /**
//         * We only want the current page that is being shown to be focusable.
//         */
//        public override void AddFocusables(IList<View> views, [GeneratedEnum] FocusSearchDirection direction, [GeneratedEnum] FocusablesFlags focusableMode)
//        {
//            int focusableCount = views.Count;

//            int descendantFocusability = GetDescendantFocusability();

//            if (descendantFocusability != FOCUS_BLOCK_DESCENDANTS)
//            {
//                for (int i = 0; i < ChildCount; i++)
//                {
//                    View child = GetChildAt(i);
//                    if (child.Visibility == ViewStates.Visible)
//                    {
//                        ItemInfo ii = InfoForChild(child);
//                        if (ii != null && ii.position == mCurItem)
//                        {
//                            child.AddFocusables(views, direction, focusableMode);
//                        }
//                    }
//                }
//            }

//            // we add ourselves (if focusable) in all cases except for when we are
//            // FOCUS_AFTER_DESCENDANTS and there are some descendants focusable. this is
//            // to avoid the focus search finding layouts when a more precise search
//            // among the focusable children would be more interesting.
//            if (descendantFocusability != FOCUS_AFTER_DESCENDANTS ||
//                    // No focusable descendants
//                    (focusableCount == views.Count))
//            {
//                // Note that we can't call the superclass here, because it will
//                // add all views in. So we need to do the same thing View does.
//                if (!IsFocusable)
//                {
//                    return;
//                }
//                if ((focusableMode & FocusablesFlags.TouchMode) == FocusablesFlags.TouchMode && IsInTouchMode && !isFocusableInTouchMode())
//                {
//                    return;
//                }
//                if (views != null)
//                {
//                    views.Add(this);
//                }
//            }
//        }

//        /**
//         * We only want the current page that is being shown to be touchable.
//         */
//        public override void AddTouchables(IList<View> views)
//        {
//            // Note that we don't call super.addTouchables(), which means that
//            // we don't call View.addTouchables(). This is okay because a ViewPager
//            // is itself not touchable.
//            for (int i = 0; i < ChildCount; i++)
//            {
//                View child = GetChildAt(i);
//                if (child.Visibility == ViewStates.Visible)
//                {
//                    ItemInfo ii = InfoForChild(child);
//                    if (ii != null && ii.position == mCurItem)
//                    {
//                        child.AddTouchables(views);
//                    }
//                }
//            }
//        }

//        /**
//         * We only want the current page that is being shown to be focusable.
//         */
//        protected override bool OnRequestFocusInDescendants(int direction, Rect previouslyFocusedRect)
//        {
//            int index;
//            int increment;
//            int end;
//            int count = ChildCount;
//            if (((FocusSearchDirection)direction & FocusSearchDirection.Forward) != 0)
//            {
//                index = 0;
//                increment = 1;
//                end = count;
//            }
//            else
//            {
//                index = count - 1;
//                increment = -1;
//                end = -1;
//            }
//            for (int i = index; i != end; i += increment)
//            {
//                View child = GetChildAt(i);
//                if (child.Visibility == ViewStates.Visible)
//                {
//                    ItemInfo ii = InfoForChild(child);
//                    if (ii != null && ii.position == mCurItem)
//                    {
//                        if (child.RequestFocus((FocusSearchDirection)direction, previouslyFocusedRect))
//                        {
//                            return true;
//                        }
//                    }
//                }
//            }
//            return false;
//        }

//        public override bool DispatchPopulateAccessibilityEvent(AccessibilityEvent @event)
//        {
//            // Dispatch scroll events from this ViewPager.
//            if (@event.EventType == EventTypes.ViewScrolled)
//            {
//                return base.DispatchPopulateAccessibilityEvent(@event);
//            }

//            // Dispatch all other accessibility events from the current page.
//            int childCount = ChildCount;

//            for (int i = 0; i<childCount; i++)
//            {
//                View child = GetChildAt(i);
//                if (child.Visibility == ViewStates.Visible)
//                {
//                    ItemInfo ii = InfoForChild(child);
//                    if (ii != null && ii.position == mCurItem && child.DispatchPopulateAccessibilityEvent(@event))
//                    {
//                        return true;
//                    }
//                }
//            }

//            return false;
//        }

//        protected override ViewGroup.LayoutParams GenerateLayoutParams(ViewGroup.LayoutParams p)
//        {
//            return GenerateDefaultLayoutParams();
//        }

//        protected override bool CheckLayoutParams(ViewGroup.LayoutParams p)
//        {
//            return p is LayoutParams && base.CheckLayoutParams(p);
//        }

//        public override ViewGroup.LayoutParams GenerateLayoutParams(IAttributeSet attrs)
//        {
//            return new LayoutParams(Context, attrs);
//        }

//        class MyAccessibilityDelegate : AccessibilityDelegate
//        {
//            public override void OnInitializeAccessibilityEvent(View host, AccessibilityEvent @event)
//            {
//                base.OnInitializeAccessibilityEvent(host, @event);
//                @event.ClassName = typeof(SdkCenteredViewPager).Name;

//                AccessibilityRecord recordCompat = AccessibilityRecord.Obtain();
//                recordCompat.Scrollable = CanScroll();

//                if (@event.EventType == EventTypes.ViewScrolled && mAdapter != null)
//                {
//                    recordCompat.ItemCount = mAdapter.getCount();
//                    recordCompat.FromIndex = mCurItem;
//                    recordCompat.ToIndex = mCurItem;
//                }
//            }

//            public override void OnInitializeAccessibilityNodeInfo(View host, AccessibilityNodeInfo info)
//            {
//                base.OnInitializeAccessibilityNodeInfo(host, info);
//                info.ClassName = typeof(SdkCenteredViewPager).Name;

//                info.Scrollable = CanScroll();

//                if (CanScrollHorizontally( 1 ) )
//                {
//                    info.AddAction(Android.Views.Accessibility.Action.ScrollForward);
//                }
//                if (CanScrollHorizontally( -1 ) )
//                {
//                    info.AddAction(Android.Views.Accessibility.Action.ScrollBackward);
//                }
//            }

//            public override bool PerformAccessibilityAction(View host, [GeneratedEnum] Android.Views.Accessibility.Action action, Bundle args)
//            {
//                if (base.PerformAccessibilityAction(host, action, args))
//                {
//                    return true;
//                }

//                switch (action)
//                {
//                    case Android.Views.Accessibility.Action.ScrollForward:
//                        {
//                            if (CanScrollHorizontally(1))
//                            {
//                                SetCurrentItem(mCurItem + 1);
//                                return true;
//                            }
//                        }
//                        return false;
//                    case Android.Views.Accessibility.Action.ScrollBackward:
//                        {
//                            if (CanScrollHorizontally(-1))
//                            {
//                                SetCurrentItem(mCurItem - 1);
//                                return true;
//                            }
//                        }
//                        return false;
//                }
//                return false;
//            }

//            private bool CanScroll()
//            {
//                return (mAdapter != null) && (mAdapter.GetCount() > 1);
//            }
//        }

//    public class PagerObserver : DataSetObserver
//    {
//        public override void OnChanged()
//        {
//            DataSetChanged();
//        }
//        public override void OnInvalidated()
//        {
//            DataSetChanged();
//        }
//    }

//        /**
//            * Layout parameters that should be supplied for views added to a ViewPager.
//            */
//        public class MyLayoutParams : ViewGroup.LayoutParams
//        {
//            /**
//                * true if this view is a decoration on the pager itself and not a view supplied by the adapter.
//                */
//            public bool IsDecor;

//            /**
//                * Gravity setting for use on decor views only: Where to position the view page within the overall ViewPager container; constants are defined in {@link android.view.Gravity}.
//                */
//            public GravityFlags Gravity;

//            /**
//                * Width as a 0-1 multiplier of the measured pager width
//                */
//            public float WidthFactor = 0.0f;

//            /**
//                * true if this view was added during layout and needs to be measured before being positioned.
//                */
//            public bool NeedsMeasure;

//            /**
//                * Adapter position this view is for if !isDecor
//                */
//            public int Position;

//            /**
//                * Current child index within the ViewPager that this view occupies
//                */
//            public int ChildIndex;

//            public MyLayoutParams() : base(MatchParent, MatchParent)
//            {

//            }

//            public MyLayoutParams(Context context, IAttributeSet attrs) : base(context, attrs)
//            {
//                TypedArray a = context.ObtainStyledAttributes(attrs, LAYOUT_ATTRS);
//                Gravity = GravityFlags.Top;
//                a.Recycle();
//            }
//        }

////        class ViewPositionComparator : Comparator<View>
////        {
////    @Override
////                public int compare(View lhs, View rhs)
////{
////    LayoutParams llp = (LayoutParams)lhs.LayoutParameters;
////    LayoutParams rlp = (LayoutParams)rhs.LayoutParameters;
////    if (llp.IsDecor != rlp.IsDecor)
////    {
////        return llp.IsDecor ? 1 : -1;
////    }
////    return llp.position - rlp.position;
////}
////            }

//    // Matt additions, not part of AOSP
//    float GetInterPagePadding()
//    {
//        return Width * interPagePadding;
//    }
//}