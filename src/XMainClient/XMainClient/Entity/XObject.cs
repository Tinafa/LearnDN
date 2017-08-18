using System;
using System.Collections.Generic;
using XUtliPoolLib;

namespace XMainClient
{
    public class XObject
    {
        private bool needDetachComponent = false;

        private List<XComponent> internalIterator;

        public List<XComponent> Components
        {
            get
            {
                if(internalIterator == null)
                {
                    internalIterator = ListPool<XComponent>.Get();
                }
                return internalIterator;
            }
        }

        public XObject()
        {
            Deprecated = false;
            Destroying = false;
        }

        public virtual bool DispatchEvent(XEventArgs e)
        {
            bool bSucceed = false;

            for (int i = 0; i < Components.Count; ++i)
            {
                if (i >= Components.Count) break;

                XComponent c = Components[i];
                if (c != null && c.Enabled)
                {
                    bool ret = c.OnEvent(e);
                    bSucceed = ret || bSucceed;
                }
            }
            return bSucceed;
        }

        public virtual bool Initilize()
        {
            Deprecated = false;
            Destroying = false;
            return true;
        }

        public virtual void Uninitilize()
        {
            if (internalIterator != null)
            {
                for (int i = internalIterator.Count - 1; i >= 0; i--)
                {
                    XComponent componentObject = internalIterator[i];
                    componentObject.OnDetachFromHost();
                    OnComponentDetached(componentObject);
                    Components.RemoveAt(i);
                    XComponentMgr.singleton.RemoveComponent(componentObject);
                }
                ListPool<XComponent>.Release(internalIterator);
                internalIterator = null;
            }

        }

        public virtual void OnCreated()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].Attached();
            }
        }

        public virtual void Update(float fDeltaT)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                XComponent o = Components[i];
                if (o != null && o.Enabled)
                    o.Update(fDeltaT);
            }
        }

        public virtual void FixedUpdate()
        {

        }

        public virtual void PostUpdate(float fDeltaT)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                XComponent o = Components[i];
                if (o != null && o.Enabled)
                    o.PostUpdate(fDeltaT);
            }
            if (needDetachComponent)
            {
                for (int i = Components.Count - 1; i >= 0; --i)
                {
                    XComponent componentObject = Components[i];
                    if (componentObject.Detached)
                    {
                        componentObject.OnDetachFromHost();
                        OnComponentDetached(componentObject);
                        Components.RemoveAt(i);
                        XComponentMgr.singleton.RemoveComponent(componentObject);
                    }
                }
                needDetachComponent = false;
            }
        }

        public virtual void OnDestroy()
        {
            Deprecated = true;
            Destroying = false;
        }

        public void OnEnterScene()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                XComponent o = Components[i];
                if (o != null && !o.Detached)
                    o.OnEnterScene();
            }
        }

        public void OnLeaveScene()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                XComponent o = Components[i];
                if (o != null && !o.Detached)
                    o.OnLeaveScene();
            }
        }
        public XComponent GetXComponent(uint uuid)
        {
            for (int i = 0, imax = Components.Count; i < imax; i++)
            {
                XComponent o = Components[i];
                if (o != null && o.ID == uuid && !o.Detached)
                    return o;
            }
            return null;
        }

        private XComponent GetXComponent(uint uuid, ref int index)
        {
            for (int i = 0, imax = Components.Count; i < imax; i++)
            {
                XComponent o = Components[i];
                if (o != null && o.ID == uuid && !o.Detached)
                {
                    index = i;
                    return o;
                }
            }
            return null;
        }

        public void DetachCompment(uint id)
        {
            int index = -1;
            XComponent componentObject = GetXComponent(id, ref index);
            if (componentObject != null && index >= 0 && index < Components.Count)
            {
                needDetachComponent = true;
                componentObject.Detached = true;
            }
        }

        public void AttachComponent(XComponent componentObject)
        {
            if (null == componentObject) return;

            int index = Components.IndexOf(componentObject);
            if (index >= 0 && index < Components.Count)
            {
                if (componentObject.Detached)
                {
                    componentObject.Detached = false;
                }
                else
                {
                    Components[index] = componentObject;
                    XDebug.singleton.AddErrorLog("Component ", componentObject.ToString(), " for ", this.ToString(), " added too many times.");
                }
            }
            else
            {
                Components.Add(componentObject);
            }
            componentObject.OnAttachToHost(this);
            OnComponentAttached(componentObject);
        }

        protected virtual void OnComponentAttached(XComponent componentObject) { }

        protected virtual void OnComponentDetached(XComponent componentObject) { }

        public bool Deprecated { get; set; }
        public bool Destroying { get; set; }
    }
}
