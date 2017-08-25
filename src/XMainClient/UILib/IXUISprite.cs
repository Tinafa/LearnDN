using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UILib
{
    public delegate void SpriteClickEventHandler(IXUISprite uiSprite);
    public delegate bool SpritePressEventHandler(IXUISprite uiSprite, bool isPressed);
    public delegate bool SpriteDragEventHandler(Vector2 delta);
    public interface IXUISprite : IXUIObject, IUIWidget, IXUICD
    {
        //IXUIAtlas uiAtlas { get; }
        string spriteName { get; set; }
        int  spriteWidth { get; set; }
        int  spriteHeight { get; set; }
        int  spriteDepth { get; set; }
        string atlasPath { get; }
        Vector4  drawRegion { get; set; }

        void SetAlpha(float alpha);
        float GetAlpha();
        bool SetSprite(string strSprite, string strAtlas, bool fullAtlasName = false);
        bool SetSprite(string strSprite);
        void SetEnabled(bool bEnabled);
        void SetGrey(bool bGrey);
        void SetColor(Color c);
        void SetAudioClip(string name);
        void MakePixelPerfect();
        void RegisterSpriteClickEventHandler(SpriteClickEventHandler eventHandler);
        void RegisterSpritePressEventHandler(SpritePressEventHandler eventHandler);
        void RegisterSpriteDragEventHandler(SpriteDragEventHandler eventHandler);

        void SetFillAmount(float val);
        void SetFlipHorizontal(bool bValue);
        void SetFlipVertical(bool bValue);

        SpriteClickEventHandler GetSpriteClickHandler();
        SpritePressEventHandler GetSpritePressHandler();

        void ResetPanel();

        void UpdateAnchors();

        bool IsEnabled();        
    }
}
