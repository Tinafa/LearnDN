using System;
using UnityEngine;

namespace UILib
{
    public delegate void TextureClickEventHandler(IXUITexture uiSprite);

    public interface IXUITexture : IXUIObject
    {
        int spriteWidth { get; set; }
        int spriteHeight { get; set; }
        int spriteDepth { get; set; }
        int aspectRatioSource { get; set; }
        //void SetTexturePath(Texture texture);
        void SetTexturePath(string texPath);
        void SetRuntimeTex(Texture tex);
        //Texture GetTexture();
        void SetUVRect(Rect rect);
        void RegisterLabelClickEventHandler(TextureClickEventHandler eventHandler);
        void SetEnabled(bool bEnabled);
        void SetAlpha(float alpha);
        void MakePixelPerfect();
        TextureClickEventHandler GetTextureClickHandler();
        void  CloseScrollView();
        void SetClickCD(float cd);
    }
}
