using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UILib
{
    public delegate bool ListSelectEventHandler(IXUIListItem uiListItem);
    public delegate bool ListDoubleClickEventHandler(IXUIListItem uiListItem);

    public delegate void OnAfterRepostion();
    public interface IXUIList : IXUIObject
    {
        void Refresh();
        void SetAnimateSmooth(bool b);

        void RegisterRepositionHandle(OnAfterRepostion reposition);

        IUIRect GetParentUIRect();
        IUIPanel GetParentPanel();

    }
}
