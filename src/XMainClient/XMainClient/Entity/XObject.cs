using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMainClient
{
    public class XObject
    {
        //private bool needDetachComponent = false;

        public XObject()
        {
            Deprecated = false;
            Destroying = false;
        }

        public virtual bool Initilize()
        {
            Deprecated = false;
            Destroying = false;
            return true;
        }

        public virtual void Uninitilize()
        {

        }
        public virtual bool DispatchEvent()
        {
            return true;
        }

        public virtual void OnCreated()
        {

        }

        public virtual void Update(float fDeltaT)
        {

        }

        public virtual void FixedUpdate()
        {

        }

        public virtual void PostUpdate(float fDelataT)
        {

        }

        public virtual void OnDestroy()
        {
            Deprecated = true;
            Destroying = false;
        }

        public bool Deprecated { get; set; }
        public bool Destroying { get; set; }
    }
}
