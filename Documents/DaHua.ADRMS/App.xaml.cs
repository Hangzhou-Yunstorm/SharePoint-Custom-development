using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DaHua.ADRMS {
	/// <summary>
	/// App.xaml 的交互逻辑
	/// </summary>
	public partial class App : Application {
		/// <summary>
		/// 窗口移动
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UIElement_OnMouseDown(object sender,MouseButtonEventArgs e) {
			if(e.LeftButton == MouseButtonState.Pressed) {
				Window win = (Window)((FrameworkElement)sender).TemplatedParent;
				win.DragMove();
			}
		}
		 /// <summary>
		 /// 窗口关闭
		 /// </summary>
		 /// <param name="sender"></param>
		 /// <param name="e"></param>
		private void BtnClose_OnClick(object sender,RoutedEventArgs e) {
			Window win = (Window)((FrameworkElement)sender).TemplatedParent;
			win.Close();
		}


		 /// <summary>
		 ///  窗口关闭
		 /// </summary>
		 /// <param name="sender"></param>
		 /// <param name="e"></param>
		private void BtnMin_OnClick(object sender,RoutedEventArgs e) {
			Window win = (Window)((FrameworkElement)sender).TemplatedParent;
			win.WindowState = WindowState.Minimized;
		}
	}
}
