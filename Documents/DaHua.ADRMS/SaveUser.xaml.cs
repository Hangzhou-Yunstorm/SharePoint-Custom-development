using System;
using System.Collections.Generic;
using System.IO;
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
using Newtonsoft.Json;
namespace DaHua.ADRMS {
	/// <summary>
	/// SaveUser.xaml 的交互逻辑
	/// </summary>
	public partial class SaveUser : Window {
		private List<UserModel> userModels;
		private FileModel fileModel;
		public SaveUser(List<UserModel> userModels,FileModel fileModel) {
			this.userModels = userModels;
			this.fileModel = fileModel;
			InitializeComponent();
			this.Loaded += SaveUser_Loaded;			
		}
		private void SaveUser_Loaded(object sender,RoutedEventArgs e) {
			this.BtnSave.Click += BtnSave_Click;
		}

		private void BtnSave_Click(object sender,RoutedEventArgs e) {
			var caseName = FCaseName.Text;
			if(string.IsNullOrEmpty(caseName))
			{
				MessageBox.Show("请输入方案名称");
				return;
			}
			if(!userModels.Any()) {
				MessageBox.Show("当前方案还没有人员!");
				return;
			}
			CaseModel caseModel = new CaseModel();
			caseModel.Id = Guid.NewGuid();
			caseModel.FCaseName = caseName;
			caseModel.UserModels = userModels;
			caseModel.FIsFullControl = fileModel.FIsFullControl;
			caseModel.FIsPrint = fileModel.FIsPrint;
			caseModel.FIsRead = fileModel.FIsRead;
			caseModel.FIsSave = fileModel.FIsSave;
			caseModel.FIsWrite = fileModel.FIsWrite;
			var filePath = PathUtility.CaseJosnPath;
			try {
				var json = JsonConvert.SerializeObject(caseModel);
				File.WriteAllText(filePath,json);
				MessageBox.Show("保存成功!");
				this.Close();
			} catch {
				MessageBox.Show("保存错误,请稍候重试!");
				return;
			}
		

		}
	}
}
