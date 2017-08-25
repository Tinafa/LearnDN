using System;
using System.Collections.Generic;
using XUtliPoolLib;
using UnityEngine;
using UILib;

namespace XMainClient
{
    class UIManager : XSingleton<UIManager>
    {
        private Dictionary<string, IXUIDlg> m_dicDlgs = new Dictionary<string, IXUIDlg>();
        private Dictionary<int, List<IXUIDlg>> m_dicUILayer = new Dictionary<int, List<IXUIDlg>>();
        private Dictionary<int, IXUIDlg> m_GroupDlg = new Dictionary<int, IXUIDlg>();

        private Transform m_uiRoot = null;

        private XLFU<IXUIDlg> m_LFU = new XLFU<IXUIDlg>(5);

        private List<IXUIDlg> m_CachedExculsiveUI = new List<IXUIDlg>();
        private Stack<IXUIDlg> m_ShowUIStack = new Stack<IXUIDlg>();
        //private List<IXUIDlg> m_StoreUIList = new List<IXUIDlg>();

        private List<IXUIDlg> m_iterDlgs = new List<IXUIDlg>();
        private List<IXUIDlg> m_ShowedDlg = new List<IXUIDlg>();
        private List<IXUIDlg> m_ToBeUnloadDlg = new List<IXUIDlg>();

        public int unloadUICount = 0;
        public Transform UIRoot
        {
            get { return m_uiRoot; }
            set
            {
                m_uiRoot = value;
            }
        }

        public void OnEnterScene()
        {
            m_LFU.Clear();
            m_CachedExculsiveUI.Clear();
            m_ShowUIStack.Clear();
            //m_StoreUIList.Clear();
            m_iterDlgs.Clear();
            m_ShowedDlg.Clear();
            m_ToBeUnloadDlg.Clear();
            unloadUICount = 0;
        }

        public void OnLeaveScene(bool transfer)
        {
            for (int i = m_iterDlgs.Count - 1; i >= 0; --i)
            {
                if (i < m_iterDlgs.Count && m_iterDlgs[i] != null)  
                    m_iterDlgs[i].UnLoad(transfer);
            }
            unloadUICount = 0;
        }

        public override bool Init()
        {
            int size = 5; // Change with config.
            m_LFU = new XLFU<IXUIDlg>(size);
            return true;
        }

        public override void Uninit()
        {

        }

        public void LoadUI(string strUIFile, LoadUIFinishedEventHandler eventHandler)
        {
            if (null != eventHandler)
            {
                eventHandler(strUIFile);
            }
        }

        public void Update(float fDeltaT)
        {
            for (int i = 0; i < m_iterDlgs.Count; i++)
            {
                IXUIDlg dlg = m_iterDlgs[i];
                if (dlg != null)
                {
                    if (dlg.uiBehaviourInterface == null || dlg.uiBehaviourInterface.gameObject == null)
                    {
                        XDebug.singleton.AddErrorLog("UI missing: ", dlg.fileName);
                        continue;
                    }
                    if (dlg.uiBehaviourInterface.gameObject.activeInHierarchy)
                        dlg.OnUpdate();
                }

            }
        }

        public void PostUpdate(float fDeltaT)
        {
            for(int i=0;i< m_iterDlgs.Count; ++i)
            {
                if (m_iterDlgs[i] != null)
                    m_iterDlgs[i].OnPostUpdate();
            }

            if(m_ToBeUnloadDlg.Count > 0)
            {
                for (int i = 0; i< m_ToBeUnloadDlg.Count; ++i)
                {
                    m_ToBeUnloadDlg[i].UnLoad();
                }
                m_ToBeUnloadDlg.Clear();
            }
        }

        public bool IsUIShowed()
        {
            if (m_ShowUIStack.Count > 0) return true;
            return false;
        }

        public int GetFullScreenUICount()
        {
            int i = 0;
            foreach (IXUIDlg dlg in m_ShowUIStack)
            {
                if (dlg.fullscreenui) i++;
            }

            return i;
        }

        public void RemoveDlg(IXUIDlg dlg)
        {
            List<IXUIDlg> listDlg = null;
            if (m_dicUILayer.TryGetValue(dlg.layer, out listDlg))
            {
                listDlg.Remove(dlg);
            }

            if (m_dicDlgs.ContainsKey(dlg.fileName))
            {
                m_dicDlgs.Remove(dlg.fileName);
                m_iterDlgs.Remove(dlg);
            }

            if (m_GroupDlg.ContainsKey(dlg.group))
            {
                m_GroupDlg.Remove(dlg.group);
            }

            if (m_ShowedDlg.Contains(dlg))
            {
                m_ShowedDlg.Remove(dlg);
            }

            m_LFU.Remove(dlg);
        }

        public bool AddDlg(IXUIDlg dlg)
        {
            if (m_dicDlgs.ContainsKey(dlg.fileName))
            {
                XDebug.singleton.AddLog("true == m_dicDlgs.ContainsKey(dlg.fileName): ", dlg.fileName);
                return false;
            }

            m_dicDlgs.Add(dlg.fileName, dlg);
            m_iterDlgs.Add(dlg);

            List<IXUIDlg> listDlg = null;
            if (m_dicUILayer.TryGetValue(dlg.layer, out listDlg) == true)
            {
                listDlg.Add(dlg);
            }
            else
            {
                listDlg = new List<IXUIDlg>();
                listDlg.Add(dlg);
                m_dicUILayer.Add(dlg.layer, listDlg);
            }
            return true;
        }

        protected void CacheExclusiveUI()
        {
            m_CachedExculsiveUI.Clear();

            for (int i = 0; i < m_ShowedDlg.Count; i++)
            {
                m_CachedExculsiveUI.Add(m_ShowedDlg[i]);
            }
        }

        public void CloseAllUI() {
            ClearUIinStack();

            // 不在uistack里面的UI
            List<IXUIDlg> toBeClose = new List<IXUIDlg>();
            for (int i = 0; i < m_ShowedDlg.Count; i++)
            {
                if (!m_ShowedDlg[i].isMainUI)
                    toBeClose.Add(m_ShowedDlg[i]);
            }

            for (int i = 0; i < toBeClose.Count; i++)
            {
                toBeClose[i].SetVisible(false);
            }
        }

        public void ClearUIinStack()
        {
            while (m_ShowUIStack.Count > 0)
            {
                IXUIDlg dlg = m_ShowUIStack.Peek();
                if (dlg != null)
                {
                    // 防止栈顶GameObject被隐藏，栈顶元素无法Pop导致死循环
                    dlg.SetVisiblePure(true);


                    dlg.SetVisible(false);
                }
            }

            UIBlurEffect(false);
        }

        public void OnDlgShow(IXUIDlg dlg)
        {
            if (dlg.exclusive)
            {
                CloseAllUI();

                CacheExclusiveUI();

                for (int i = 0; i < m_CachedExculsiveUI.Count; i++)
                {
                    m_CachedExculsiveUI[i].uiBehaviourInterface.uiDlgInterface.SetVisiblePure(false);
                }
            }
            else
            {
                if (!m_ShowedDlg.Contains(dlg))
                {
                    m_ShowedDlg.Add(dlg);
                }
            }

            if (dlg.pushstack)
            {
                IXUIDlg autoUnloadDlg = m_LFU.Add(dlg);
                if (autoUnloadDlg != null)
                {
                    m_ToBeUnloadDlg.Add(autoUnloadDlg);
                    XDebug.singleton.AddGreenLog("Auto Unload UI: ", autoUnloadDlg.fileName, " while opening ", dlg.fileName);
                }
            }

            if (dlg.hideMainMenu)
            {
                UIBlurEffect(true);
            }

            if (dlg.pushstack)
            {
                if (m_ShowUIStack.Count > 0)
                {
                    IXUIDlg top = m_ShowUIStack.Peek();
                    top.LeaveStackTop();
                    top.uiBehaviourInterface.uiDlgInterface.SetVisiblePure(false);

                    //防止循环入栈
                    Stack<IXUIDlg> temp = new Stack<IXUIDlg>();

                    IXUIDlg tempTop = m_ShowUIStack.Pop();
                    while (tempTop != dlg && m_ShowUIStack.Count > 0)
                    {
                        temp.Push(tempTop);
                        tempTop = m_ShowUIStack.Pop();
                    }

                    if (tempTop != dlg) m_ShowUIStack.Push(tempTop);

                    while (temp.Count > 0)
                    {
                        m_ShowUIStack.Push(temp.Pop());
                    }

                }

                m_ShowUIStack.Push(dlg);
            }

        }

        public void OnDlgHide(IXUIDlg dlg)
        {
            if (dlg.exclusive)
            {
                for (int i = 0; i < m_CachedExculsiveUI.Count; i++)
                {
                    m_CachedExculsiveUI[i].uiBehaviourInterface.uiDlgInterface.SetVisiblePure(true);
                }
            }
            else
            {
                m_ShowedDlg.Remove(dlg);
            }

            if (dlg.pushstack)
                m_LFU.MarkCanPop(dlg, true);

            if (dlg.hideMainMenu)
            {
                UIBlurEffect(false);
            }

            if (dlg.pushstack && m_ShowUIStack.Count > 0)
            {
                IXUIDlg peek = m_ShowUIStack.Peek();
                if (peek != dlg)
                {
                    Stack<IXUIDlg> temp = new Stack<IXUIDlg>();

                    IXUIDlg tempTop = m_ShowUIStack.Pop();
                    while (tempTop != dlg)
                    {
                        temp.Push(tempTop);

                        if (m_ShowUIStack.Count == 0)
                        {
                            System.Text.StringBuilder sb = XCommon.singleton.sb;
                            sb.Length = 0;
                            sb.Append("Hide UI not in stack!!!! : ").Append(dlg.fileName);
                            sb.Append("; UIs in stack: ");
                            foreach (IXUIDlg d in temp)
                            {
                                sb.Append(d.fileName).Append(" ");
                            }

                            XDebug.singleton.AddErrorLog(sb.ToString());
                            break;
                        }
                        tempTop = m_ShowUIStack.Pop();
                    }
                    while (temp.Count > 0)
                    {
                        m_ShowUIStack.Push(temp.Pop());
                    }

                    if (m_ShowUIStack.Count > 0)
                    {
                        IXUIDlg top = m_ShowUIStack.Peek();
                        if (top.hideMainMenu)
                        {
                            UIBlurEffect(true);
                        }
                    }
                }
                else
                {
                    m_ShowUIStack.Pop();
                    if (m_ShowUIStack.Count > 0)
                    {
                        IXUIDlg top = m_ShowUIStack.Peek();
                        top.uiBehaviourInterface.uiDlgInterface.SetVisiblePure(true);
                        top.StackRefresh();


                        if (top.hideMainMenu)
                        {
                            UIBlurEffect(true);
                        }
                    }

                }
            }
        }

        public List<IXUIDlg> GetShowedUI()
        {
            return m_ShowedDlg;
        }

        void UIBlurEffect(bool bOn) { }
    }
}
