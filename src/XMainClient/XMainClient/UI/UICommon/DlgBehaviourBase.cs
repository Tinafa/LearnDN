using UnityEngine;
using UILib;

namespace XMainClient.UI
{
    public class DlgBehaviourBase : MonoBehaviour, IXUIBehaviour
    {
        public IXUIObject parent
        {
            get { return null; }
            set { }
        }
        public ulong ID
        {
            get { return m_id; }
            set { m_id = value; }
        }

        public bool Exculsive
        {
            get { return m_bExculsive; }
            set { m_bExculsive = value; }
        }

        public bool IsVisible()
        {
            return gameObject.activeInHierarchy;
        }

        public void SetVisible(bool bVisible)
        {
            gameObject.SetActive(bVisible);
        }

        public IXUIDlg uiDlgInterface
        {
            get { return m_uiDlgInterface; }
            set { m_uiDlgInterface = value; }
        }

        public IXUIObject[] uiChilds
        {
            get { return m_uiChilds; }
        }

        public IXUIObject GetUIObject(string strName)
        {
            Transform transform = base.transform.FindChild(strName);
            if (null != transform)
            {
                return transform.GetComponent<XUIObjectBase>();
            }
            return null;
        }

        public virtual void Init()
        {
            m_uiChilds = GetComponentsInChildren<XUIObjectBase>();
            for (int i = 0; i < m_uiChilds.Length; ++i)
            {
                XUIObjectBase o = m_uiChilds[i] as XUIObjectBase;
                o.Init();
            }
        }

        public void OnPress()
        {
            OnFocus();
        }

        public void OnFocus() { }

        public virtual void Highlight(bool bTrue)
        {

        }

        private IXUIDlg m_uiDlgInterface = null;
        private IXUIObject[] m_uiChilds = null;
        private ulong m_id;
        private bool m_bExculsive = false;
    }
}
