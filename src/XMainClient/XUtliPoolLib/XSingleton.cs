using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XUtliPoolLib
{
    public abstract class XBaseSingleton
    {
        public abstract bool Init();
        public abstract void Unint();
    }

    public abstract class XSingleton<T> : XBaseSingleton where T : new()
    {
        protected XSingleton()
        {
            if (null != _instance)
            {
                throw new ApplicationException();
            }
        }

        private static readonly T _instance = new T();

        public static T Singleton
        {
            get
            {
                return _instance;
            }
        }

        public override bool Init(){return true;}

        public override void Unint(){ }
    }
}
