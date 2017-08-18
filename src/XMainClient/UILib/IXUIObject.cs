using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UILib
{
    public interface IXUIObject
    {
        GameObject gameObject { get; }
        IXUIObject parent { get; set; }        
        IXUIObject GetUIObject(string strName);
        ulong ID { get; set; }
        bool IsVisible();
        void SetVisible(bool bVisible);
        void OnFocus();
        void Highlight(bool bTrue);
        bool Exculsive { get; set; }
    }
}
