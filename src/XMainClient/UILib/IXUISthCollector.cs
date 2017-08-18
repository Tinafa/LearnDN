using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UILib
{
    public delegate void SthArrivedEventHandler(int rank);
    public delegate void CollectFinishEventHandler();
    public interface IXUISthCollector : IXUIObject
    {
        void SetPosition(Vector3 srcGlobalPos, Vector3 desGlobalPos);
        void SetSth(string name);
        void SetSth(List<GameObject> goes);
        void Emit();
        void RegisterSthArrivedEventHandler(SthArrivedEventHandler eventHandler);
        void RegisterCollectFinishEventHandler(CollectFinishEventHandler eventHandler);
    }
}
