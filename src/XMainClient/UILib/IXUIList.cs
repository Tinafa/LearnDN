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
        //Int32 Count { get; }
        //bool EnableMultiSelect { get; set; }
        //void Clear();
        void Refresh();
        void CloseList();
        void SetAnimateSmooth(bool b);

        void RegisterRepositionHandle(OnAfterRepostion reposition);

        IUIRect GetParentUIRect();
        IUIPanel GetParentPanel();
        //int GetSelectedIndex();
        //void SetSelectedIndex(int nIndex);
        //void SetSelectedItemById(UInt32 unId);
        //IXUIListItem GetSelectedItem();
        //IXUIListItem[] GetSelectedItems();
        //IXUIListItem GetItemById(UInt32 unId);
        //IXUIListItem GetItemByIndex(Int32 nIndex);
        //IXUIListItem[] GetAllItems();
        //IXUIListItem AddListItem(GameObject obj);
        //IXUIListItem AddListItem();
        //void SetEnable(bool bEnable);
        //void SetEnableSelect(bool bEnable);
        //void SetEnableSelect(List<UInt32> listIds);
        //bool DelItemById(UInt32 unId);
        //bool DelItemByIndex(Int32 nIndex);
        //void Highlight(bool bTrue);
        //void Highlight(List<UInt32> listIds);

        //void RegisterListSelectEventHandler(ListSelectEventHandler eventHandler);
        //void RegisterListUnSelectEventHandler(ListSelectEventHandler eventHandler);
        //void RegisterListDoubleClickEventHandler(ListDoubleClickEventHandler eventHandler);
    }
}
