using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Pinokio.Core
{
    public class PinokioCategoryAttribute : CategoryAttribute
    {
        private const char NonPrintableChar = '\r';
        private const ushort maxLength = 10;

        public PinokioCategoryAttribute(string category, ushort categoryPos)
            : base(category.PadLeft(category.Length + (maxLength - categoryPos), PinokioCategoryAttribute.NonPrintableChar))
        {
        }
    }

    public class PinokioNameAttribute : DisplayNameAttribute
    {
        private const char NonPrintableChar = '\r';
        /// <summary>
        /// Max Attribute 개수 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public PinokioNameAttribute(string name, int priority)
            : base(name.PadLeft(name.Length + (-priority), PinokioNameAttribute.NonPrintableChar))
        {
        }
    }
}
