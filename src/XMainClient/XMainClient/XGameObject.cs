using System;
using System.Collections.Generic;
using UnityEngine;
using XUtliPoolLib;

namespace XMainClient
{
    public class XEngineObject
    {
        protected GameObject m_GameObject = null;
        protected GameObject m_Parent = null;
        public string m_Location = "";
    }

    class XGameObject : XEngineObject
    {
        private Vector3 m_Position = Vector3.one * 1000;
        private Quaternion m_Rotation = Quaternion.identity;
        private Vector3 m_Scale = Vector3.one;

        private Transform m_TransformCache = null;
        //private SkinnedMeshRenderer m_SkinMeshRenderCache = null;
        //private CharacterController m_ccCache = null;

        public int objID = -1;
        public static int globalObjID = 0;

        private short m_LoadStatus = 0;

        public XGameObject()
        {

        }

        public bool IsLoaded
        {
            get
            {
                return m_LoadStatus == 1;

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

        public static int GetGlobalObjID()
        {
            globalObjID++;
            if (globalObjID > 1000000)
            {
                globalObjID = 0;
            }
            return globalObjID;
        }

        public static XGameObject CreateXGameObjec(string location, Vector3 position, Quaternion rotation, bool async = true, bool usePool = true)
        {
            XGameObject gameObject = CreateXGameObjec(location, async, usePool);
            gameObject.Position = position;
            gameObject.Rotation = rotation;
            return gameObject;
        }

        public static XGameObject CreateXGameObjec(string location, bool async = true, bool usePool = true)
        {
            XGameObject gameObject = CommonObjectPool<XGameObject>.Get();
            gameObject.objID = GetGlobalObjID();
            gameObject.m_Location = location;

            if (string.IsNullOrEmpty(location))
            {
                GameObject go = new GameObject("EmptyObject");
                gameObject.LoadFinish(go, null);
            }
            else
            {

            }

            return gameObject;
        }

        private void LoadFinish(UnityEngine.Object obj, System.Object cbOjb)
        {

        }

        public void LoadAsync(string location, bool usePool)
        {

        }

        public void Load(string location, bool usePool)
        {

        }

        public static void DestroyXGameObject(XGameObject gameObject)
        {
            CommonObjectPool<XGameObject>.Release(gameObject);
        }
    }
}
