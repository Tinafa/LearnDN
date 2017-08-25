using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UILib
{
    public delegate void LabelClickEventHandler(IXUILabel uiSprite);

    public interface IXUILabel : IXUIObject
    {
        float AlphaVar { get; }
        float Alpha { get; set; }
        int spriteWidth { get; set; }
        int spriteHeight { get; }
        int spriteDepth { get; set; }
        int fontSize { get; set; }
        Color GetColor();
        string GetText();
        void SetText(string strText);
        void SetColor(Color c);
        void SetEffectColor(Color c);
        void SetGradient(bool bEnable, Color top, Color bottom);
        void ToggleGradient(bool bEnable);
        void SetEnabled(bool bEnabled);
        Vector2 GetPrintSize();
        void SetDepthOffset(int d);
        void MakePixelPerfect();

        void RegisterLabelClickEventHandler(LabelClickEventHandler eventHandler);

        void SetIdentity(int i);
        bool HasIdentityChanged(int i);
    }
}
