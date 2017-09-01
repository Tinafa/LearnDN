using System;
using System.Collections.Generic;
using XUtliPoolLib;
using UnityEngine;

namespace XMainClient
{
    public sealed class XEntityMgr : XSingleton<XEntityMgr>
    {
        //private XTableAsyncLoader _async_loader = null;

        private Dictionary<ulong, XEntity> _entities = new Dictionary<ulong, XEntity>();
        private List<XEntity> _entities_itor = new List<XEntity>();

        private List<XEntity> _addlist = new List<XEntity>();
        private List<XEntity> _removelist = new List<XEntity>();


        public override bool Init()
        {
            /*if(_async_loader == null)
            {
                _async_loader = new XTableAsyncLoader();
                _async_loader.AddTask();
                _async_loader.Execute();
            }*/

            return true;
        }

        public override void Uninit()
        {
            //_async_loader = null;

            //_empty.Clear();

            _entities.Clear();
            //_scene_npcs.Clear();
            //_npcs.Clear();

            _entities_itor.Clear();

            _addlist.Clear();
            _removelist.Clear();
        }

        /*
         * Creation can be called any where of context.
         * Actual object creation is always delayed until after the current PostUpdate loop, but will always be done before rendering.
         */
        public XRole CreateRole(XAttrComp attr, Vector3 position, Quaternion rotation, bool autoAdd, bool emptyPrefab = false)
        {
            XRole e = PrepareEntity<XRole>(attr, position, rotation, autoAdd, false, emptyPrefab);
            e.OnCreated();

            return e;
        }

        private T PrepareEntity<T>(XAttrComp attr, Vector3 position, Quaternion rotation, bool autoAdd, bool emptyPrefab = false, bool asyncLoad = true) where T : XEntity
        {
            attr.AppearAt = position;

            T x = Activator.CreateInstance<T>();
            XGameObject o = null;
            if (emptyPrefab)
            {
                o = XGameObject.CreateXGameObject("", position, rotation, asyncLoad);
            }
            else
            {
                o = XGameObject.CreateXGameObject("Prefabs/" + attr.Prefab, position, rotation, asyncLoad);
            }

            o.CCStepOffset = 0.1f;
            x.Initilize(o, attr);

            if (autoAdd) SafeAdd(x);

            return x;
        }

        /*
         * Destroy can be called any where of context.
         * Actual object destruction is always delayed until after the current PostUpdate loop, but will always be done before rendering.
         */
        public void DestroyEntity(XEntity x)
        {
            if (null == x || x.Deprecated) return;

            if (!_entities.ContainsKey(x.ID))
            {
                DestroyImmediate(x);

                _addlist.Remove(x);
                _removelist.Remove(x);
            }
            else
            {
                x.Destroying = true;
                x.OnDestroy();
                //if (x != null && x.IsRole)
                //    XDebug.singleton.AddLog("obj destroy:", x.Name);
                SafeRemove(x);
            }
        }

        /*
         * x must not be in the _entities map
         */
        public void DestroyImmediate(XEntity x)
        {
            if (null == x || x.Deprecated) return;

            x.Destroying = true;
            x.OnDestroy();

            x.Uninitilize();
        }

        public void OnLeaveScene()
        {
            foreach (KeyValuePair<ulong, XEntity> pair in _entities)
                DestroyImmediate(pair.Value);

            for (int i = 0; i < _addlist.Count; i++)
                DestroyImmediate(_addlist[i]);

            for (int i = 0; i < _removelist.Count; i++)
            {
                _removelist[i].Uninitilize();
            }

            _addlist.Clear();
            _removelist.Clear();
            _entities.Clear();
            _entities_itor.Clear();
        }

        public void Update(float fDeltaT)
        {
            Iterate(_entities_itor, fDeltaT);
        }

        public void PostUpdate(float fDeltaT)
        {
            PostIterate(_entities_itor, fDeltaT);

            InnerAdd();
            InnerRemove();
        }

        public void FixedUpdate()
        {
            FixedIterate(_entities_itor);
        }

        public XEntity Add(XEntity x)
        {
            ulong hash = x.ID;

            if (!DuplicationCheck(hash))
            {
#if DEBUG
                //throw new ArgumentException("Duplicated ID " + hash + " Name " + x.Name);
#else
                DestroyImmediate(_entities[hash]);
                _entities[hash] = x;
#endif
            }
            else
            {
                _entities.Add(hash, x);
            }

            _entities_itor.Add(x);
            return x;
        }


        private bool DuplicationCheck(ulong hash)
        {
            if (!_entities.ContainsKey(hash)) return true;

            if (_removelist.Contains(_entities[hash]))
            {
                XEntity x = _entities[hash];

                Remove(hash);
                x.Uninitilize();

                return true;
            }
            else
                return false;
        }

        private void SafeAdd(XEntity x)
        {
            _addlist.Add(x);
            _removelist.Remove(x);
        }

        private void Remove(ulong hash)
        {
            if (_entities.ContainsKey(hash))
            {
                XEntity x = _entities[hash];

                _entities.Remove(hash);
                _entities_itor.Remove(x);
            }
        }

        private void SafeRemove(XEntity x)
        {
            _removelist.Add(x);
            _addlist.Remove(x);
        }

        private void InnerAdd()
        {
            for (int i = 0, imax = _addlist.Count; i < imax; ++i)
            {
                XEntity x = _addlist[i];
                Add(x);
            }

            _addlist.Clear();
        }

        private void InnerRemove()
        {
            for (int i = 0, imax = _removelist.Count; i < imax; ++i)
            {
                XEntity x = _removelist[i];
                Remove(x.ID);
                x.Uninitilize();
            }

            _removelist.Clear();
        }

        private void Iterate(List<XEntity> iterator, float fDeltaT)
        {
            int len = iterator.Count;

            for (int i = 0; i < len; i++)
            {
                XEntity e = iterator[i];
                if (!e.Deprecated) e.Update(fDeltaT);
            }
        }

        private void PostIterate(List<XEntity> iterator, float fDeltaT)
        {
            int len = iterator.Count;

            for (int i = 0; i < len; i++)
            {
                XEntity e = iterator[i];
                if (!e.Deprecated) e.PostUpdate(fDeltaT);
            }
        }

        private void FixedIterate(List<XEntity> iterator)
        {
            int len = iterator.Count;

            for (int i = 0; i < len; i++)
            {
                XEntity e = iterator[i];
                if (!e.Deprecated) e.FixedUpdate();
            }
        }
    }
}
