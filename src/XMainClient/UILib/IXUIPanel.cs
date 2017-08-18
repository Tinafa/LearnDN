using System;
using System.Collections.Generic;
using UnityEngine;

namespace UILib
{
    public interface IXUIPanel : IXUIObject
    {
        void SetSize(float width, float height);

        void SetAlpha(float a);

        float GetAlpha();

        void SetDepth(int d);
        int GetDepth();

        Vector2 offset { get; set; }
        Vector2 softness { get; set; }
        Vector4 ClipRange { get; set; }
        Vector4 GetBaseRect();

        Action onMoveDel { get; set; }

        Component UIComponent { get; }
    }


}
