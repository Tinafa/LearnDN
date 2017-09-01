using System;
using System.Collections.Generic;
using UnityEngine;
using XUtliPoolLib;

namespace XMainClient
{
    public class XLocalPRSAsyncData
    {
        public Vector3 localPos = Vector3.zero;
        public Quaternion localRotation = Quaternion.identity;
        public Vector3 localScale = Vector3.one;
        public short mask = 0;
        public void Reset()
        {
            localPos = Vector3.zero;
            localRotation = Quaternion.identity;
            localScale = Vector3.one;
            mask = 0;
            CommonObjectPool<XLocalPRSAsyncData>.Release(this);
        }
    }

    public class XEngineObject
    {
        protected GameObject m_GameObject = null;
        protected GameObject m_Parent = null;
        public string m_Location = "";
    }

    public delegate void ResetCallback();
    public delegate void LoadCallback(XGameObject gameObject);
    //public delegate void AnimLoadCallback(XAnimator ator);
    public delegate void CommandCallback(XGameObject gameObject, object obj, int commandID);

    public class XGameObject : XEngineObject
    {
        enum ECallbackCmd
        {
            ESyncPosition = 0x00000001,
            ESyncRotation = 0x00000002,
            ESyncScale = 0x00000004,
            ESyncLayer = 0x00000008,
            ESyncCCEnable = 0x00000010,
            ESyncBCEnable = 0x00000020,
            ESyncUpdateWhenOffscreen = 0x00000040,
            ESyncActive = 0x00000080,
            ESyncTag = 0x00000100,
            ESyncCCStepOffset = 0x00000200,
        }

        private bool m_Valid = false;
        private Vector3 m_Position = Vector3.one * 1000;
        private Quaternion m_Rotation = Quaternion.identity;
        private Vector3 m_Scale = Vector3.one;

        private bool m_EnableCC = true;
        private bool m_EnableBC = true;
        private float m_CCStepOffset = 0.0f;
        private bool m_UpdateWhenOffscreen = false;
        //private bool m_EnableRender = true;
        //private string m_TagFilter = "";
        private string m_Tag = "Untagged";

        private int m_Layer = 0;
        private ulong m_UID = 0;
        private string m_Name = "";

        private Transform m_TransformCache = null;
        private SkinnedMeshRenderer m_SkinMeshRenderCache = null;
        private CharacterController m_ccCache = null;
        private BoxCollider m_bcCache = null;

        private Rigidbody2D m_Rigid2d = null;
        private BoxCollider2D m_Box2d = null;
        private Animator m_animator = null;

        public int objID = -1;
        public static int globalObjID = 0;

        public static string EmptyObject = "";
        //private XAnimator m_Ator = null;
        //private short m_UpdateFrame = 0;
        private LoadAsyncTask loadTask = null;
        private LoadCallBack loadCb = null;
        private short m_LoadStatus = 0;//0 not load 1 loading finish 2 load finish

        private int m_LoadFinishCbFlag = 0;

        private static CommandCallback SyncSyncSetParentTransCmd = SyncSetParentTrans;
        private static CommandCallback SyncLocalPRSCmd = SyncLocalPRS;
        private static LoadCallback[] loadCallbacks = null;
        private static CommandCallback _setPhysicTransformCb = _SetPhysicTransform;

        public XGameObject()
        {
            loadCb = LoadFinish;
            if (loadCallbacks == null)
            {
                loadCallbacks = new LoadCallback[] {
                    SyncPosition ,
                    SyncRotation ,
                    SyncScale ,
                    SyncLayer ,
                    SyncCCEnable ,
                    SyncBCEnable ,
                    //SyncUpdateWhenOffscreen ,
                    SyncActive ,
                    SyncTag,
                    SyncCCOffset,
                };
            }
        }

        public Rigidbody2D Rigid2d
        {
            get {
                return m_Rigid2d;
            }
        }

        public BoxCollider2D Box2d
        {
            get
            {
                return m_Box2d;
            }
        }

        public Animator Anim
        {
            get { return m_animator; }
        }

        public Transform transform
        {
            get
            {
                return m_GameObject == null ? null : m_GameObject.transform;
            }
        }

        public Vector3 Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = value;
                AppendPositionCommand();
            }
        }

        public Vector3 LocalEulerAngles
        {
            get
            {
                if (Trans != null)
                {
                    return Trans.localEulerAngles;
                }
                return Vector3.zero;
            }
            set
            {
                if (Trans != null)
                {
                    Trans.localEulerAngles = value;
                }
            }
        }

        public Vector3 LocalScale
        {
            get
            {
                return m_Scale;
            }
            set
            {
                m_Scale = value;
                AppendScaleCommand();
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return m_Rotation;
            }
            set
            {
                m_Rotation = value;
                AppendRotationCommand();
            }
        }
 
        public int Layer
        {
            get
            {
                return m_Layer;
            }
            set
            {
                m_Layer = value;
                AppendLayerCommand();
            }
        }

        public float CCStepOffset
        {
            get
            {
                return m_CCStepOffset;
            }
            set
            {
                m_CCStepOffset = value;
                AppendCCStepOffsetCommand();

            }
        }

        public bool EnableCC
        {
            get
            {
                return m_EnableCC;
            }
            set
            {
                m_EnableCC = value;
                AppendEnableCCCommand();
            }
        }

        public bool EnableBC
        {
            get
            {
                return m_EnableBC;
            }
            set
            {
                m_EnableBC = value;
                AppendEnableBCommand();
            }
        }

        public bool UpdateWhenOffscreen
        {
            set
            {
                m_UpdateWhenOffscreen = value;
                AppendUpdateWhenOffscreen();
            }
        }

        public Vector3 Forward
        {
            get
            {
                return m_Rotation * Vector3.forward;
            }
            set
            {
                m_Rotation = Quaternion.LookRotation(value);
                AppendRotationCommand();
            }
        }

        public Vector3 Up
        {
            get
            {
                return m_Rotation * Vector3.up;
            }
            set
            {
                m_Rotation = Quaternion.FromToRotation(Vector3.up, value);
                AppendRotationCommand();
            }
        }

        public Vector3 Right
        {
            get
            {
                return m_Rotation * Vector3.right;
            }
            set
            {
                m_Rotation = Quaternion.FromToRotation(Vector3.right, value);
                AppendRotationCommand();
            }
        }

        public string Tag
        {
            get
            {
                return m_Tag;
            }
            set
            {
                m_Tag = value;
                AppendTagCommand();
            }
        }

        private Transform Trans
        {
            get
            {
                if (IsLoaded)
                {
                    if (m_GameObject != null && m_TransformCache == null)
                    {
                        m_TransformCache = m_GameObject.transform;
                    }
                }
                return m_TransformCache;
            }
        }

        public ulong UID
        {
            get
            {
                return m_UID;
            }
            set
            {
                m_UID = value;
            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
                if (m_GameObject != null)
                {
                    m_GameObject.name = m_Name;
                }
            }
        }

        public bool IsValid
        {
            get
            {
                return m_Valid;
            }
        }

        public bool IsVisible
        {
            get
            {
                if (m_SkinMeshRenderCache != null)
                    return m_SkinMeshRenderCache.isVisible;
                return false;
            }
        }

        public bool IsNotEmptyObject
        {
            get
            {
                return !string.IsNullOrEmpty(m_Location);
            }
        }

        public bool HasSkin
        {
            get
            {
                return m_SkinMeshRenderCache != null;
            }
        }

        public bool IsLoaded
        {
            get
            {
                return m_LoadStatus == 1;

            }
        }


        public Transform Find(string name)
        {
            if (Trans != null && !string.IsNullOrEmpty(name))
            {
                return Trans.Find(name);
            }
            return Trans;
        }
        public GameObject Get()
        {
            return m_GameObject;
        }
        public void Rotate(Vector3 axis, float degree)
        {
            if (Trans != null)
            {
                Trans.Rotate(axis, degree);
            }
        }

        private void SetCbFlag(ECallbackCmd cmd, bool add)
        {
            int flag = XEnum2Int<ECallbackCmd>.ToInt(cmd);
            if (add)
            {
                m_LoadFinishCbFlag |= flag;
            }
            else
            {
                m_LoadFinishCbFlag &= ~flag;
            }
        }

        private bool IsCbFlag(int index)
        {
            int flag = 1 << index;
            return (m_LoadFinishCbFlag & flag) != 0;
        }

        public static int GetGlobalObjID()
        {
            globalObjID++;
            if (globalObjID > 1000000)
            {
                globalObjID = 0;
            }
            return globalObjID;
        }

        public static XGameObject CreateXGameObject(string location, Vector3 position, Quaternion rotation, bool async = true, bool usePool = true)
        {
            XGameObject gameObject = CreateXGameObject(location, async, usePool);
            gameObject.Position = position;
            gameObject.Rotation = rotation;
            return gameObject;
        }

        public static XGameObject CreateXGameObject(string location, bool async = true, bool usePool = true)
        {
            XGameObject gameObject = CommonObjectPool<XGameObject>.Get();
            gameObject.objID = GetGlobalObjID();
            gameObject.m_Location = location;

            if (string.IsNullOrEmpty(location))
            {
                //> 优化 池装
                GameObject go = new GameObject("EmptyObject");
                gameObject.LoadFinish(go, null);
            }
            else
            {
                if (/*XResourceLoaderMgr.singleton.DelayLoad &&*/ async)
                {
                    //gameObject.LoadAsync(location, usePool);
                    gameObject.Load(location, usePool);
                }
                else
                {
                    gameObject.Load(location, usePool);
                }
            }

            return gameObject;
        }

        private void LoadFinish(UnityEngine.Object obj, System.Object cbOjb)
        {
            m_GameObject = obj as GameObject;
            m_LoadStatus = 1;
            if (m_GameObject != null)
            {
                m_ccCache = m_GameObject.GetComponent<CharacterController>();
                m_bcCache = m_GameObject.GetComponent<BoxCollider>();
                m_Rigid2d = m_GameObject.GetComponent<Rigidbody2D>();
                m_Box2d = m_GameObject.GetComponent<BoxCollider2D>();
                m_animator = m_GameObject.GetComponent<Animator>();
                m_SkinMeshRenderCache = m_GameObject.GetComponentInChildren<SkinnedMeshRenderer>();
                if (!string.IsNullOrEmpty(m_Name))
                    m_GameObject.name = m_Name;
//                 if (m_Ator != null)
//                 {
//                     m_UpdateFrame = 0;
//                     m_Ator.Init(m_GameObject);
//                 }
            }
            for (int i = 0; i < loadCallbacks.Length; ++i)
            {
                if (IsCbFlag(i))
                {
                    LoadCallback cb = loadCallbacks[i];
                    cb(this);
                }
            }
            m_LoadFinishCbFlag = 0;
            if (m_GameObject != null)
            {
                if (m_Layer != m_GameObject.layer)
                {
                    m_Layer = m_GameObject.layer;
                }
            }
        }

        public void LoadAsync(string location, bool usePool)
        {
//             loadTask = XResourceLoaderMgr.singleton.CreateFromPrefabAsync(location, loadCb, null, usePool);
//             if (IsLoaded)
//             {
//                 loadTask = null;
//             }
        }

        public void Load(string location, bool usePool)
        {
            GameObject go = XResourceLoaderMgr.singleton.CreateFromAsset<GameObject>(location, ".prefab", usePool);
            LoadFinish(go, null);
        }

        public void Update()
        {
            SyncPos();
            /*if (m_Ator != null && m_Ator.IsLoaded)
            {
                if (m_UpdateFrame < delayFrameCount)
                {
                    m_UpdateFrame++;
                }
                else if (m_UpdateFrame == delayFrameCount)
                {
                    m_UpdateFrame++;
                    XAnimator.Update(m_Ator);
                }
            }*/
        }

        public void Reset()
        {
            m_Position = XResourceLoaderMgr.Far_Far_Away;
            m_Rotation = Quaternion.identity;
            m_Scale = Vector3.one;
            m_EnableCC = true;
            m_EnableBC = true;
            m_Tag = "Untagged";
            m_Layer = 0;
            m_UID = 0;
            m_TransformCache = null;
            m_SkinMeshRenderCache = null;
            objID = -1;
            if (m_ccCache != null)
            {
                m_ccCache.enabled = false;
                m_ccCache = null;
            }
            if (m_bcCache != null)
            {
                m_bcCache.enabled = false;
                m_bcCache = null;
            }
            m_LoadStatus = 0;
            if (loadTask != null)
            {
                loadTask.CancelLoad(loadCb);
                loadTask = null;
            }

            m_LoadFinishCbFlag = 0;
//             if (m_Ator != null)
//             {
//                 m_Ator.Reset();
//                 CommonObjectPool<XAnimator>.Release(m_Ator);
//                 m_Ator = null;
//             }
            //m_UpdateFrame = 0;

            if (string.IsNullOrEmpty(m_Location))
            {
                //XEngineCommandMgr.singleton.ReturnGameObject(m_GameObject);
                m_GameObject = null;
            }
            else
            {
                if (m_GameObject != null)
                    m_GameObject.name = m_Location;
                XResourceLoaderMgr.SafeDestroy(ref m_GameObject);
            }
            m_Location = "";
            m_Valid = false;
        }

        private static void SyncPosition(XGameObject gameObject)
        {
            Transform t = gameObject.Trans;
            if (t != null)
            {
                t.position = gameObject.m_Position;
            }
        }

        private static void SyncRotation(XGameObject gameObject)
        {
            Transform t = gameObject.Trans;
            if (t != null)
            {
                t.rotation = gameObject.m_Rotation;
            }
        }

        private static void SyncScale(XGameObject gameObject)
        {
            Transform t = gameObject.Trans;
            if (t != null)
            {
                t.localScale = gameObject.m_Scale;
            }
        }

        private static void SyncLayer(XGameObject gameObject)
        {
            if (gameObject.m_GameObject != null)
            {
                gameObject.m_GameObject.layer = gameObject.m_Layer;
            }
        }

        private static void SyncCCOffset(XGameObject gameObject)
        {
            if (gameObject.m_ccCache != null)
            {
                if (gameObject.m_CCStepOffset >= gameObject.m_ccCache.height)
                    gameObject.m_ccCache.height = gameObject.m_CCStepOffset + 0.1f;
                gameObject.m_ccCache.stepOffset = gameObject.m_CCStepOffset;
            }
        }

        private static void SyncCCEnable(XGameObject gameObject)
        {
            if (gameObject.m_ccCache != null)
            {
                gameObject.m_ccCache.enabled = gameObject.m_EnableCC;
            }
        }

        private static void SyncBCEnable(XGameObject gameObject)
        {
            if (gameObject.m_bcCache != null)
            {
                gameObject.m_bcCache.enabled = gameObject.m_EnableBC;
            }
        }

        private static void SyncUpdateWhenOffscreen(XGameObject gameObject)
        {
            /*if (gameObject.m_GameObject != null)
            {
                XCommon.tmpSkinRender.Clear();
                gameObject.m_GameObject.GetComponentsInChildren<SkinnedMeshRenderer>(XCommon.tmpSkinRender);
                int length = XCommon.tmpSkinRender.Count;
                for (int i = 0; i < length; i++)
                    XCommon.tmpSkinRender[i].updateWhenOffscreen = gameObject.m_UpdateWhenOffscreen;
                XCommon.tmpSkinRender.Clear();
            }*/
        }

        private static void SyncActive(XGameObject gameObject)
        {
            /*if (gameObject.m_GameObject != null)
            {
                if (gameObject.IsLoaded)
                {
                    XCommon.tmpRender.Clear();
                    gameObject.m_GameObject.GetComponentsInChildren<Renderer>(XCommon.tmpRender);
                    int length = XCommon.tmpRender.Count;
                    for (int i = 0; i < length; i++)
                    {
                        Renderer r = XCommon.tmpRender[i];
                        if (r.sharedMaterial != null && (gameObject.m_TagFilter == "" || r.tag.StartsWith(gameObject.m_TagFilter)))
                        {
                            r.enabled = gameObject.m_EnableRender;
                        }
                    }
                    XCommon.tmpRender.Clear();
                }
            }
*/
        }
        private static void SyncTag(XGameObject gameObject)
        {
            if (gameObject.m_GameObject != null)
            {
                if (gameObject.IsLoaded)
                {
                    gameObject.m_GameObject.tag = gameObject.Tag;
                }
            }
        }

        public static void DestroyXGameObject(XGameObject gameObject)
        {
            gameObject.Reset();
            CommonObjectPool<XGameObject>.Release(gameObject);
        }

        private static void SyncSetParentTrans(XGameObject gameObject, object obj, int commandID)
        {
            if (gameObject.Trans != null)
            {
                gameObject.Trans.parent = obj as Transform;
            }
        }

        private static void SyncLocalPRS(XGameObject gameObject, object obj, int commandID)
        {
            XLocalPRSAsyncData prs = obj as XLocalPRSAsyncData;
            if (gameObject.Trans != null && prs != null)
            {
                if ((prs.mask & 1) > 0)
                {
                    gameObject.Trans.localPosition = prs.localPos;
                    gameObject.SyncPos();
                }
                if ((prs.mask & 2) > 0)
                    gameObject.Trans.localRotation = prs.localRotation;
                if ((prs.mask & 3) > 0)
                    gameObject.Trans.localScale = prs.localScale;
            }

        }

        #region Command
        private void AppendPositionCommand()
        {
            if (IsLoaded)
            {
                SyncPosition(this);
            }
            else
            {
                SetCbFlag(ECallbackCmd.ESyncPosition, true);
            }
        }

        private void AppendRotationCommand()
        {
            if (IsLoaded)
            {
                SyncRotation(this);
            }
            else
            {
                SetCbFlag(ECallbackCmd.ESyncRotation, true);
            }
        }

        private void AppendScaleCommand()
        {
            if (IsLoaded)
            {
                SyncScale(this);
            }
            else
            {
                SetCbFlag(ECallbackCmd.ESyncScale, true);
            }
        }

        private void AppendLayerCommand()
        {
            if (IsLoaded)
            {
                SyncLayer(this);
            }
            else
            {
                SetCbFlag(ECallbackCmd.ESyncLayer, true);
            }
        }

        private void AppendCCStepOffsetCommand()
        {
            if (IsLoaded)
            {
                SyncCCOffset(this);
            }
            else
            {
                SetCbFlag(ECallbackCmd.ESyncCCStepOffset, true);
            }
        }

        private void AppendEnableCCCommand()
        {
            if (IsLoaded)
            {
                SyncCCEnable(this);
            }
            else
            {
                SetCbFlag(ECallbackCmd.ESyncCCEnable, true);
            }
        }

        private void AppendEnableBCommand()
        {
            if (IsLoaded)
            {
                SyncBCEnable(this);
            }
            else
            {
                SetCbFlag(ECallbackCmd.ESyncBCEnable, true);
            }
        }

        private void AppendUpdateWhenOffscreen()
        {
            if (IsLoaded)
            {
                SyncUpdateWhenOffscreen(this);
            }
            else
            {
                SetCbFlag(ECallbackCmd.ESyncUpdateWhenOffscreen, true);
            }
        }
        private void AppendTagCommand()
        {
            if (IsLoaded)
            {
                SyncTag(this);
            }
            else
            {
                SetCbFlag(ECallbackCmd.ESyncTag, true);
            }
        }
        #endregion

        #region public
        public void SyncPos()
        {
            if (Trans != null)
            {
                m_Position = Trans.position;
            }
        }

        private static void _SetPhysicTransform(XGameObject gameObject, object obj, int commandID)
        {
            XGameObject original = obj as XGameObject;
            if (original.m_EnableCC && original.m_ccCache == null)
            {
                original.m_ccCache = gameObject.m_ccCache;
                if (original.m_ccCache != null)
                    original.m_ccCache.enabled = true;
                if (original.m_bcCache != null)
                    original.m_bcCache.enabled = false;
            }
            else if (original.m_EnableBC && original.m_bcCache == null)
            {
                original.m_bcCache = gameObject.m_bcCache;
                if (original.m_bcCache != null)
                    original.m_bcCache.enabled = true;
                if (original.m_ccCache != null)
                    original.m_ccCache.enabled = false;
            }
            original.m_TransformCache = gameObject.Trans;
        }

        
        #endregion
    }
}
