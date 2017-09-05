using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDragonBones;
using DragonBones;
using XUtliPoolLib;

public class XUnityArmatureComp : MonoBehaviour,IXUniArmatureComp {

    public UnityArmatureComponent _armatureComp = null;
    public Armature _armature = null;

    private bool isInited = false;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if(!isInited)
        {
            _armatureComp = this.transform.GetComponentInChildren<UnityArmatureComponent>();
            if (_armatureComp != null) _armature = _armatureComp.armature;
            isInited = true;
        }
    }

    public IXArmature armature { get{
            if(!isInited)
            {
                Init();
            }
            return _armature as IXArmature;} }
}
