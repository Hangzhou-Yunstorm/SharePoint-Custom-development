using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace DaHua.ADRMS {
	public class LoadingState {

		#region Static Fields

		private static ManualResetEvent resetSplashCreated;

		#endregion

		#region Properties

		internal static Thread SplashThread { get; set; }

		internal static LoadingWindow LoadingWindow { get; set; }

		#endregion

		//#endregion

		//#region Public Methods and Operators

		#region Public Methods and Operators

		public static void Hide() {
			LoadingWindow.Dispatcher.BeginInvoke(DispatcherPriority.Send,new Action(() => LoadingWindow.Close()));
		}

		public static void Show() {
			resetSplashCreated = new ManualResetEvent(false);
			SplashThread = new Thread(CreateCounterWindowThread);
			SplashThread.SetApartmentState(ApartmentState.STA);
			SplashThread.IsBackground = true;
			SplashThread.Start();
			resetSplashCreated.WaitOne();
		}

		#endregion

		#region Methods

		private static void CreateCounterWindowThread() {
			LoadingWindow = new LoadingWindow();
			LoadingWindow.Show();
			resetSplashCreated.Set();
			Dispatcher.Run();
		}

		#endregion
	}
}
