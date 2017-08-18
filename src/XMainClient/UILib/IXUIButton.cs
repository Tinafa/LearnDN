using System;
using UnityEngine;

namespace UILib
{
    public delegate bool ButtonClickEventHandler(IXUIButton button);
    public delegate void ButtonPressEventHandler(IXUIButton button, bool state);
    public delegate void ButtonDragEventHandler(IXUIButton button, Vector2 delta);

    public interface IXUIButton : IXUIObject, IXUICD
    {
        int spriteWidth { get; }
        int spriteHeight { get; }
        int spriteDepth { get; set; }

        void SetCaption(string strText);
        void SetEnable(bool bEnable, bool withcollider = false);
        void SetGrey(bool bGrey);
        void SetAlpha(float f);
        void CloseScrollView();
        void RegisterClickEventHandler(ButtonClickEventHandler eventHandler);
        void RegisterPressEventHandler(ButtonPressEventHandler eventHandler);
        void RegisterDragEventHandler(ButtonDragEventHandler eventHandler);
        void SetSpriteWithPrefix(string prefix);
        void SetAudioClip(string name);
        void SetSprites(string normal, string hover, string press);
        ButtonClickEventHandler GetClickEventHandler();
        ButtonPressEventHandler GetPressEventHandler();
        void ResetState();

        void ResetPanel();

        void SetUnavailableCD(int cd);
    }
}
