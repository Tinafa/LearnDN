using System;
using System.Collections.Generic;
using System.Text;

namespace UILib
{
    public interface IXUITextList : IXUIObject
    {
        void Clear();
        void Add(string text);
    }
}
