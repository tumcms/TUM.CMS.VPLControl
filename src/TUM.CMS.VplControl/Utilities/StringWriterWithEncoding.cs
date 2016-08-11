using System;
using System.IO;
using System.Text;

namespace TUM.CMS.VplControl.Utilities
{
    public class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding encoding;

        public StringWriterWithEncoding()
        {
        }

        public StringWriterWithEncoding(IFormatProvider formatProvider)
            : base(formatProvider)
        {
        }

        public StringWriterWithEncoding(StringBuilder sb)
            : base(sb)
        {
        }

        public StringWriterWithEncoding(StringBuilder sb, IFormatProvider formatProvider)
            : base(sb, formatProvider)
        {
        }

        public StringWriterWithEncoding(Encoding encoding)
        {
            this.encoding = encoding;
        }

        public StringWriterWithEncoding(IFormatProvider formatProvider, Encoding encoding)
            : base(formatProvider)
        {
            this.encoding = encoding;
        }

        public StringWriterWithEncoding(StringBuilder sb, Encoding encoding)
            : base(sb)
        {
            this.encoding = encoding;
        }

        public StringWriterWithEncoding(StringBuilder sb, IFormatProvider formatProvider, Encoding encoding)
            : base(sb, formatProvider)
        {
            this.encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return encoding ?? base.Encoding; }
        }
    }
}