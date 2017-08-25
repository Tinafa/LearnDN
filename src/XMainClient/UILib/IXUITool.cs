using System;
using System.Collections.Generic;
using UnityEngine;

namespace UILib
{
    public delegate void LoadUIFinishedEventHandler(string location);
    public delegate void LoadUIAsynEventHandler(string strUIFile, LoadUIFinishedEventHandler eventHandler);
    public delegate void AnimFinishedEventHandler();

    public interface IXUITool
    {
        void SetActive(GameObject obj, bool state);
        void SetLayer(GameObject go, int layer);
        void SetUIEventFallThrough(GameObject obj);
        void SetUIGenericEventHandle(GameObject obj);
        void ShowTooltip(string str);
        void RegisterLoadUIAsynEventHandler(LoadUIAsynEventHandler eventHandler);
        Camera GetUICamera();
        void PlayAnim(Animation anim, string strClipName, AnimFinishedEventHandler eventHandler);
        void Destroy(UnityEngine.Object obj);
        void SetUIDepthDelta(GameObject go, int delta);
        string GetLocalizedStr(string key);
        Vector2 CalculatePrintedSize(string text);
        void ReleaseAllDrawCall();
    }
}
