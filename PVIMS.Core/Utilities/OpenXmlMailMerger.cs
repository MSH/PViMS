using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;
using DocumentFormat.OpenXml.Wordprocessing;
using Wp = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using Pic = DocumentFormat.OpenXml.Drawing.Pictures;
using A14 = DocumentFormat.OpenXml.Office2010.Drawing;

using OpenXmlPowerTools;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PVIMS.Core.Utilities
{
    public static class OpenXmlMailMerger
    {
        public static void Merge(string templateName, string documentName, MergeData mergeData)
        {
            var mergedDocumentName = string.Empty;

            var templateDirectory = ConfigurationManager.AppSettings["TemplateFolder"] ?? String.Format("{0}\\Templates\\", System.AppDomain.CurrentDomain.BaseDirectory);
            var templatePath = String.Format("{0}{1}", templateDirectory, templateName);
            var documentPath = String.Format("{0}{1}", String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory), documentName);

            File.Copy(templatePath, documentPath, true);

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(documentPath, true))
            {
                wordDocument.ChangeDocumentType(WordprocessingDocumentType.Document);

                var mainDocPart = wordDocument.MainDocumentPart.Document;

                // Make document readonly
                var docSett = wordDocument.MainDocumentPart.DocumentSettingsPart;
                docSett.RootElement.Append(new DocumentProtection { Edit = DocumentProtectionValues.ReadOnly });
                docSett.RootElement.Save();

                foreach (var mergeElement in mergeData.MergeElements.Where(x => x.GetType() == typeof(TextMergeElement)))
                {
                    var mergeField = GetMergeField(mainDocPart.Descendants<SimpleField>(), mergeElement.MergeField);

                    if (mergeField == null) continue;

                    foreach (var textField in mergeField.Descendants<Text>())
                    {
                        textField.Text = mergeElement.MergeValue;
                    }

                }

                foreach (var mergeElement in mergeData.MergeElements.Where(x => x.GetType() == typeof(ImageMergeElement)))
                {
                    var bookMarkStart = mainDocPart.Descendants<BookmarkStart>().SingleOrDefault(b => b.Name == mergeElement.MergeField);

                    if (bookMarkStart == null) continue;

                    string imageRId = "r" + Guid.NewGuid().ToString().Replace("-", String.Empty);

                    MainDocumentPart mainPart = wordDocument.MainDocumentPart;

                    ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Bmp, imageRId);

                    using (FileStream stream = new FileStream(mergeElement.MergeValue, FileMode.Open))
                    {
                        imagePart.FeedData(stream);
                    }

                    bookMarkStart.Parent.AppendChild<Drawing>(CreateDrawing(imageRId));

                }

                wordDocument.Close();
            }
        }

        public static void MergeWithTextReplace(string templateName, string documentName, MergeData mergeData)
        {
            var mergedDocumentName = string.Empty;

            var templateDirectory = ConfigurationManager.AppSettings["TemplateFolder"] ?? String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);
            var templatePath = String.Format("{0}{1}", templateDirectory, templateName);
            var documentPath = String.Format("{0}{1}", String.Format("{0}\\Temp\\", AppDomain.CurrentDomain.BaseDirectory), documentName);

            File.Copy(templatePath, documentPath, true);

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(documentPath, true))
            {
                wordDocument.ChangeDocumentType(WordprocessingDocumentType.Document);

                // Make document readonly
                var docSett = wordDocument.MainDocumentPart.DocumentSettingsPart;
                docSett.RootElement.Append(new DocumentProtection { Edit = DocumentProtectionValues.ReadOnly });
                docSett.RootElement.Save();

                var body = wordDocument.MainDocumentPart.Document.Body;

                foreach (var mergeElement in mergeData.MergeElements)
                {
                    foreach (var run in body.Descendants<Run>())
                    {
                        foreach (var text in run.Descendants<Text>())
                        {
                            var search = String.Format("*{0}*", mergeElement.MergeField);
                            if (text.Text.Contains(search))
                            {
                                text.Text = text.Text.Replace(search, mergeElement.MergeValue);
                            }
                        }
                    }
                }

                wordDocument.Close();
            }
        }

        public static void Join(string outputPath, int startId, params string[] filepaths)
        {
            if (filepaths != null && filepaths.Length > 0)
            {
                //WordprocessingDocument myDoc;

                //if (File.Exists(outputPath)) {
                //    myDoc = WordprocessingDocument.Open(outputPath, true);
                //}
                //else 
                //{
                //    myDoc = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document, true);
                //    myDoc.AddMainDocumentPart();

                //    myDoc.MainDocumentPart.Document = new Document();
                //    myDoc.MainDocumentPart.Document.Body = new Body();

                //    myDoc.MainDocumentPart.Document.Save();
                //}
                //MainDocumentPart mainPart = myDoc.MainDocumentPart;

                //for (int i = 0; i < filepaths.Length; i++)
                //{
                //    string altChunkId = "AltChunkId" + (startId + i).ToString();
                //    AlternativeFormatImportPart chunk = mainPart.AddAlternativeFormatImportPart(
                //        AlternativeFormatImportPartType.WordprocessingML, altChunkId);
                //    using (FileStream fileStream = File.Open(String.Format("{0}{1}", String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory), @filepaths[i]), FileMode.Open))
                //    {
                //        chunk.FeedData(fileStream);
                //    }
                //    DocumentFormat.OpenXml.Wordprocessing.AltChunk altChunk = new DocumentFormat.OpenXml.Wordprocessing.AltChunk();
                //    altChunk.Id = altChunkId;
                //    mainPart.Document.Body.AppendChild(new Paragraph());
                //    //next document
                //    mainPart.Document.Body.InsertAfter(altChunk, mainPart.Document.Body.Elements<Paragraph>().Last());
                //    //new page
                //    mainPart.Document.Body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
                //}
                //mainPart.Document.Save();
                //myDoc.Close();
                //myDoc = null;

                List<Source> sources = new List<Source>();
                //if (File.Exists(outputPath)) {
                //    sources.Add(new Source(new WmlDocument(outputPath), true));
                //}
                for (int i = 0; i < filepaths.Length; i++)
                {
                    sources.Add(new Source(new WmlDocument(String.Format("{0}{1}", String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory), @filepaths[i])), true));
                }
                DocumentBuilder.BuildDocument(sources, outputPath);
            }
        }

        private static SimpleField GetMergeField(IEnumerable<SimpleField> simpleFields, string fieldName)
        {
            var expression = string.Format(@"[\s]*MERGEFIELD[\s]+{0}", fieldName);

            foreach (var simpleField in simpleFields)
            {
                if (Regex.IsMatch(simpleField.Instruction.Value, expression, RegexOptions.None))
                {
                    return simpleField;
                }
            }

            return null;
        }

        // Please note that this method is specifically for generating the barcodes of the paymentlist. Needs to be refactored to accept more parameters (dimensions for instance)
        private static Drawing CreateDrawing(string imageResourceId)
        {
            Drawing drawing1 = new Drawing();

            Wp.Inline inline1 = new Wp.Inline() { DistanceFromTop = (UInt32Value)0U, DistanceFromBottom = (UInt32Value)0U, DistanceFromLeft = (UInt32Value)0U, DistanceFromRight = (UInt32Value)0U };
            Wp.Extent extent1 = new Wp.Extent() { Cx = 1057423L, Cy = 295316L };
            Wp.EffectExtent effectExtent1 = new Wp.EffectExtent() { LeftEdge = 0L, TopEdge = 0L, RightEdge = 9525L, BottomEdge = 9525L };
            Wp.DocProperties docProperties1 = new Wp.DocProperties() { Id = (UInt32Value)1U, Name = "Picture 1" };

            Wp.NonVisualGraphicFrameDrawingProperties nonVisualGraphicFrameDrawingProperties1 = new Wp.NonVisualGraphicFrameDrawingProperties();

            A.GraphicFrameLocks graphicFrameLocks1 = new A.GraphicFrameLocks() { NoChangeAspect = true };
            graphicFrameLocks1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

            nonVisualGraphicFrameDrawingProperties1.Append(graphicFrameLocks1);

            A.Graphic graphic1 = new A.Graphic();
            graphic1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

            A.GraphicData graphicData1 = new A.GraphicData() { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" };

            Pic.Picture picture1 = new Pic.Picture();
            picture1.AddNamespaceDeclaration("pic", "http://schemas.openxmlformats.org/drawingml/2006/picture");

            Pic.NonVisualPictureProperties nonVisualPictureProperties1 = new Pic.NonVisualPictureProperties();
            Pic.NonVisualDrawingProperties nonVisualDrawingProperties1 = new Pic.NonVisualDrawingProperties() { Id = (UInt32Value)0U, Name = "bc_7032.bmp" };
            Pic.NonVisualPictureDrawingProperties nonVisualPictureDrawingProperties1 = new Pic.NonVisualPictureDrawingProperties();

            nonVisualPictureProperties1.Append(nonVisualDrawingProperties1);
            nonVisualPictureProperties1.Append(nonVisualPictureDrawingProperties1);

            Pic.BlipFill blipFill1 = new Pic.BlipFill();

            A.Blip blip1 = new A.Blip() { Embed = imageResourceId };

            A.BlipExtensionList blipExtensionList1 = new A.BlipExtensionList();

            A.BlipExtension blipExtension1 = new A.BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" };

            A14.UseLocalDpi useLocalDpi1 = new A14.UseLocalDpi() { Val = false };
            useLocalDpi1.AddNamespaceDeclaration("a14", "http://schemas.microsoft.com/office/drawing/2010/main");

            blipExtension1.Append(useLocalDpi1);

            blipExtensionList1.Append(blipExtension1);

            blip1.Append(blipExtensionList1);

            A.Stretch stretch1 = new A.Stretch();
            A.FillRectangle fillRectangle1 = new A.FillRectangle();

            stretch1.Append(fillRectangle1);

            blipFill1.Append(blip1);
            blipFill1.Append(stretch1);

            Pic.ShapeProperties shapeProperties1 = new Pic.ShapeProperties();

            A.Transform2D transform2D1 = new A.Transform2D();
            A.Offset offset1 = new A.Offset() { X = 0L, Y = 0L };
            A.Extents extents1 = new A.Extents() { Cx = 1057423L, Cy = 295316L };

            transform2D1.Append(offset1);
            transform2D1.Append(extents1);

            A.PresetGeometry presetGeometry1 = new A.PresetGeometry() { Preset = A.ShapeTypeValues.Rectangle };
            A.AdjustValueList adjustValueList1 = new A.AdjustValueList();

            presetGeometry1.Append(adjustValueList1);

            shapeProperties1.Append(transform2D1);
            shapeProperties1.Append(presetGeometry1);

            picture1.Append(nonVisualPictureProperties1);
            picture1.Append(blipFill1);
            picture1.Append(shapeProperties1);

            graphicData1.Append(picture1);

            graphic1.Append(graphicData1);

            inline1.Append(extent1);
            inline1.Append(effectExtent1);
            inline1.Append(docProperties1);
            inline1.Append(nonVisualGraphicFrameDrawingProperties1);
            inline1.Append(graphic1);

            drawing1.Append(inline1);

            return drawing1;
        }

        public static void ValidateWordDocument(string filepath)
        {
            using (WordprocessingDocument wordprocessingDocument =
            WordprocessingDocument.Open(filepath, true))
            {
                try
                {
                    OpenXmlValidator validator = new OpenXmlValidator();
                    int count = 0;
                    foreach (ValidationErrorInfo error in
                        validator.Validate(wordprocessingDocument))
                    {
                        count++;
                        Console.WriteLine("Error " + count);
                        Console.WriteLine("Description: " + error.Description);
                        Console.WriteLine("ErrorType: " + error.ErrorType);
                        Console.WriteLine("Node: " + error.Node);
                        Console.WriteLine("Path: " + error.Path.XPath);
                        Console.WriteLine("Part: " + error.Part.Uri);
                        Console.WriteLine("-------------------------------------------");
                    }

                    Console.WriteLine("count={0}", count);
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                wordprocessingDocument.Close();
            }
        }

        public static void ValidateCorruptedWordDocument(string filepath)
        {
            // Insert some text into the body, this would cause Schema Error
            using (WordprocessingDocument wordprocessingDocument =
            WordprocessingDocument.Open(filepath, true))
            {
                // Insert some text into the body, this would cause Schema Error
                Body body = wordprocessingDocument.MainDocumentPart.Document.Body;
                Run run = new Run(new Text("some text"));
                body.Append(run);

                try
                {
                    OpenXmlValidator validator = new OpenXmlValidator();
                    int count = 0;
                    foreach (ValidationErrorInfo error in
                        validator.Validate(wordprocessingDocument))
                    {
                        count++;
                        Console.WriteLine("Error " + count);
                        Console.WriteLine("Description: " + error.Description);
                        Console.WriteLine("ErrorType: " + error.ErrorType);
                        Console.WriteLine("Node: " + error.Node);
                        Console.WriteLine("Path: " + error.Path.XPath);
                        Console.WriteLine("Part: " + error.Part.Uri);
                        Console.WriteLine("-------------------------------------------");
                    }

                    Console.WriteLine("count={0}", count);
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

    public class MergeData
    {
        public MergeData() { MergeElements = new List<MergeElement>(); }

        public List<MergeElement> MergeElements { get; set; }
    }

    public abstract class MergeElement
    {
        public string MergeField { get; set; }
        public string MergeValue { get; set; }
    }

    public class TextMergeElement : MergeElement
    {

    }

    public class ImageMergeElement : MergeElement
    {
    }
}
