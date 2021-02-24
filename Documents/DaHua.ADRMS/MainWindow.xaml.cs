using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Office.Core;
using Newtonsoft.Json;
using System.IO;
namespace DaHua.ADRMS {

	public partial class MainWindow : System.Windows.Window {
		private FileModel fileModel = new FileModel();
		private List<CaseModel> caseModels = new List<CaseModel>();
		private List<UserModel> userModels = new List<UserModel>();
		private List<HistoryModel> historyModels = new List<HistoryModel>();
		public MainWindow() {
			this.Loaded += MainWindow_Loaded;
			InitializeComponent();
			RefreshSelect();
		}

		private void MainWindow_Loaded(object sender,RoutedEventArgs e) {
			this.BtnSelectFile.Click += BtnSelectFile_Click;
			this.BtnSelectFolder.Click += BtnSelectFolder_Click;
			this.StartEncrypt.Click += StartEncrypt_Click;
			this.BtnUserSelect.Click += BtnUserSelect_Click;
			this.SaveUserCase.Click += SaveUserCase_Click;
            this.BtnAllUserSelect.Click += BtnAllUserSelect_Click;
		}
		public void CaseList_SelectionChanged(object sender,System.Windows.Controls.SelectionChangedEventArgs e) {

			var index = CaseList.SelectedIndex;
			if(index == 0 || index==-1) {
				return;
			}
			var item = CaseList.SelectedItem as CaseModel;
			UserList.ItemsSource = null;
		
			userModels = item.UserModels;
            UserList.ItemsSource = userModels;
			if(item.FIsFullControl) {
				ck.IsChecked = true;
			} else {
				ck.IsChecked = false;
			}
			if(item.FIsRead) {
				ck1.IsChecked = true;
			}else {
				ck1.IsChecked = false;
			}
			if(item.FIsPrint) {
				ck2.IsChecked = true;
				
			}else {
				ck2.IsChecked = false;
			}
			if(item.FIsSave) {
				ck3.IsChecked = true;
			}else {
				ck3.IsChecked = false;
			}
			if(item.FIsWrite) {
				ck4.IsChecked = true;
			}else {
				ck4.IsChecked = false;
			}
		}
		private void SaveUserCase_Click(object sender,RoutedEventArgs e) {
			if(!userModels.Any()) {
				MessageBox.Show("还没选择人员");
				return;
			}
			fileModel.FIsFullControl = ck.IsChecked ?? false;
			fileModel.FIsRead = ck1.IsChecked ?? false;
			fileModel.FIsPrint = ck2.IsChecked ?? false;
			fileModel.FIsSave = ck3.IsChecked ?? false;
			fileModel.FIsWrite = ck4.IsChecked ?? false;
			if(!fileModel.FIsFullControl && !fileModel.FIsRead && !fileModel.FIsPrint && !fileModel.FIsSave && !fileModel.FIsWrite) {
				MessageBox.Show("请至少选择一个加密权限");
				return;
			}
			SaveUser user = new SaveUser(userModels,fileModel);
			user.ShowDialog();
			RefreshSelect();//刷新方案
		}

        private void BtnAllUserSelect_Click(object sender, RoutedEventArgs e)
        {
            UserModel model = new UserModel();           
            model.FID = "Everyone";
            model.FDeptName = "Everyone";
            model.FMail = "Everyone";
            model.FUserName = "Everyone";
            userModels.Add(model);
            UserList.ItemsSource = null;
            UserList.ItemsSource = userModels;
        }

        private void RefreshSelect() {

			caseModels.Clear();

			CaseModel firstModel = new CaseModel();
			firstModel.FCaseName = "--选择方案--";
			firstModel.Id = Guid.NewGuid();
			caseModels.Add(firstModel);
			CaseList.ItemsSource = null;
			var storagePath = PathUtility.BaseStoragePath;

			var directories = IOUtility.EnumerateDirectoriesExludeHidden(storagePath);
			if(!directories.Any()) {
				return;
			}
			foreach(var dir in directories) {
				var caseJsonFile = System.IO.Path.Combine(dir.FullName,PathUtility.CaseJsonFileName);
				if(!File.Exists(caseJsonFile)) {
					return;
				}
				using(var stream = new StreamReader(caseJsonFile)) {
					var str = stream.ReadToEnd();
					var caseModel = JsonConvert.DeserializeObject<CaseModel>(str);
					caseModels.Add(caseModel);
				}
			}         
            
            CaseList.ItemsSource = caseModels;
			CaseList.SelectedValuePath = "Id";
			//CaseList.DisplayMemberPath = "FCaseName";
			CaseList.SelectedIndex = 0;

		}

        private void Combo_Item_Loaded(object sender, RoutedEventArgs e)
        {
            var target = sender as DockPanel;
            var template = target.TemplatedParent as FrameworkElement;
            var model = template.DataContext as CaseModel;
            if(model.FCaseName== "--选择方案--")
            {
                var btn_del= target.FindName("btn_del") as UIElement;
                target.Children.Remove(btn_del);
            }

        }

        private void BtnDelCase_Click(object sender, RoutedEventArgs e)
        {
            var target = sender as Button;
            var template = target.TemplatedParent as FrameworkElement;
            var model = template.DataContext as CaseModel;
            if(MessageBox.Show("确认删除该方案吗?", "此删除不可恢复", MessageBoxButton.OKCancel)==MessageBoxResult.OK)
            {
                var storagePath = PathUtility.BaseStoragePath;
                var directories = IOUtility.EnumerateDirectoriesExludeHidden(storagePath);
                if (!directories.Any())
                {
                    return;
                }
                foreach (var dir in directories)
                {
                    var caseJsonFile = System.IO.Path.Combine(dir.FullName, PathUtility.CaseJsonFileName);
                    if (!File.Exists(caseJsonFile))
                    {
                        return;
                    }
                    var caseModel = new CaseModel();
                    using (var stream = new StreamReader(caseJsonFile))
                    {
                        var str = stream.ReadToEnd();
                        caseModel = JsonConvert.DeserializeObject<CaseModel>(str);                                                
                    }
                    if (caseModel.Id == model.Id)
                    {
                        try
                        {
                            File.Delete(caseJsonFile);
                            dir.Delete();
                        }
                        catch (Exception message)
                        {
                            MessageBox.Show("删除失败！\r\n原因：" + message.Message);
                        }
                        RefreshSelect();
                        return;
                    }
                }

            }

        }

		private void BtnUserSelect_Click(object sender,RoutedEventArgs e) {
			AddUserWindow addUserFmail = new AddUserWindow();

			addUserFmail.GetFUserMailEvenet += this.SetFUserMailText;
			addUserFmail.ShowDialog();

		}
		private void SetFUserMailText(List<UserInfo> userResults) {

			foreach(var item in userResults) {
				if(!userModels.Any(p => p.FID == item.FID)) {
					UserModel model = new UserModel();
					model.FID = item.FID;
					model.FDeptName = item.FDeptName;
					model.FMail = item.EMail;
					model.FUserName = item.FItemName;
					userModels.Add(model);
				}
			}
			UserList.ItemsSource = null;
			UserList.ItemsSource = userModels;

		}

        /// <summary>
        /// 开始加密
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void StartEncrypt_Click(object sender,RoutedEventArgs e) {

		try {
				var checkBool = Check();
				if(!checkBool) {
					return;
				}
				var checkBoxBool = CheckBoxSelect();
				if(!checkBoxBool) {
					return;
				}
				if(!userModels.Any()) {
					MessageBox.Show("请添加要加密的人员!");
					return;
				}
				LoadingState.Show();
				var filePaths = this.FileNameText.Text.Split(';');
				foreach(var filePath in filePaths) {
					var filePathToLower = filePath.ToLower();
					if(filePathToLower.Contains(".doc") || filePathToLower.Contains(".docx")) {
						EncryptWord(filePath);
					}
					if(filePathToLower.Contains(".xls") || filePathToLower.Contains(".xlsx")) {
						EncryptExecl(filePath);
					}
				}
				this.FileNameText.Text = "";
				LoadingState.Hide();
				MessageBox.Show("批量加密成功！");
			}catch {

			
			}
		
		}

        /// <summary>
        /// 加密word
        /// </summary>
        /// <param name="filePath"></param>
		private void EncryptWord(string filePath) {
			try {
				var wordClass = new Microsoft.Office.Interop.Word.ApplicationClass();
				Microsoft.Office.Interop.Word.Document mydoc = wordClass.Documents.Open(filePath);
				DateTime exprieDateTime = new DateTime(2029,5,2);
				if(mydoc != null) {
					mydoc.Activate();
					var irmPremission = mydoc.Permission;
					if(irmPremission.Enabled == true) {
						MessageBox.Show("当前文档已经加密，操作失败!");
						return;
					}
					mydoc.Application.Visible = false;
					irmPremission.Enabled = true;

					foreach(var item in userModels) {
						if(fileModel.FIsFullControl) {
							irmPremission.Add(item.FMail,MsoPermission.msoPermissionFullControl,exprieDateTime);
						}
						if(fileModel.FIsPrint) {

							irmPremission.Add(item.FMail,MsoPermission.msoPermissionPrint,exprieDateTime);
						}
						if(fileModel.FIsRead) {
							irmPremission.Add(item.FMail,MsoPermission.msoPermissionRead,exprieDateTime);
						}
						if(fileModel.FIsSave) {
							irmPremission.Add(item.FMail,MsoPermission.msoPermissionSave,exprieDateTime);
						}
						if(fileModel.FIsWrite) {
							irmPremission.Add(item.FMail,MsoPermission.msoPermissionEdit,exprieDateTime);
						}
					}

					mydoc.SaveAs(fileModel.FFolderName + "\\" + System.IO.Path.GetFileName(filePath));
					mydoc.Close();
				}
				wordClass.Quit();

				HistoryModel hmodel = new HistoryModel();
				hmodel.FFileName = filePath;
				hmodel.CreateTime = DateTime.Now;
				hmodel.Status = "执行成功";
				OperateList.ItemsSource = null;
				historyModels.Add(hmodel);
				OperateList.ItemsSource = historyModels;
			} catch(Exception ex) {
				HistoryModel hmodel = new HistoryModel();
				hmodel.FFileName = filePath;
				hmodel.CreateTime = DateTime.Now;
				hmodel.Status = "执行失败:" + ex.InnerException.Message.ToString();
				historyModels.Add(hmodel);
				OperateList.ItemsSource = null;
				OperateList.ItemsSource = historyModels;

			}
		}

        /// <summary>
        /// 加密Excel
        /// </summary>
        /// <param name="filePath"></param>
		private void EncryptExecl(string filePath) {
			try {
				var execlClass = new Microsoft.Office.Interop.Excel.ApplicationClass();
				var myExecl = execlClass.Workbooks.Open(filePath);
				DateTime exprieDateTime = new DateTime(2029,5,2);
				if(myExecl != null) {
					myExecl.Activate();
					var irmPremission = myExecl.Permission;                   
                    if (irmPremission.Enabled == true) {
						MessageBox.Show("当前文档已经加密，操作失败!");                       
                        return;
					}
					myExecl.Application.Visible = false;
					irmPremission.Enabled = true;

					foreach(var item in userModels) {
						if(fileModel.FIsFullControl) {
							irmPremission.Add(item.FMail,MsoPermission.msoPermissionFullControl,exprieDateTime);
						}
						if(fileModel.FIsPrint) {

							irmPremission.Add(item.FMail,MsoPermission.msoPermissionPrint,exprieDateTime);
						}
						if(fileModel.FIsRead) {
							irmPremission.Add(item.FMail,MsoPermission.msoPermissionRead,exprieDateTime);
						}
						if(fileModel.FIsSave) {
							irmPremission.Add(item.FMail,MsoPermission.msoPermissionSave,exprieDateTime);
						}
						if(fileModel.FIsWrite) {
							irmPremission.Add(item.FMail,MsoPermission.msoPermissionEdit,exprieDateTime);
						}
					}

					myExecl.SaveAs(fileModel.FFolderName + "\\" + System.IO.Path.GetFileName(filePath));
					myExecl.Close();
				}
				execlClass.Quit();

				HistoryModel hmodel = new HistoryModel();
				hmodel.FFileName = filePath;
				hmodel.CreateTime = DateTime.Now;
				hmodel.Status = "执行成功";
				OperateList.ItemsSource = null;
				historyModels.Add(hmodel);
				OperateList.ItemsSource = historyModels;
			} catch(Exception ex) {
				HistoryModel hmodel = new HistoryModel();
				hmodel.FFileName = filePath;
				hmodel.CreateTime = DateTime.Now;
				hmodel.Status = "执行失败:" + ex.InnerException.Message.ToString();
				historyModels.Add(hmodel);
				OperateList.ItemsSource = null;
				OperateList.ItemsSource = historyModels;

			}           
		}

        /// <summary>
        /// 判断信息是否完整
        /// </summary>
        /// <returns></returns>
		private bool Check() {
			if(string.IsNullOrWhiteSpace(FileNameText.Text)) {
				MessageBox.Show("请选择文件");
				return false;
			}
			fileModel.FFileName = FileNameText.Text.Trim();
			if(string.IsNullOrWhiteSpace(FolderNameText.Text)) {
				MessageBox.Show("请选择保存目录");
				return false;
			}
			fileModel.FFolderName = FolderNameText.Text.Trim();
			return true;
		}

        /// <summary>
        /// 判断是否选择了加密策略
        /// </summary>
        /// <returns></returns>
		private bool CheckBoxSelect() {
			fileModel.FIsFullControl = ck.IsChecked ?? false;
			fileModel.FIsRead = ck1.IsChecked ?? false;
			fileModel.FIsPrint = ck2.IsChecked ?? false;
			fileModel.FIsSave = ck3.IsChecked ?? false;
			fileModel.FIsWrite = ck4.IsChecked ?? false;
			if(!fileModel.FIsFullControl && !fileModel.FIsRead && !fileModel.FIsPrint && !fileModel.FIsSave && !fileModel.FIsWrite) {
				MessageBox.Show("请选择一个RMS权限，谢谢!");
				return false;
			}
			return true;
		}

		private void BtnSelectFolder_Click(object sender,RoutedEventArgs e) {


			System.Windows.Forms.FolderBrowserDialog m_Dialog = new System.Windows.Forms.FolderBrowserDialog();
			System.Windows.Forms.DialogResult result = m_Dialog.ShowDialog();

			if(result == System.Windows.Forms.DialogResult.Cancel) {
				return;
			}
			this.FolderNameText.Text = m_Dialog.SelectedPath.Trim();
		}

		private void BtnSelectFile_Click(object sender,RoutedEventArgs e) {
			var op = new Microsoft.Win32.OpenFileDialog { Multiselect = true,RestoreDirectory = true,Filter = "Word/Execl|*.xls;*.xlsx;*.docx;*.doc;",FilterIndex = 2 };

			Stream mysteam;
			var fileNames = "";
			if(op.ShowDialog() == true) {
                try {
                    if ((mysteam = op.OpenFile()) != null) {
                        for (int i = 0; i < op.FileNames.Length; i++) {
                            if (string.IsNullOrEmpty(fileNames)) {
                                fileNames = op.FileNames[i];
                            } else {
                                fileNames = fileNames + ";" + op.FileNames[i];
                            }
                        }
                        mysteam.Close();
                    }
                }
                catch
                {
                    MessageBox.Show("请确定您选择的文件没有被打开或者被其它软件所使用！", "错误提示信息", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
			}


			this.FileNameText.Text = fileNames;
		}

		private void HandleDoubleRemoveClick(object sender,MouseButtonEventArgs e) {
			object o = UserList.SelectedItem;
			if(o == null)
				return;
			UserModel item = o as UserModel;
			if(item == null) {
				return;
			}


			userModels.Remove(item);

			UserList.ItemsSource = null;
			UserList.ItemsSource = userModels;
		}


	}
}
