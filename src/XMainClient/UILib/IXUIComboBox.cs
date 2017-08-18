using System;
using UnityEngine;

namespace UILib
{
    public delegate void ComboboxClickEventHandler(int value);

    public interface IXUIComboBox : IXUIObject
    {
        /// <summary>
        /// 组件初始化 用于初始化XUIComboBox下挂载的对象
        /// </summary>
        void ModuleInit();

        void AddItem(string text, int value);

        GameObject GetItem(int value);

        void ClearItems();

        bool SelectItem(int value, bool withCallback);

        void RegisterSpriteClickEventHandler(ComboboxClickEventHandler eventHandler);

        void ResetState();
    }
    
}
