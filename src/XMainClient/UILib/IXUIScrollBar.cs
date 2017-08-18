using System;
using System.Collections.Generic;
using System.Text;

namespace UILib
{
    public delegate bool ScrollBarChangeEventHandler(IXUIScrollBar iXUIScrollBar);
    public delegate bool ScrollBarDragFinishedEventHandler();
    public interface IXUIScrollBar
    {
        float value { get; set; }
        float size { get; set; }
        void RegisterScrollBarChangeEventHandler(ScrollBarChangeEventHandler eventHandler);
        void RegisterScrollBarDragFinishedEventHandler(ScrollBarDragFinishedEventHandler eventHandler);
    }
}
