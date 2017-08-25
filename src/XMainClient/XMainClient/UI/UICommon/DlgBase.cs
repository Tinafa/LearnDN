using System;
using System.Collections.Generic;
using UILib;
using UnityEngine;
using XUtliPoolLib;

namespace XMainClient.UI
{
    public abstract class DlgBase<TDlgClass, TUIBehaviour>:IXUIDlg
        where TDlgClass : IXUIDlg, new()
        where TUIBehaviour : DlgBehaviourBase
    {
        public static TDlgClass singleton
        {
            get
            {
                if (null == s_instance)
                {
                    lock (s_objLock)
                    {
                        if (null == s_instance)
                        {
                            s_instance = new TDlgClass();
                        }
                    }
                }
                return s_instance;
            }
        }

        public IXUIBehaviour uiBehaviourInterface
        {
            get { return m_uiBehaviour; }
        }

        public TUIBehaviour uiBehaviour
        {
            get { return m_uiBehaviour; }
        }

        public virtual string fileName
        {
            get { return ""; }
        }

        public virtual int layer
        {
            get { return 2; }
        }

        public virtual int group
        {
            get { return 0; }
        }

        public virtual bool exclusive
        {
            get { return false; }
        }

        public virtual bool autoload
        {
            get { return false; }
        }

        public virtual bool hideMainMenu
        {
            get { return false; }
        }
        public virtual bool pushstack
        {
            get { return false; }
        }

        public virtual bool isMainUI
        {
            get { return false; }
        }

        public virtual int sysid
        {
            get { return 0; }
        }

        public virtual bool fullscreenui
        {
            get { return false; }
        }

        public bool Prepared
        {
            get { return (null != m_uiBehaviour); }
        }

        private LoadUIFinishedEventHandler _loadUICb = null;
        public DlgBase()
        {
            _loadUICb = new LoadUIFinishedEventHandler(OnLoadUIFinishedEventHandler);
        }

        protected virtual void Init()
        {

        }

        private void InnerInit()
        {
            m_uiBehaviour.Init();
            Vector3 localPos = uiBehaviour.transform.localPosition;
            localPos.z = m_fDepthZ;
            uiBehaviour.transform.localPosition = localPos;
            //m_DlgController = uiBehaviour.transform.FindChild("DlgController");
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnPostUpdate()
        {
        }

        //不走OnShow OnHide逻辑
        public void SetVisiblePure(bool bVisible)
        {
            if (m_bLoaded == false && autoload)
            {
                Load();
            }
            else if (false == m_bLoaded && !autoload)
            {
                return;
            }

            uiBehaviour.SetVisible(bVisible);

            m_bVisible = bVisible;

            OnSetVisiblePure(bVisible);

        }
        public virtual void SetVisible(bool bIsVisible, bool bEnableAuto = true)
        {
            if(!m_bLoaded)
            {
                if (autoload && bEnableAuto)
                {
                    Load();
                }
                else
                {
                    return;
                }
            }

            if (true == Prepared)
            {
                if (m_bVisible != bIsVisible)
                {
                    uiBehaviour.SetVisible(bIsVisible);
                    m_bVisible = bIsVisible;

                    if(m_bVisible)
                    {
                        UIManager.singleton.OnDlgShow(s_instance);
                        OnShow();
                    }else
                    {
                        OnHide();
                        UIManager.singleton.OnDlgHide(s_instance);
                    }
                }
            }

        }

        public void Load()
        {
            if (false == m_bLoaded)
            {
                m_bLoaded = true;

                UIManager.singleton.LoadUI(fileName, _loadUICb);

                UIManager.singleton.AddDlg(s_instance);

                OnLoad();
            }
        }

        public bool IsLoaded()
        {
            return m_bLoaded;
        }

        public void UnLoad(bool bTransfer = false)
        {
            if (m_bLoaded)
            {
                if (bTransfer)
                {
                    SetVisible(false, false);
                }
                else
                {
                    OnUnload();

                    UIManager.singleton.RemoveDlg(s_instance);

                    XResourceLoaderMgr.singleton.UnSafeDestroy(uiBehaviour.gameObject, false);
                    m_uiBehaviour = null;
                    m_uiBehaviour = default(TUIBehaviour);
                    m_bLoaded = false;

                    ///..
                    if (!XGame.singleton.switchScene)
                    {
                        if (UIManager.singleton.unloadUICount >= 10)
                        {
                            Resources.UnloadUnusedAssets();
                            UIManager.singleton.unloadUICount = 0;
                        }
                        else
                        {
                            UIManager.singleton.unloadUICount++;
                        }
                    }
                }
            }
        }

        public bool IsVisible()
        {
            if (m_bLoaded)
            {
                return uiBehaviour.IsVisible();
            }

            return false;
        }

        public void SetDepthZ(int nDepthZ)
        {
            m_fDepthZ = nDepthZ * 10; // Max for 48
            if (true == Prepared)
            {
                Vector3 localPos = uiBehaviour.transform.localPosition;
                localPos.z = m_fDepthZ;
                uiBehaviour.transform.localPosition = localPos;
            }
        }

        public virtual void Reset()
        {

        }

        public virtual void StackRefresh()
        {

        }

        public virtual void LeaveStackTop()
        {

            
        }
        public virtual void RegisterEvent()
        {

        }

        protected virtual void UnRegisterEvent()
        {

        }
        protected virtual void OnShow() { }

        protected virtual void OnHide() { }

        protected virtual void OnLoad() { }

        protected virtual void OnUnload()
        {
            UnRegisterEvent();
        }
        protected virtual void OnSetVisiblePure(bool bShow)
        {

        }

        private void OnLoadUIFinishedEventHandler(string location)
        {
            GameObject objUI;

            objUI = XResourceLoaderMgr.singleton.CreateFromPrefab("UI/" + location) as GameObject;

            if (null != objUI)
            {
                objUI.transform.parent = UIManager.singleton.UIRoot;
                objUI.transform.localPosition = new Vector3(0.0f, 0.0f, 0);
                objUI.transform.localScale = new Vector3(1, 1, 1);
                m_uiBehaviour = objUI.AddComponent<TUIBehaviour>();
                m_uiBehaviour.uiDlgInterface = this;


                Init();
                InnerInit();
                RegisterEvent();

                uiBehaviour.SetVisible(false);
                m_bVisible = false;
            }
        }

        public void SetAlpha(float a)
        {
            IXUIPanel panel = uiBehaviour.gameObject.GetComponent("XUIPanel") as IXUIPanel;

            if(panel != null)
                panel.SetAlpha(a);
        }

        public float GetAlpha()
        {
            IXUIPanel panel = uiBehaviour.gameObject.GetComponent("XUIPanel") as IXUIPanel;

            if (panel != null)
                return panel.GetAlpha();
            return 1f;
        }

        /// <summary>
        /// XNGUI点击回调方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        public virtual void OnXNGUIClick(GameObject obj, string path)
        {
            XDebug.singleton.AddLog(obj.name, " ", path);
        }

        protected TUIBehaviour m_uiBehaviour = null;
        private static TDlgClass s_instance = default(TDlgClass);
        private static object s_objLock = new object();
        //private Transform m_DlgController = null;
        private bool m_bVisible = false;
        protected bool m_bLoaded = false;
        private float m_fDepthZ = 0.0f;
    }
}
