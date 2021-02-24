using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using Newtonsoft.Json;
namespace DaHua.ADRMS {

	public partial class AddUserWindow : Window {

		public delegate void GetFUserMailHander(List<UserInfo> userResults);
		public event GetFUserMailHander GetFUserMailEvenet;
		private List<UserInfo> userInfos = new List<UserInfo>();
		private List<UserInfo> userResults = new List<UserInfo>();
		public AddUserWindow() {
			InitializeComponent();
			this.Loaded += AddUserWindow_Loaded;
		}

		private void AddUserWindow_Loaded(object sender,RoutedEventArgs e) {

			this.SubmitImageBtn.Click += SubmitImageBtn_Click;
			this.SerachBtn.Click += SerachBtn_Click;

		}

		private void SerachBtn_Click(object sender,RoutedEventArgs e) {
			string fName = this.FName.Text;
			if(string.IsNullOrWhiteSpace(fName)) {
				return;
			}
            fName = fName.Replace("；", ";");
			LoadingState.Show();
			WebClient webclient = new WebClient();
			var stream = webclient.OpenRead("http://filetrans.dahuatech.com:8001/api/Member/Userinfo?userInfo=" + fName);
			System.IO.StreamReader _read = new System.IO.StreamReader(stream,System.Text.Encoding.UTF8);
			var str = _read.ReadToEnd();
			if(str == "\"0\"") {
				LoadingState.Hide();
                MessageBox.Show("可能原因：\r\n\r\na）员工不是公司在职员工；\r\n\r\nb）分隔符输入错误；", "没有查询到此账号信息。", MessageBoxButton.OK, MessageBoxImage.Warning);
				//this.FName.Text = "";
				return;
			}
			//foreach(var item in userInfos) {
			//	userInfos.Remove(item);
			//}
			userInfos = JsonConvert.DeserializeObject<List<UserInfo>>(str);
			if(userInfos.Any()) {
				LoadingState.Hide();
				SearchUserList.ItemsSource = null;
				SearchUserList.ItemsSource = userInfos;
				return;
			}
			LoadingState.Hide();
		}

		private void SubmitImageBtn_Click(object sender,RoutedEventArgs e) {

			if(this.GetFUserMailEvenet == null) {
				return;
			}
			if(!userResults.Any())
			{
				MessageBox.Show("请选择一条用户，再确认，谢谢!");
				return;
			}
			this.GetFUserMailEvenet(userResults);
			this.Close();
		}


		private bool CheckEmail(string email) {
			string pattern =
				@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
			return IsMatch(pattern,email);
		}
		private static bool IsMatch(string pattern,string input) {
			if(string.IsNullOrEmpty(input)) {
				return false;
			}
			var regex = new Regex(pattern);
			return regex.IsMatch(input);
		}
		private void HandleDoubleClick(object sender,MouseButtonEventArgs e) {
			object o = SearchUserList.SelectedItem;
			if(o == null)
				return;
			UserInfo item = o as UserInfo;
			if(item == null) {
				return;
			}
			if(item.FID.ToLower() == ApplicationContext.UserName.ToLower())
			{
				MessageBox.Show("不支持对自己进行加密!");
				return;
			}
			if(!userResults.Any(p => p.FID == item.FID)) {

			
			 
				userResults.Add(item);
			}
			ResultUserList.ItemsSource = null;
			ResultUserList.ItemsSource = userResults;
			

		}
		private void HandleDoubleRemoveClick(object sender,MouseButtonEventArgs e) {
			object o = ResultUserList.SelectedItem;
			if(o == null)
				return;
			UserInfo item = o as UserInfo;
			if(item == null) {
				return;
			}

				userResults.Remove(item);
		
			ResultUserList.ItemsSource = null;
			ResultUserList.ItemsSource = userResults;
		}
    }
}
