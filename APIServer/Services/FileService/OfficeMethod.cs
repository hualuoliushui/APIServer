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
using System.Runtime.InteropServices;

namespace APIServer.Services.FileService
{
    public class OfficeMethod : FileConvertBase
    {
        public override bool WordToHTML(string sourcePath, string targetPath, string targetRelativeDirectory)
        {
            try
            {
                if (!sourcePath.EndsWith(".doc") && !sourcePath.EndsWith(".docx"))
                {
                    return false;
                }
                 
                Microsoft.Office.Interop.Word.Application docApp = new Microsoft.Office.Interop.Word.Application();
                Document doc = null;
                // 缺省参数 
                object Unknown = Type.Missing;

                //如果已存在，则重新生成
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }

                // 只读方式打开 
                object readOnly = true;
                // 指定另存为格式(html) 
                object formatHtml = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatHTML;

                object source = sourcePath;
                object target = targetPath;
                // 打开doc文件 
                Documents docs = docApp.Documents;
                doc = docs.Open(ref source, ref Unknown, ref readOnly,
                    ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown,
                    ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown);
                //另存为HTML
                doc.SaveAs(ref target, ref formatHtml,
                      ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown,
                      ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown);
                Marshal.ReleaseComObject(docs);
                Marshal.ReleaseComObject(doc);

                //修改html文件中的src元素的路径
                changePathInHtml(targetPath, targetRelativeDirectory);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool ExcelToHTML(string sourcePath, string targetPath, string targetRelativeDirectory)
        {
            Excel.XlFileFormat targetType = Excel.XlFileFormat.xlHtml;
            object missing = Type.Missing;
            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            try
            {
                if (!sourcePath.EndsWith(".xls") && !sourcePath.EndsWith(".xlsx"))
                {
                    return false;
                }

                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                excelApp = new Excel.Application();
                object target = targetPath;
                object type = targetType;
                Workbooks workbooks = excelApp.Workbooks;
                workbook = workbooks.Open(sourcePath, missing, missing, missing, missing, missing,
                        missing, missing, missing, missing, missing, missing, missing, missing, missing);
                workbook.SaveAs(target, type, missing, missing, missing, missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, missing, missing, missing, missing, missing);

                Marshal.ReleaseComObject(workbooks);
                Marshal.ReleaseComObject(workbook);
                //修改html文件中的src元素的路径
                changePathInHtml(targetPath, targetRelativeDirectory);

                return  true;
            }
            catch
            {
                return false;
            }
            finally
            {
                //if (workBook != null)
                //{
                //    workBook.Close(true, missing, missing);
                //    workBook = null;
                //}
                //if (application != null)
                //{
                //    application.Quit();
                //    application = null;
                //}
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
            }
        }

        public override bool PPToHTML(string sourcePath, string targetPath, string targetRelativeDirectory)
        {
            PowerPoint.PpSaveAsFileType targetFileType = PowerPoint.PpSaveAsFileType.ppSaveAsHTML;
            object missing = Type.Missing;
            PowerPoint.Application pptApp = null;
            PowerPoint.Presentation presentation = null;
            try
            {
                if (!sourcePath.EndsWith(".ppt") && !sourcePath.EndsWith(".pptx"))
                {
                    return false;
                }

                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                object target = targetPath;
                pptApp = new PowerPoint.Application();
                PowerPoint.Presentations presentations = pptApp.Presentations;
                presentation = presentations.Open(sourcePath, MsoTriState.msoTrue, MsoTriState.msoFalse, MsoTriState.msoFalse);
                presentation.SaveAs(target.ToString(), targetFileType, Microsoft.Office.Core.MsoTriState.msoTrue);

                Marshal.ReleaseComObject(presentations);
                Marshal.ReleaseComObject(presentation);
                //修改html文件中的src元素的路径
                changePathInHtml(targetPath, targetRelativeDirectory); 
                
                return true;
            }
            catch
            {
                return  false;
            }
            finally
            {
                //if (presentation != null)
                //{
                //    presentation.Close();
                //    presentation = null;
                //}
                //if (application != null)
                //{
                //    application.Quit();
                //    application = null;
                //}
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
            }
        }
    }
}