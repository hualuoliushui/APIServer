using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using System.IO;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Office;

namespace APIServer.Services
{
    public class FileConvert
    {

        public static bool WordToHTML(string sourcePath)
        {
            try
            {
                string targetHtml = sourcePath.Substring(0, sourcePath.LastIndexOf(".")) + ".html";//将转换后的HTML文件存储在源文件的目录下
                if (!sourcePath.EndsWith(".doc") && !sourcePath.EndsWith(".docx"))
                {
                    return false;
                }
                //测试时发现源文件已经存在时删除不成功，原因是word正在占用文件（无法解除占用），无法删除
                //然后整个函数就会直接返回false
                if (File.Exists(targetHtml))
                {
                    File.Delete(targetHtml);
                }
                Microsoft.Office.Interop.Word.Application newApp = new Microsoft.Office.Interop.Word.Application();
                Document doc = null;
                // 缺省参数 
                object Unknown = Type.Missing;


                // 只读方式打开 
                object readOnly = true;
                // 指定另存为格式(html) 
                object formatHtml = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatHTML;

                object source = sourcePath;
                object target = targetHtml;
                // 打开doc文件 
                doc = newApp.Documents.Open(ref source, ref Unknown, ref readOnly,
                    ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown,
                    ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown);
                //另存为HTML
                doc.SaveAs(ref target, ref formatHtml,
                      ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown,
                      ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ExcelToHTML(string sourcePath)
        {
            bool result = false;
            Excel.XlFileFormat targetType = Excel.XlFileFormat.xlHtml;
            object missing = Type.Missing;
            Excel.ApplicationClass application = null;
            Excel.Workbook workBook = null;
            try
            {
                string targetPath = sourcePath.Substring(0, sourcePath.LastIndexOf(".")) + ".html";//将转换后的HTML文件存储在源文件的目录下
                if (!sourcePath.EndsWith(".xls") && !sourcePath.EndsWith(".xlsx"))
                {
                    return false;
                }

                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                application = new Excel.ApplicationClass();
                object target = targetPath;
                object type = targetType;
                workBook = application.Workbooks.Open(sourcePath, missing, missing, missing, missing, missing,
                        missing, missing, missing, missing, missing, missing, missing, missing, missing);
                workBook.SaveAs(target, type, missing, missing, missing, missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, missing, missing, missing, missing, missing);
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (workBook != null)
                {
                    workBook.Close(true, missing, missing);
                    workBook = null;
                }
                if (application != null)
                {
                    application.Quit();
                    application = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return result;
        }

        public static bool PPTToHTML(string sourcePath)
        {
            bool result;
            PowerPoint.PpSaveAsFileType targetFileType = PowerPoint.PpSaveAsFileType.ppSaveAsHTML;
            object missing = Type.Missing;
            PowerPoint.ApplicationClass application = null;
            PowerPoint.Presentation persentation = null;
            try
            {
                string targetPath = sourcePath.Substring(0, sourcePath.LastIndexOf(".")) + ".html";//将转换后的HTML文件存储在源文件的目录下
                if (!sourcePath.EndsWith(".ppt") && !sourcePath.EndsWith(".pptx"))
                {
                    return false;
                }

                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                object target = targetPath;
                application = new PowerPoint.ApplicationClass();
                persentation = application.Presentations.Open(sourcePath, MsoTriState.msoTrue, MsoTriState.msoFalse, MsoTriState.msoFalse);
                persentation.SaveAs(target.ToString(), targetFileType, Microsoft.Office.Core.MsoTriState.msoTrue);
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (persentation != null)
                {
                    persentation.Close();
                    persentation = null;
                }
                if (application != null)
                {
                    application.Quit();
                    application = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return result;
        }
    }
}