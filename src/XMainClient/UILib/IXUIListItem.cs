using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UILib
{
    public interface IXUIListItem : IXUIObject
    {
        UInt32 id { get; set; }
        Int32 Index { get; }
        void SetIconSprite(string strSprite);
        void SetIconSprite(string strSprite, string strAtlas);
        void SetIconTexture(string strTexture);
        void SetTip(string strTip);
        void SetColor(Color color);
        bool SetText(UInt32 unIndex, string strText);
        void SetEnable(bool bEnable);
        void SetEnableSelect(bool bEnable);
        void Clear();
    }
}
