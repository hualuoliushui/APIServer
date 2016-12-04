using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Aspose.Words;
using Aspose.Cells;
using System.IO;
using System.Text.RegularExpressions;

namespace APIServer.Services.FileService
{
    public class AposeMethod : FileConvertBase
    {

        public override bool WordToHTML(string sourcePath,string targetPath,string targetRelativeDirectory)
        {
            try
            {
                Document awd = new Document(sourcePath);
                awd.Save(targetPath, Aspose.Words.SaveFormat.Html);
                changePathInHtml(targetPath, targetRelativeDirectory);
                return true;
            }
            catch (Exception e)
            {

                return false;
            }
        }

        public override bool ExcelToHTML(string sourcePath, string targetPath, string targetRelativeDirectory)
        {
            try
            {
                Workbook book = new Workbook(sourcePath);
                book.Save(targetPath, Aspose.Cells.SaveFormat.Html);
                changePathInHtml(targetPath, targetRelativeDirectory);
                return true;
            }
            catch (Exception e)
            {

                return false;
            }
        }

        //未完成
        public override bool PPToHTML(string sourcePath, string targetPath, string targetRelativeDirectory)
        {
            try
            {
                
                return true;
            }
            catch (Exception e)
            {

                return false;
            }
        }

       
    }
}