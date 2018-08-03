using ProcessCIW.Utilities;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Xunit;
using System.IO;
using GemBox.Document;
using System;

namespace ProcessCIW.Test.dll
{
    public partial class CiwTest
    {
        XmlTool xmlTool = new XmlTool();

        [STAThread]
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

            document.Save("Hello World.docx");

            DocumentModel dm = DocumentModel.Load(filePath, new DocxLoadOptions() { Password = "passowrd" });
            dm.Save(filePath, new DocxSaveOptions() { Password = "password" });

            
            var result = xmlTool.isPasswordProtected(filePath);
        }

        public static TheoryData<string> isPasswordProtected_True =>
            new TheoryData<string>
            {
                { @"C:\Users\JasonBGoodell\Source\Repos\CIW\ProcessCIW\bin\Debug\Temp\file1.docx" }
            };

        public void isPasswordProtected_False_Test(string filePath)
        {
            xmlTool.isPasswordProtected(filePath);
        }
    }
}
