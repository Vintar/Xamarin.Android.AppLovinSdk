using Com.Applovin.Adview;
using Java.Interop;
using System;

namespace Com.Applovin.Impl.Adview
{
	public partial class AdViewControllerImpl
	{
		public void SetAdViewEventListener(IAppLovinAdViewEventListener p0)
		{
			AdViewEventListener = p0;
		}
	}
}

namespace Com.Applovin.Impl.Sdk
{
	public partial class AppLovinAdServiceImpl
	{
		public virtual unsafe void RemoveAdUpdateListener(global::Com.Applovin.Sdk.IAppLovinAdUpdateListener p0, global::Com.Applovin.Sdk.AppLovinAdSize p1 = null)
		{
			const string __id = "removeAdUpdateListener.(Lcom/applovin/sdk/AppLovinAdUpdateListener;Lcom/applovin/sdk/AppLovinAdSize;)V";
			try
			{
				JniArgumentValue* __args = stackalloc JniArgumentValue[2];
				__args[0] = new JniArgumentValue((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object)p0).Handle);
				__args[1] = new JniArgumentValue((p1 == null) ? IntPtr.Zero : ((global::Java.Lang.Object)p1).Handle);
				_members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, __args);
			}
			finally
			{
			}
		}
	}
}