using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DaHua.ADRMS {
	/// <summary>
	/// Yuhao 2015.10.13 add
	/// </summary>
	public partial class Login : Window {
		public Login() {
			this.Activate();
			InitializeComponent();
			this.Loaded += Login_Loaded;
		}
		private bool CheckValid() {
			var result = !String.IsNullOrEmpty(UserName) && !String.IsNullOrEmpty(Password);
			return result;
		}

		private void Login_Loaded(object sender,RoutedEventArgs e) {
			this.UsernameBox.KeyUp += UsernameBox_KeyUp;
			this.PasswordBox.KeyUp += PasswordBox_KeyUp;
			this.ButtonLogin.Click += ButtonLogin_Click;
		
		
		}

		private void ButtonLogin_Click(object sender,RoutedEventArgs e) {
			CheckLogin();
		}

		private void PasswordBox_KeyUp(object sender,KeyEventArgs e) {
			if(e.Key == Key.Enter) {
				CheckLogin();
			}
		}
		private void CheckLogin() {
		   if(CheckValid()) {
				Status.Visibility = Visibility.Visible;
				using(var deUser = new DirectoryEntry(@"LDAP://DaHuaTech.com",UserName,Password)) {
					try {
						var src = new DirectorySearcher(deUser) {
							Filter =
								"(&(&(objectCategory=person)(objectClass=user))(sAMAccountName="
								+ UserName + "))"
						};
						src.PropertiesToLoad.Add("cn");
						src.SearchRoot = deUser;
						src.SearchScope = SearchScope.Subtree;
						SearchResult result = src.FindOne();
						if(result != null) {

							AppDomain.CurrentDomain.SetData("RmsUserId",UserName);
							MainWindow windows = new MainWindow();
							windows.Show();
							this.Close();	
						}
					} catch (Exception e){
						Status.Visibility = Visibility.Hidden;
						MessageBox.Show("帐号密码错误!");
						return;
					}
				}
			} else {
				Status.Visibility = Visibility.Hidden;
				MessageBox.Show("请输入帐号和密码!");
				return;
		   }
		}

		private void UsernameBox_KeyUp(object sender,KeyEventArgs e) {
			if(e.Key == Key.Enter) {
				PasswordBox.Focus();
			}
		}

		public string UserName {
			get {
				return this.UsernameBox.Text.Trim();
			}
			set {
				this.UsernameBox.Text = value;
			}
		}
		public string Password {
			get {
				return this.PasswordBox.Password.Trim();
			}
			set {
				this.PasswordBox.Password = value;
			}
		}
	}
}
