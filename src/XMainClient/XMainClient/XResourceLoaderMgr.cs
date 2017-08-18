using System;
using System.Collections.Generic;
using UnityEngine;
using XUtliPoolLib;

namespace XMainClient
{
    public sealed class XResourceLoaderMgr : XSingleton<XResourceLoaderMgr>
    {
        public class UniteObjectInfo
        {
            public uint hashID = 0;
            public UnityEngine.Object asset = null;
            public int refCount = 0;
            public Queue<UnityEngine.Object> objPool = null;
        }

        public bool useNewMgr = false;
        private Dictionary<uint, UnityEngine.Object> _asset_pool = new Dictionary<uint, UnityEngine.Object>();

        private uint _prefixHash = 0;

        public XResourceLoaderMgr()
        {
            _prefixHash = XCommon.singleton.XHashLowerRelpaceDot(_prefixHash, "Assets.Resources.");
        }

        public GameObject CreateFromPrefab(string location, Vector3 position, Quaternion quaternion, bool usePool = true, bool dontDestroy = false)
        {
            GameObject o = CreateFromPrefab(location, usePool, dontDestroy) as GameObject;
            o.transform.position = position;
            o.transform.rotation = quaternion;

            return o;
        }

        public UnityEngine.Object CreateFromPrefab(string location, bool usePool = true, bool dontDestroy = false)
        {
            return CreateFromAsset<GameObject>(location, ".prefab", usePool, dontDestroy);
        }

        public T CreateFromAsset<T>(string location, string suffix, bool usePool = true, bool dontDestroy = false) where T : UnityEngine.Object
        {
            uint hash = Hash(location, suffix);

            UnityEngine.Object obj = null;
            UnityEngine.Object asset = Resources.Load(location, typeof(T));
            obj = UnityEngine.Object.Instantiate(asset);
            return obj as T;
        }

        private UnityEngine.Object CreateFromAssets<T>(string location, uint hash)
        {
            UnityEngine.Object o;
            o = Resources.Load(location, typeof(T));

            return o;
        }

        public UnityEngine.Object AddAssetInPool(UnityEngine.Object asset, uint hash)
        {
            if (asset == null) return null;

            UnityEngine.Object obj = null;
            if (!_asset_pool.TryGetValue(hash, out obj))
            {
                obj = asset;
                _asset_pool.Add(hash, obj);               
            }
            return obj;
        }

        private uint Hash(string location, string ext)
        {
            uint hash = 0;
            hash = XCommon.singleton.XHashLowerRelpaceDot(_prefixHash, location);
            return XCommon.singleton.XHashLowerRelpaceDot(hash, ext);

        }
    }
}
