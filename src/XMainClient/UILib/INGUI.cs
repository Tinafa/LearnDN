using System;
using System.Collections.Generic;
using UnityEngine;

namespace UILib
{
    public interface IUIRect
    {
        Transform transform { get; }
    }
    public interface IUIWidget : IUIRect
    {
        IXUIPanel GetPanel();
    }

    public delegate void RefreshRenderQueueCb(int rq);

    public interface IUIDummy : IUIWidget
    {
        int RenderQueue { get; }

        RefreshRenderQueueCb RefreshRenderQueue { get; set; }

        void Reset();

        int depth { get; set; }

        float alpha { get; set; }
    }

    public interface IUIBloodGrid : IUIWidget
    {
        void SetMAXHP(int maxHp);

        int MAXHP { get; }
    }

    public interface IUI3DFollow
    {
        void SetPos(Vector3 pos);
    }

    public interface IUIPanel : IUIRect
    {

    }
}
