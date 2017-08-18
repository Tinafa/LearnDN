using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UILib
{
    public delegate void OnDropReleaseEventHandler(GameObject item, GameObject surface);
    public delegate void OnDropStartEventHandler(GameObject item);

    public interface IXUIDragDropItem : IXUIObject
    {
        void RegisterOnStartEventHandler(OnDropStartEventHandler eventHandler);
        void RegisterOnFinishEventHandler(OnDropReleaseEventHandler eventHandler);

        void SetCloneOnDrag(bool cloneOnDrag);

        void SetRestriction(int restriction);

        void SetParent(Transform parent  ,bool addPanel = false, int depth = 0);

        void SetActive(bool active);
        OnDropStartEventHandler GetStartEventHandler();
        OnDropReleaseEventHandler GetReleaseEventHandler();
    }
}
