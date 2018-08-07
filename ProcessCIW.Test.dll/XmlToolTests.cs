using ProcessCIW.Utilities;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Xunit;
using System.IO;
using GemBox.Document;
using System;
using System.Collections.Generic;
using ProcessCIW.Models;
using ProcessCIW.Process;

namespace ProcessCIW.Test.dll
{
    public partial class CiwTest
    {

        [Theory]
        [MemberData(nameof(isPasswordProtected_True))]
        public void isPasswordProtected_True_Test(string filePath)
        {           
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            DocumentModel document = new DocumentModel();
            Section section = new Section(document);
            document.Sections.Add(section);
            GemBox.Document.Paragraph paragraph = new GemBox.Document.Paragraph(document);
            section.Blocks.Add(paragraph);
            GemBox.Document.Run run = new GemBox.Document.Run(document, "Hello World!");
            paragraph.Inlines.Add(run);
            document.Save(filePath, new DocxSaveOptions() { Password = "password" });            
            var result = xmlTool.isPasswordProtected(filePath);
            Assert.True(result);
            File.Delete(filePath);
        }

        public static TheoryData<string> isPasswordProtected_False =>
            new TheoryData<string>
            {
                { @"C:\Users\JasonBGoodell\Source\Repos\CIW\ProcessCIW\bin\Debug\Temp\file1.docx" }
            };

        public static TheoryData<string> isPasswordProtected_True =>
            new TheoryData<string>
            {
                { @"C:\Users\JasonBGoodell\Source\Repos\CIW\ProcessCIW\bin\Debug\Temp\file1.docx" }
            };

        [Theory]
        [MemberData(nameof(isPasswordProtected_False))]
        public void isPasswordProtected_False_Test(string filePath)
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            DocumentModel document = new DocumentModel();
            Section section = new Section(document);
            document.Sections.Add(section);
            GemBox.Document.Paragraph paragraph = new GemBox.Document.Paragraph(document);
            section.Blocks.Add(paragraph);
            GemBox.Document.Run run = new GemBox.Document.Run(document, "Hello World!");
            paragraph.Inlines.Add(run);
            document.Save(filePath);
            var result = xmlTool.isPasswordProtected(filePath);
            Assert.False(result);
            File.Delete(filePath);
        }
        [Fact]
        public void parseCiwDocument_True_Test()
        {
            int x=0;
            File.Copy(@"C: \Users\JasonBGoodell\Source\Repos\CIW\ProcessCIW.Test.dll\TestDocuments\Blank_Eqiptest.docx", @"C: \Users\JasonBGoodell\Source\Repos\CIW\ProcessCIW.Test.dll\TestDocuments\Blank_Eqiptest2.docx");
            var result = xmlTool.parseCiwDocument(@"C: \Users\JasonBGoodell\Source\Repos\CIW\ProcessCIW.Test.dll\TestDocuments\Blank_Eqiptest2.docx", 353973, @"Blank_Eqiptest.docx",out x);
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.IsType<List<CIWData>>(result);
            Assert.Equal((int)Enum.ErrorCodes.successfully_processed, x);
            File.Delete(@"C: \Users\JasonBGoodell\Source\Repos\CIW\ProcessCIW.Test.dll\TestDocuments\Blank_Eqiptest2.docx");
        }

        [Theory]
        [MemberData(nameof(parseCiwDocument_Negative))]
        public void parseCiwDocument_Negative_Test(string s)
        {
            string folder = @"C: \Users\JasonBGoodell\Source\Repos\CIW\ProcessCIW.Test.dll\TestDocuments\";
            string newfile = string.Format("{0}{1}2{2}", folder, Path.GetFileNameWithoutExtension(s), Path.GetExtension(s));
            if(File.Exists(newfile)){File.Delete(newfile);}
            File.Copy(string.Format("{0}{1}",folder, s), newfile);
            int x = 0;
            var result = xmlTool.parseCiwDocument(newfile, 47084, @"Blank_Eqiptest.docx", out x);
            Assert.Null(result);
            Assert.NotEqual((int)Enum.ErrorCodes.successfully_processed, x);
            File.Delete(newfile);
        }

        public static TheoryData<string> parseCiwDocument_Negative =>
            new TheoryData<string>
            {
                { "Wrongversion.docx" }
            };
    }
}
