using System;
using System.Collections.Generic;
using UnityEngine;

namespace UILib
{
    public delegate void SpriteAnimationFinishCallback(IXUISpriteAnimation iSA);

    public interface IXUISpriteAnimation : IXUIObject
    {
        void SetNamePrefix(string name);

        void SetNamePrefix(string atlas, string name);
        void SetFrameRate(int rate);
        void Reset();
        void StopAndReset();
        void RegisterFinishCallback(SpriteAnimationFinishCallback callback);
        void MakePixelPerfect();   
    }
}
