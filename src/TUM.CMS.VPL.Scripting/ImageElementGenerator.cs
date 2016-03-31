// Copyright (c) 2009 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ICSharpCode.AvalonEdit.Rendering;

namespace TUM.CMS.VPL.Scripting
{
    /// <summary>
    ///     This class can be used to embed images inside AvalonEdit like this: <img src="Images/Save.png" />
    /// </summary>
    public class ImageElementGenerator : VisualLineElementGenerator
    {
        private static readonly Regex imageRegex = new Regex(@"<img src=""([\.\/\w\d]+)""/?>",
            RegexOptions.IgnoreCase);

        private readonly string basePath;

        public ImageElementGenerator(string basePath)
        {
            if (basePath == null)
                throw new ArgumentNullException("basePath");
            this.basePath = basePath;
        }

        private Match FindMatch(int startOffset)
        {
            // fetch the end offset of the VisualLine being generated
            var endOffset = CurrentContext.VisualLine.LastDocumentLine.EndOffset;
            var document = CurrentContext.Document;
            var relevantText = document.GetText(startOffset, endOffset - startOffset);
            return imageRegex.Match(relevantText);
        }

        /// <summary>
        ///     Gets the first offset >= startOffset where the generator wants to construct
        ///     an element.
        ///     Return -1 to signal no interest.
        /// </summary>
        public override int GetFirstInterestedOffset(int startOffset)
        {
            var m = FindMatch(startOffset);
            return m.Success ? (startOffset + m.Index) : -1;
        }

        /// <summary>
        ///     Constructs an element at the specified offset.
        ///     May return null if no element should be constructed.
        /// </summary>
        public override VisualLineElement ConstructElement(int offset)
        {
            var m = FindMatch(offset);
            // check whether there's a match exactly at offset
            if (m.Success && m.Index == 0)
            {
                var bitmap = LoadBitmap(m.Groups[1].Value);
                if (bitmap != null)
                {
                    var image = new Image
                    {
                        Source = bitmap,
                        Width = bitmap.PixelWidth,
                        Height = bitmap.PixelHeight
                    };
                    // Pass the length of the match to the 'documentLength' parameter
                    // of InlineObjectElement.
                    return new InlineObjectElement(m.Length, image);
                }
            }
            return null;
        }

        private BitmapImage LoadBitmap(string fileName)
        {
            // TODO: add some kind of cache to avoid reloading the image whenever the
            // VisualLine is reconstructed
            try
            {
                var fullFileName = Path.Combine(basePath, fileName);
                if (File.Exists(fullFileName))
                {
                    var bitmap = new BitmapImage(new Uri(fullFileName));
                    bitmap.Freeze();
                    return bitmap;
                }
            }
            catch (ArgumentException)
            {
                // invalid filename syntax
            }
            catch (IOException)
            {
                // other IO error
            }
            return null;
        }
    }
}