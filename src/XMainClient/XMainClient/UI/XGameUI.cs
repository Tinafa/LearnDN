using System;
using System.Collections.Generic;
using XUtliPoolLib;
using UnityEngine;
using UILib;

namespace XMainClient
{
    class XGameUI : XSingleton<XGameUI> , IXGameUI
    {
        public int Base_UI_Width { get; set; }
        public int Base_UI_Height { get; set; }

        private Transform m_objUIRoot = null;
        private UnityEngine.Camera m_uiCamera = null;

        private IXUISprite m_overlay = null;

        public static int _far_far_away = 1000;
        public static Vector3 Far_Far_Away = new Vector3(10000, 10000, 0);

        private GameObject[] _buttonTpl = new GameObject[3];
        private GameObject[] _spriteTpl = new GameObject[1];
        private GameObject _dlgControllerTpl = null;

        public void OnGenericClick()
        {

        }

        public Transform UIRoot {
            get
            {
                return m_objUIRoot;
            }set
            {
                m_objUIRoot = value;

                UIManager.singleton.UIRoot = m_objUIRoot;
            }
        }

        public override bool Init()
        {
            XInterfaceMgr.singleton.AttachInterface<IXGameUI>(XCommon.singleton.XHash("XGameUI"), this);

            if (UIRoot == null)
            {
                UIRoot = (XResourceLoaderMgr.singleton.CreateFromPrefab("UI/Common/UIRoot", false, true) as GameObject).transform;
                GameObject.DontDestroyOnLoad(UIRoot);
            }

            return true;
        }

        public GameObject DlgControllerTpl
        {
            get { return _dlgControllerTpl; }
        }

        public GameObject[] buttonTpl
        {
            get { return _buttonTpl; }
        }

        public GameObject[] spriteTpl
        {
            get { return _spriteTpl; }
        }

        public void SetOverlayAlpha(float alpha)
        {
            if (alpha > 0)
            {
                if (!m_overlay.gameObject.activeSelf) m_overlay.gameObject.SetActive(true);
                m_overlay.SetAlpha(alpha);
            }
            else
            {
                m_overlay.gameObject.SetActive(false);
            }
        }

        public void GetOverlay()
        {
            if (m_overlay == null)
            {
                m_overlay = m_objUIRoot.FindChild("fade_panel/fade_canvas").GetComponent("XUISprite") as IXUISprite;
                m_overlay.gameObject.SetActive(false);
            }
        }
        public float GetOverlayAlpha()
        {
            GetOverlay();

            return m_overlay.gameObject.activeSelf ? m_overlay.GetAlpha() : 0;
        }

        public UnityEngine.Camera UICamera
        {
            get { return m_uiCamera; }
            set
            {
                m_uiCamera = value;
            }
        }

        public bool Deprecated
        {
            get;set;
        }
    }
}
