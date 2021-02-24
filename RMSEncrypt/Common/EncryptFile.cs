using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Common
{
    public class EncryptFile
    {
        /// <summary>
        /// Office文件打开等待时间（毫秒）
        /// </summary>
        static int SleepTime = int.Parse(ConfigurationSettings.AppSettings["SleepTime"]);

        /// <summary>
        /// 加密Word
        /// </summary>
        /// <param name="RMSFileModel">文件属性</param>
        /// <param name="path">文件路径</param>
        /// <returns>加密结果</returns>
        public static bool EncryptWord(RMSFileModel filemodel, string path)
        {
            var wordClass = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document mydoc = null;
            try
            {
                mydoc = wordClass.Documents.Open(filemodel.FSourcePath);
                Thread.Sleep(SleepTime);
                DateTime exprieDateTime = DateTime.Now.AddYears(50);
                if (mydoc != null)
                {
                    mydoc.Activate();
                    var irmPremission = mydoc.Permission;
                    if (irmPremission.Enabled == true)
                    {
                        filemodel.FErrorMessage = "加密已加密的Word时错误";
                        filemodel.FContent = "当前文档已经加密，操作失败!";
                        filemodel.IsFalse = true;
                        MailHelper mail = MailHelper.GetMailHelper(filemodel.AuthorEmail, filemodel.Author, filemodel.FTitle, filemodel.UploadTime, "File was already encrypted by using RMS Protocol before, do not support");
                        mail.Send();

                        try
                        {
                            mydoc.Save();
                        }
                        catch { }
                        
                        return false;
                    }
                    mydoc.Application.Visible = false;
                    irmPremission.Enabled = true;
                    int MsoPermissionValue = 0;
                    if (filemodel.Users != null && filemodel.Users.Length > 0)
                    {
                        foreach (var item in filemodel.Users)
                        {
                            string value = item.LookupValue;
                            string email = item.Email;
                            if (!String.IsNullOrEmpty(value) && (value == "每个人" || value.ToLower() == "everyone"))
                            {
                                email = "Everyone";
                            }
                            if (!String.IsNullOrEmpty(Helper.ObjIsEmail(email)))
                            {

                                if (filemodel.FIsRead)
                                {
                                    MsoPermissionValue += MsoPermission.msoPermissionRead.GetHashCode();
                                    //MsoPermissionValue = MsoPermissionValue + MsoPermission.msoPermissionRead.();
                                    //irmPremission.Add(email, MsoPermission.msoPermissionRead, exprieDateTime);
                                }
                                if (filemodel.FIsPrint)
                                {
                                    MsoPermissionValue += MsoPermission.msoPermissionPrint.GetHashCode();
                                    //irmPremission.Add(email, MsoPermission.msoPermissionPrint, exprieDateTime);
                                }
                                if (filemodel.FIsEdit)
                                {
                                    MsoPermissionValue += MsoPermission.msoPermissionEdit.GetHashCode();
                                    MsoPermissionValue += MsoPermission.msoPermissionExtract.GetHashCode();
                                    //irmPremission.Add(email, MsoPermission.msoPermissionEdit, exprieDateTime);
                                }
                                if (filemodel.FIsSave)
                                {
                                    MsoPermissionValue += MsoPermission.msoPermissionSave.GetHashCode();
                                    //irmPremission.Add(email, MsoPermission.msoPermissionSave, exprieDateTime);
                                }
                                if (filemodel.FIsFullControl)
                                {
                                    MsoPermissionValue += MsoPermission.msoPermissionFullControl.GetHashCode();
                                    //irmPremission.Add(email, MsoPermission.msoPermissionFullControl, exprieDateTime);
                                }
                                irmPremission.Add(email, MsoPermissionValue, exprieDateTime);
                            }
                        }
                    }
                    else
                    {
                        if (filemodel.FIsRead)
                        {
                            MsoPermissionValue += MsoPermission.msoPermissionRead.GetHashCode();
                            //irmPremission.Add("Everyone", MsoPermission.msoPermissionRead, exprieDateTime);
                        }
                        if (filemodel.FIsPrint)
                        {
                            MsoPermissionValue += MsoPermission.msoPermissionPrint.GetHashCode();
                            //irmPremission.Add("Everyone", MsoPermission.msoPermissionPrint, exprieDateTime);
                        }
                        if (filemodel.FIsEdit)
                        {
                            MsoPermissionValue += MsoPermission.msoPermissionEdit.GetHashCode();
                            MsoPermissionValue += MsoPermission.msoPermissionExtract.GetHashCode();
                            //irmPremission.Add("Everyone", MsoPermission.msoPermissionEdit, exprieDateTime);
                        }
                        if (filemodel.FIsSave)
                        {
                            MsoPermissionValue += MsoPermission.msoPermissionSave.GetHashCode();
                            //irmPremission.Add("Everyone", MsoPermission.msoPermissionSave, exprieDateTime);
                        }
                        if (filemodel.FIsFullControl)
                        {
                            MsoPermissionValue += MsoPermission.msoPermissionFullControl.GetHashCode();
                            //irmPremission.Add("Everyone", MsoPermission.msoPermissionFullControl, exprieDateTime);
                        }
                        irmPremission.Add("Everyone", MsoPermissionValue, exprieDateTime);
                    }
                }

                string tempPath = path + filemodel.FTitle;
                mydoc.SaveAs(tempPath);
                System.IO.File.Delete(tempPath);
                
                filemodel.FRMSPath = tempPath;
                filemodel.FContent = "加密成功";
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("permission") || ex.Message.Contains("权限"))
                {
                    filemodel.FErrorMessage = "加密Word时错误，文件已经过RMS加密";
                    filemodel.FContent = ex.Message;

                    filemodel.IsFalse = true;
                    MailHelper mail = MailHelper.GetMailHelper(filemodel.AuthorEmail, filemodel.Author, filemodel.FTitle, filemodel.UploadTime, "File was already encrypted by using RMS Protocol before, do not support");
                    mail.Send();

                    try
                    {
                        mydoc.Save();
                    }
                    catch { }

                    return false;
                }
                else
                {
                    filemodel.FErrorMessage = "加密Word时错误";
                    filemodel.FContent = ex.Message;

                    filemodel.IsFalse = false;
                    //MailHelper mail = MailHelper.GetMailHelper(filemodel.AuthorEmail, filemodel.Author, filemodel.FTitle, filemodel.UploadTime, ex.Message);
                    //mail.Send();

                    try
                    {
                        mydoc.Save();
                    }
                    catch { }


                    return false;

                }
            }
            finally
            {
                if (mydoc != null)
                    mydoc.Close();
                wordClass.Quit();
            }
        }

        /// <summary>
        /// 加密Excel
        /// </summary>
        /// <param name="RMSFileModel">文件属性</param>
        /// <param name="path">文件路径</param>
        /// <returns>加密结果</returns>
        public static bool EncryptExcel(RMSFileModel filemodel, string path)
        {
            var excelClass = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook myExecl = null;
            try
            {
                myExecl = excelClass.Workbooks.Open(filemodel.FSourcePath);
                Thread.Sleep(SleepTime);
                DateTime exprieDateTime = DateTime.Now.AddYears(50);
                if (myExecl != null)
                {
                    myExecl.Activate();
                    var irmPremission = myExecl.Permission;
                    if (irmPremission.Enabled == true)
                    {
                        filemodel.FErrorMessage = "加密已加密的Excel时错误";
                        filemodel.FContent = "当前文档已经加密，操作失败!";
                        filemodel.IsFalse = true;
                        MailHelper mail = MailHelper.GetMailHelper(filemodel.AuthorEmail, filemodel.Author, filemodel.FTitle, filemodel.UploadTime, "File was already encrypted by using RMS Protocol before, do not support");
                        mail.Send();
                        try
                        {
                            myExecl.Save();
                        }
                        catch { }

                        return false;
                    }
                    myExecl.Application.Visible = false;
                    irmPremission.Enabled = true;
                    int MsoPermissionValue = 0;
                    if (filemodel.Users != null && filemodel.Users.Length > 0)
                    {
                        foreach (var item in filemodel.Users)
                        {
                            string value = item.LookupValue;
                            string email = item.Email;
                            if (!String.IsNullOrEmpty(value) && (value == "每个人" || value.ToLower() == "everyone"))
                            {
                                email = "Everyone";
                            }
                            if (!String.IsNullOrEmpty(Helper.ObjIsEmail(email)))
                            {
                                if (filemodel.FIsRead)
                                {
                                    MsoPermissionValue += MsoPermission.msoPermissionRead.GetHashCode();
                                    //MsoPermissionValue = MsoPermissionValue + MsoPermission.msoPermissionRead.();
                                    //irmPremission.Add(email, MsoPermission.msoPermissionRead, exprieDateTime);
                                }
                                if (filemodel.FIsPrint)
                                {
                                    MsoPermissionValue += MsoPermission.msoPermissionPrint.GetHashCode();
                                    //irmPremission.Add(email, MsoPermission.msoPermissionPrint, exprieDateTime);
                                }
                                if (filemodel.FIsEdit)
                                {
                                    MsoPermissionValue += MsoPermission.msoPermissionEdit.GetHashCode();
                                    MsoPermissionValue += MsoPermission.msoPermissionExtract.GetHashCode();
                                    //irmPremission.Add(email, MsoPermission.msoPermissionEdit, exprieDateTime);
                                }
                                if (filemodel.FIsSave)
                                {
                                    MsoPermissionValue += MsoPermission.msoPermissionSave.GetHashCode();
                                    //irmPremission.Add(email, MsoPermission.msoPermissionSave, exprieDateTime);
                                }
                                if (filemodel.FIsFullControl)
                                {
                                    MsoPermissionValue += MsoPermission.msoPermissionFullControl.GetHashCode();
                                    //irmPremission.Add(email, MsoPermission.msoPermissionFullControl, exprieDateTime);
                                }
                                irmPremission.Add(email, MsoPermissionValue, exprieDateTime);
                            }
                        }
                    }
                    else
                    {
                        if (filemodel.FIsRead)
                        {
                            MsoPermissionValue += MsoPermission.msoPermissionRead.GetHashCode();
                            //irmPremission.Add("Everyone", MsoPermission.msoPermissionRead, exprieDateTime);
                        }
                        if (filemodel.FIsPrint)
                        {
                            MsoPermissionValue += MsoPermission.msoPermissionPrint.GetHashCode();
                            //irmPremission.Add("Everyone", MsoPermission.msoPermissionPrint, exprieDateTime);
                        }
                        if (filemodel.FIsEdit)
                        {
                            MsoPermissionValue += MsoPermission.msoPermissionEdit.GetHashCode();
                            MsoPermissionValue += MsoPermission.msoPermissionExtract.GetHashCode();
                            //irmPremission.Add("Everyone", MsoPermission.msoPermissionEdit, exprieDateTime);
                        }
                        if (filemodel.FIsSave)
                        {
                            MsoPermissionValue += MsoPermission.msoPermissionSave.GetHashCode();
                            //irmPremission.Add("Everyone", MsoPermission.msoPermissionSave, exprieDateTime);
                        }
                        if (filemodel.FIsFullControl)
                        {
                            MsoPermissionValue += MsoPermission.msoPermissionFullControl.GetHashCode();
                            //irmPremission.Add("Everyone", MsoPermission.msoPermissionFullControl, exprieDateTime);
                        }
                        irmPremission.Add("Everyone", MsoPermissionValue, exprieDateTime);
                    }
                }
                string tempPath = path + filemodel.FTitle;
                myExecl.SaveAs(tempPath);
                System.IO.File.Delete(tempPath);
                
                filemodel.FRMSPath = tempPath;
                filemodel.FContent = "加密成功";
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("permission") || ex.Message.Contains("权限") || ex.Message.Contains("0x800A03EC"))
                {
                    filemodel.FErrorMessage = "加密Excel时错误，文件已经过RMS加密";
                    filemodel.FContent = ex.Message;

                    filemodel.IsFalse = true;
                    MailHelper mail = MailHelper.GetMailHelper(filemodel.AuthorEmail, filemodel.Author, filemodel.FTitle, filemodel.UploadTime, "File was already encrypted by using RMS Protocol before, do not support");
                    mail.Send();
                    try
                    {
                        myExecl.Save();
                    }
                    catch { }


                    return false;
                }
                else
                {
                    filemodel.FErrorMessage = "加密Excel时错误";
                    filemodel.FContent = ex.Message;
                    filemodel.IsFalse = false;
                    //MailHelper mail = MailHelper.GetMailHelper(filemodel.AuthorEmail, filemodel.Author, filemodel.FTitle, filemodel.UploadTime, ex.Message);
                    //mail.Send();
                    try
                    {
                        myExecl.Save();
                    }
                    catch { }

                    return false;

                }
            }
            finally
            {
                if (myExecl != null)
                    myExecl.Close();
                excelClass.Quit();
            }
        }

        /// <summary>
        /// 加密PPT
        /// </summary>
        /// <param name="RMSFileModel">文件属性</param>
        /// <param name="path">文件路径</param>
        /// <returns>加密结果</returns>
        public static bool EncryptPPT(RMSFileModel filemodel, string path)
        {
            var pptClass = new Microsoft.Office.Interop.PowerPoint.Application();
            Microsoft.Office.Interop.PowerPoint.Presentation myppt = null;
            try
            {
                myppt = pptClass.Presentations.Open(filemodel.FSourcePath, MsoTriState.msoCTrue, MsoTriState.msoCTrue, MsoTriState.msoFalse);
                Thread.Sleep(SleepTime);
                DateTime exprieDateTime = DateTime.Now.AddYears(50);
                if (myppt != null)
                {
                    //myppt.Activate();
                    var irmPremission = myppt.Permission;

                    if (irmPremission.Enabled == true)
                    {
                        filemodel.FErrorMessage = "加密已加密的Excel时错误";
                        filemodel.FContent = "当前文档已经加密，操作失败!";
                        filemodel.IsFalse = true;
                        MailHelper mail = MailHelper.GetMailHelper(filemodel.AuthorEmail, filemodel.Author, filemodel.FTitle, filemodel.UploadTime, "File was already encrypted by using RMS Protocol before, do not support");
                        mail.Send();

                        try
                        {
                            myppt.Save();
                        }
                        catch { }

                        return false;
                    }
                    //myppt.Application.Visible = false;
                    irmPremission.Enabled = true;
                    int MsoPermissionValue = 0;
                    if (filemodel.Users != null && filemodel.Users.Length > 0)
                    {
                        foreach (var item in filemodel.Users)
                        {
                            string value = item.LookupValue;
                            string email = item.Email;
                            if (!String.IsNullOrEmpty(value) && (value == "每个人" || value.ToLower() == "everyone"))
                            {
                                email = "Everyone";
                            }
                            if (!String.IsNullOrEmpty(Helper.ObjIsEmail(email)))
                            {
                                if (filemodel.FIsRead)
                                {
                                    MsoPermissionValue += MsoPermission.msoPermissionRead.GetHashCode();
                                    //MsoPermissionValue = MsoPermissionValue + MsoPermission.msoPermissionRead.();
                                    //irmPremission.Add(email, MsoPermission.msoPermissionRead, exprieDateTime);
                                }
                                if (filemodel.FIsPrint)
                                {
                                    MsoPermissionValue += MsoPermission.msoPermissionPrint.GetHashCode();
                                    //irmPremission.Add(email, MsoPermission.msoPermissionPrint, exprieDateTime);
                                }
                                if (filemodel.FIsEdit)
                                {
                                    MsoPermissionValue += MsoPermission.msoPermissionEdit.GetHashCode();
                                    MsoPermissionValue += MsoPermission.msoPermissionExtract.GetHashCode();
                                    //irmPremission.Add(email, MsoPermission.msoPermissionEdit, exprieDateTime);
                                }
                                if (filemodel.FIsSave)
                                {
                                    MsoPermissionValue += MsoPermission.msoPermissionSave.GetHashCode();
                                    //irmPremission.Add(email, MsoPermission.msoPermissionSave, exprieDateTime);
                                }
                                if (filemodel.FIsFullControl)
                                {
                                    MsoPermissionValue += MsoPermission.msoPermissionFullControl.GetHashCode();
                                    //irmPremission.Add(email, MsoPermission.msoPermissionFullControl, exprieDateTime);
                                }
                                irmPremission.Add(email, MsoPermissionValue, exprieDateTime);
                            }
                        }
                    }
                    else
                    {
                        if (filemodel.FIsRead)
                        {
                            MsoPermissionValue += MsoPermission.msoPermissionRead.GetHashCode();
                            //irmPremission.Add("Everyone", MsoPermission.msoPermissionRead, exprieDateTime);
                        }
                        if (filemodel.FIsPrint)
                        {
                            MsoPermissionValue += MsoPermission.msoPermissionPrint.GetHashCode();
                            //irmPremission.Add("Everyone", MsoPermission.msoPermissionPrint, exprieDateTime);
                        }
                        if (filemodel.FIsEdit)
                        {
                            MsoPermissionValue += MsoPermission.msoPermissionEdit.GetHashCode();
                            MsoPermissionValue += MsoPermission.msoPermissionExtract.GetHashCode();
                            //irmPremission.Add("Everyone", MsoPermission.msoPermissionEdit, exprieDateTime);
                        }
                        if (filemodel.FIsSave)
                        {
                            MsoPermissionValue += MsoPermission.msoPermissionSave.GetHashCode();
                            //irmPremission.Add("Everyone", MsoPermission.msoPermissionSave, exprieDateTime);
                        }
                        if (filemodel.FIsFullControl)
                        {
                            MsoPermissionValue += MsoPermission.msoPermissionFullControl.GetHashCode();
                            //irmPremission.Add("Everyone", MsoPermission.msoPermissionFullControl, exprieDateTime);
                        }
                        irmPremission.Add("Everyone", MsoPermissionValue, exprieDateTime);

                    }
                }
                string tempPath = path + filemodel.FTitle;
                myppt.SaveAs(tempPath);
                System.IO.File.Delete(tempPath);
                
                filemodel.FRMSPath = tempPath;
                filemodel.FContent = "加密成功";
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("permission") || ex.Message.Contains("权限") || ex.Message.Contains("HRESULT E_FAIL"))
                {
                    filemodel.FErrorMessage = "加密PPT时错误，文件已经过RMS加密";
                    filemodel.FContent = ex.Message;

                    filemodel.IsFalse = true;
                    MailHelper mail = MailHelper.GetMailHelper(filemodel.AuthorEmail, filemodel.Author, filemodel.FTitle, filemodel.UploadTime, "File was already encrypted by using RMS Protocol before, do not support");
                    mail.Send();

                    try
                    {
                        myppt.Save();
                    }
                    catch { }



                    return false;
                }
                else
                {
                    filemodel.FErrorMessage = "加密PPT时错误";
                    filemodel.FContent = ex.Message;

                    filemodel.IsFalse = false;
                    //MailHelper mail = MailHelper.GetMailHelper(filemodel.AuthorEmail, filemodel.Author, filemodel.FTitle, filemodel.UploadTime, ex.Message);
                    //mail.Send();

                    try
                    {
                        myppt.Save();
                    }
                    catch { }



                    return false;

                }
            }
            finally
            {
                if (myppt != null)
                    myppt.Close();
                pptClass.Quit();
            }
        }

        /// <summary>
        /// 清理Office进程
        /// </summary>
        public static void KillOfficeProcess()
        {
            try
            {
                var proWord = System.Diagnostics.Process.GetProcessesByName("WinWord");
                foreach (var word in proWord)
                {
                    //word.Close();
                    //word.Dispose();
                    word.Kill();
                    //word.CloseMainWindow();
                }
                var proExcel = System.Diagnostics.Process.GetProcessesByName("Excel");
                foreach (var excel in proExcel)
                {
                    //excel.Close();
                    //excel.Dispose();
                    excel.Kill();
                    //excel.CloseMainWindow();
                }
                var proPPT = System.Diagnostics.Process.GetProcessesByName("POWERPNT");
                foreach (var ppt in proPPT)
                {
                    //ppt.Close();
                    //ppt.Dispose();
                    ppt.Kill();
                    //ppt.CloseMainWindow();
                }
            }
            catch (Exception)
            {


            }
        }
    }
}
