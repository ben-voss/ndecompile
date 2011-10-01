using System;
using System.Security.Permissions;
using System.Windows.Forms;

namespace LittleNet.NDecompile.FormsUI.Views
{
	internal class CodeBrowser : WebBrowser
	{
		public class NavigateEventArgs : EventArgs
		{
			private readonly String _uri;

			private bool _cancel;

			public NavigateEventArgs(String uri)
			{
				_uri = uri;
			}

			public bool Cancel
			{
				get
				{
					return _cancel;
				}
				set
				{
					_cancel = value;
				}
			}

			public String Uri
			{
				get
				{
					return _uri;
				}
			}
		}

		//This class will capture _events from the WebBrowser
		private class WebBrowserExtendedEvents : UnsafeNativeMethods.DWebBrowserEvents2
		{
			private readonly CodeBrowser _browser;

			public WebBrowserExtendedEvents(CodeBrowser browser)
			{
				_browser = browser;
			}

			public void StatusTextChange (string text)
			{
			}

			public void ProgressChange (int progress, int progressMax)
			{
			}

			public void CommandStateChange (int command, bool bnable)
			{
			}

			public void DownloadBegin()
			{
			}

			public void DownloadComplete()
			{
			}

			public void TitleChange (string text)
			{
			}

			public void PropertyChange (string szProperty)
			{
			}

			public void BeforeNavigate2 (object pDisp, ref object url, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel)
			{
				NavigateEventArgs args = new NavigateEventArgs(url.ToString());
				_browser.OnBeforeNavigate(args);
				cancel = args.Cancel;
			}

			public void NewWindow2 (ref object ppDisp, ref bool cancel)
			{
			}

			public void NavigateComplete2 (object pDisp, ref object url)
			{
			}

			public void DocumentComplete (object pDisp, ref object url)
			{
			}

			public void OnQuit ()
			{
			}

			public void OnVisible (bool visible)
			{
			}

			public void OnToolBar (bool toolBar)
			{
			}

			public void OnMenuBar (bool menuBar)
			{
			}

			public void OnStatusBar (bool statusBar)
			{
			}

			public void OnFullScreen (bool fullScreen)
			{
			}

			public void OnTheaterMode (bool theaterMode)
			{
			}

			public void WindowSetResizable (bool resizable)
			{
			}

			public void WindowSetLeft (int left)
			{
			}

			public void WindowSetTop (int top)
			{
			}

			public void WindowSetWidth (int width)
			{
			}

			public void WindowSetHeight (int height)
			{
			}

			public void WindowClosing (bool isChildWindow, ref bool cancel)
			{
			}

			public void ClientToHostWindow (ref int cx, ref int cy)
			{
			}

			public void SetSecureLockIcon (int secureLockIcon)
			{
			}

			public void FileDownload (ref bool cancel)
			{
			}

			public void NavigateError (object pDisp, ref object url, ref object frame, ref object statusCode, ref bool cancel)
			{
			}

			public void PrintTemplateInstantiation (object pDisp)
			{
			}

			public void PrintTemplateTeardown (object pDisp)
			{
			}

			public void UpdatePageStatus (object pDisp, ref object nPage, ref object fDone)
			{
			}

			public void PrivacyImpactedStateChange (bool bImpacted)
			{
			}

			public void NewWindow3 (ref object ppDisp, ref bool cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
			{
			}
		}

		private AxHost.ConnectionPointCookie _cookie;
		private WebBrowserExtendedEvents _events;

		/// <summary>
		/// This method will be called to give
		/// you a chance to create your own event sink
		/// </summary>
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		protected override void CreateSink()
		{
			// Make sure to call the base class or the normal _events won't fire
			base.CreateSink();
			_events = new WebBrowserExtendedEvents(this);
			_cookie = new AxHost.ConnectionPointCookie(ActiveXInstance,
					 _events, typeof(UnsafeNativeMethods.DWebBrowserEvents2));
		}

		/// <summary>
		/// Detaches the event sink
		/// </summary>
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		protected override void DetachSink()
		{
			if (null != _cookie)
			{
				_cookie.Disconnect();
				_cookie = null;
			}
		}

		protected void OnBeforeNavigate(NavigateEventArgs args)
		{
			if (BeforeNavigate != null)
				BeforeNavigate(this, args);
		}

		public event EventHandler<NavigateEventArgs> BeforeNavigate;
	}
}
