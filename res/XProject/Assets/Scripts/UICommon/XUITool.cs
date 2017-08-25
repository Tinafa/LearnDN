using UILib;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using XUtliPoolLib;

public class XUITool : MonoBehaviour, IXUITool
{
    public static XUITool Instance { get { return s_instance; } }

    private Dictionary<int, uint> _TweenFadeInGroupDelayNum = new Dictionary<int, uint>();
    private IXGameUI m_gameui = null;
    private GameObject m_preloadBillboard = null;
    private GameObject m_preloadTitle = null;

    public IXGameUI XGameUI
    {
        get
        {
            if (m_gameui == null)
            {
                m_gameui = XInterfaceMgr.singleton.GetInterface<IXGameUI>(XCommon.singleton.XHash("XGameUI"));
            }
            return m_gameui;
        }
    }
    public Camera GetUICamera()
    {
        return UICamera.currentCamera;
    }

    public void SetActive(GameObject obj, bool state)
    {
        NGUITools.SetActive(obj, state);
    }

    public void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;
        Transform t = go.transform;
        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            SetLayer(child.gameObject, layer);
        }
    }

    public void SetUIDepthDelta(GameObject go, int delta)
    {
        /*XUISprite[] sp = go.GetComponentsInChildren<XUISprite>();
        XUILabel[] la = go.GetComponentsInChildren<XUILabel>();
        XUITexture[] te = go.GetComponentsInChildren<XUITexture>();

        for (int i = 0; i < sp.Length; ++i)
        {
            sp[i].spriteDepth += delta;
        }

        for (int i = 0; i < la.Length; ++i)
        {
            la[i].spriteDepth += delta;
        }

        for (int i = 0; i < te.Length; ++i)
        {
            te[i].spriteDepth += delta;
        }*/
    }

    public void SetUIEventFallThrough(GameObject obj)
    {
        UICamera.fallThrough = obj;
    }

    public void SetUIGenericEventHandle(GameObject obj)
    {
        UICamera.genericEventHandler = obj;
    }

    public void ShowTooltip(string str)
    {
        UITooltip.ShowText(str);
    }

    public void RegisterLoadUIAsynEventHandler(LoadUIAsynEventHandler eventHandler)
    {
        m_loadUIAsynEventHandler = eventHandler;
    }

    public void LoadResAsyn(string strFile, LoadUIFinishedEventHandler eventHandler)
    {
        if (null != m_loadUIAsynEventHandler)
        {
            //m_loadUIAsynEventHandler(strFile, eventHandler);
        }
        else
        {
            Debug.LogError("null == m_loadUIAsynEventHandler");
        }

    }

    public void SetCursor(string strSpriteName)
    {
        //UICursor.Set(strSpriteName);	
    }

    public void SetCursor(string strSprite, string strAtlas)
    {

    }

    public void PlayAnim(Animation anim, string strClipName, AnimFinishedEventHandler eventHandler)
    {
        if (null == anim || null == strClipName || strClipName.Length == 0)
        {
            eventHandler();
            return;
        }
    }

    public string GetLocalizedStr(string key)
    {
        return Localization.Get(key);
    }

    void IXUITool.Destroy(UnityEngine.Object obj)
    {
        NGUITools.Destroy(obj);
    }

    public Vector2 CalculatePrintedSize(string text)
    {
        return NGUIText.CalculatePrintedSize(text);
    }

    public void ReleaseAllDrawCall()
    {
        UIDrawCall.ReleaseInactive();
    }

    private LoadUIAsynEventHandler m_loadUIAsynEventHandler = null;
    //private string m_spriteNameForCusorCached = "";
    static XUITool s_instance = null;
}
