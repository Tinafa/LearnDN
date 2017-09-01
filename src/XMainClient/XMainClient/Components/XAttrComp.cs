using System;
using XUtliPoolLib;
using UnityEngine;

namespace XMainClient
{
    public class XAttrComp : XComponent
    {
        public static new readonly uint uuID = XCommon.singleton.XHash("XAttrComp");
        public override uint ID { get { return uuID; } }

        //uniqe id
        private ulong _id;

        private Vector3 _appear_pos = Vector3.zero;
        private string _prefab_name = null;
        private string _name = string.Empty;


        public ulong EntityID
        {
            get { return _id; }
            set { _id = value; }
        }               

        public Vector3 AppearAt { get { return _appear_pos; } set { _appear_pos = value; } }

        public string Prefab { get { return _prefab_name; } set { _prefab_name = value; } }
        public string Name { get { return _name; } set { _name = value; } }

        XPlayerData _datas = null;
        

        public override void OnAttachToHost(XObject host)
        {
            base.OnAttachToHost(host);
        }

        public override void Attached()
        {
            base.Attached();
        }

    }
}
