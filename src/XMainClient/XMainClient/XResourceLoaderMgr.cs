using System;
using System.Collections.Generic;
using UnityEngine;
using XUtliPoolLib;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

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
        public static readonly Vector3 Far_Far_Away = new Vector3(0, -1000, 0);
        //public bool useNewMgr = false;
        private Dictionary<uint, UnityEngine.Object> _asset_pool = new Dictionary<uint, UnityEngine.Object>();
        private Dictionary<uint, int> _asset_ref_count = new Dictionary<uint, int>();

        private Dictionary<uint, Queue<UnityEngine.Object>> _object_pool = new Dictionary<uint, Queue<UnityEngine.Object>>();
        private Dictionary<int, uint> _reverse_map = new Dictionary<int, uint>();

        private Dictionary<uint, UnityEngine.Object> _xml_pool = new Dictionary<uint, UnityEngine.Object>();
        //private XmlSerializer[] xmlSerializerCache = new XmlSerializer[1];
        private MemoryStream shareMemoryStream = new MemoryStream(8192);//512k

        private uint _prefixHash = 0;

        public XResourceLoaderMgr()
        {
            _prefixHash = XCommon.singleton.XHashLowerRelpaceDot(_prefixHash, "Assets.Resources.");

            //xmlSerializerCache[0] = new XmlSerializer(typeof(XSaveDoc));
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

            UnityEngine.Object o = null;
            UnityEngine.Object instance = null;
            if (usePool && GetInObjectPool(ref o, hash))
            {
                instance = o;
            }else
            {
                o = GetAssetInPool(hash);
                if(o == null)
                {
                    o = CreateFromAssets<T>(location, hash);
                }
                instance = o != null ? UnityEngine.Object.Instantiate(o) : null;
                if (instance != null)
                {
                    AssetsRefRetain(hash);
                    LogReverseID(instance, hash);
                }
            }            
            return instance as T;
        }
        private UnityEngine.Object CreateFromAssets<T>(string location, uint hash, bool showError = true)
        {
            UnityEngine.Object o;
            o = Resources.Load(location, typeof(T));
            o = AddAssetInPool(o, hash);

            if (o == null)
            {
                if (showError) LoadErrorLog(location);
                return null;
            }

            return o;
        }

        private UnityEngine.Object CreateFromAssets<T>(string location, uint hash)
        {
            UnityEngine.Object o;
            o = Resources.Load(location, typeof(T));

            o = AddAssetInPool(o, hash);

            return o;
        }

        private UnityEngine.Object GetAssetInPool(uint hash)
        {
            UnityEngine.Object o = null;
            _asset_pool.TryGetValue(hash, out o);
            return o;
        }

        private bool GetInObjectPool(ref UnityEngine.Object o, uint id)
        {
            Queue<UnityEngine.Object> list = null;

            if (_object_pool.TryGetValue(id, out list))
            {
                int count = list.Count;
                if (count > 0)
                {
                    UnityEngine.Object temp = list.Dequeue();
                    while (temp == null && list.Count > 0)
                    {
                        temp = list.Dequeue();
                    }

                    if (list.Count == 0) _object_pool.Remove(id);
                    if (temp == null) return false;
                    o = temp;

                    return true;
                }
                else
                    _object_pool.Remove(id);
            }
            return false;
        }

        private UnityEngine.Object AddAssetInPool(UnityEngine.Object asset, uint hash)
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

        private void LogReverseID(UnityEngine.Object o, uint id)
        {
            if (o != null)
            {
                int uid = o.GetInstanceID();

                if (!_reverse_map.ContainsKey(uid))
                    _reverse_map.Add(uid, id);
            }
        }

        public T GetSharedResource<T>(string location, string suffix, bool canNotNull = true, bool preload = false) where T : UnityEngine.Object
        {
            uint hash = Hash(location, suffix);
  
            {
                float time = Time.time;

                UnityEngine.Object asset = GetAssetInPool(hash);

                if (asset == null)
                {
                    asset = CreateFromAssets<T>(location, hash, canNotNull);
                }

                if (asset != null)
                {
                    AssetsRefRetain(hash);
                }

                return asset as T;
            }
        }

        public void UnSafeDestroy(UnityEngine.Object o, bool returnPool = true)
        {
            if (o == null) return;
            int instanceID = o.GetInstanceID();

            uint hash = 0;
            if (returnPool && _reverse_map.TryGetValue(instanceID, out hash))
            {
                AddToObjectPool(hash, o);
            }
            else
            {
                if (_reverse_map.TryGetValue(instanceID, out hash))
                {
                    AssetsRefRelease(hash);
                    _reverse_map.Remove(instanceID);
                }
                UnityEngine.Object.Destroy(o);

            }
        }

        public void UnSafeDestroyShareResource(string location, string suffix, UnityEngine.Object o, bool triggerRelease = false)
        {
            if (o == null)
                return;
            //if (useNewMgr)
            //{
            //    int instanceID = o.GetInstanceID();
            //    InnerDestroy(o, instanceID, true);
            //}
            //else
            {
                if (!string.IsNullOrEmpty(location))
                {
                    uint hash = Hash(location, suffix);
                    if (_asset_ref_count.ContainsKey(hash))
                    {
                        if (AssetsRefRelease(hash) && triggerRelease /*&& !UniteObjectInfo.CanDestroy(o)*/)
                        {
                            Resources.UnloadAsset(o);
                        }
                    }
                    else/* if (!UniteObjectInfo.CanDestroy(o))**/
                    {
                        Resources.UnloadAsset(o);
                    }
                }

            }
        }

        private void AddToObjectPool(uint id, UnityEngine.Object obj)
        {
            Queue<UnityEngine.Object> list = null;

            if(!_object_pool.TryGetValue(id, out list))
            {
                list = new Queue<UnityEngine.Object>();
                _object_pool.Add(id, list);
            }

            GameObject go = obj as GameObject;
            if(go != null)
            {
                Transform t = go.transform;
                t.position = Far_Far_Away;
                t.rotation = Quaternion.identity;
                t.parent = null;
            }
            list.Enqueue(obj);
        }
        private void AssetsRefRetain(uint hash)
        {
            int refCount = 0;
            _asset_ref_count.TryGetValue(hash, out refCount);
            refCount += 1;
            _asset_ref_count[hash] = refCount;
        }

        private bool AssetsRefRelease(uint hash)
        {
            int refCount = 0;
            if (_asset_ref_count.TryGetValue(hash, out refCount))
            {
                refCount -= 1;

                if (refCount < 0)
                {
                    refCount = 0;
                }

                if (refCount == 0)
                {
                    _asset_pool.Remove(hash);

                    _asset_ref_count.Remove(hash);
                    return true;
                }
                else
                {
                    _asset_ref_count[hash] = refCount;
                }
            }
            return false;
        }

        private uint Hash(string location, string ext)
        {
            uint hash = 0;
            hash = XCommon.singleton.XHashLowerRelpaceDot(_prefixHash, location);
            return XCommon.singleton.XHashLowerRelpaceDot(hash, ext);

        }

        public Stream ReadText(string location, bool error = true)
        {
            TextAsset data = Resources.Load<TextAsset>(location);

            if (data == null)
            {
                if (error)
                    LoadErrorLog(location);
                else
                    return null;
            }
            try
            {
                shareMemoryStream.SetLength(0);
                shareMemoryStream.Write(data.bytes, 0, data.bytes.Length);
                shareMemoryStream.Seek(0, SeekOrigin.Begin);
                return shareMemoryStream;
            }
            catch (Exception e)
            {
                XDebug.singleton.AddErrorLog(e.Message, location);
                return shareMemoryStream;
            }
            finally
            {
                Resources.UnloadAsset(data);
            }
        }

        public T GetData<T>(string pathwithname)
        {
            uint hash = XCommon.singleton.XHash(pathwithname);
            UnityEngine.Object data = null;

            if (!_xml_pool.TryGetValue(hash, out data))
            {
                TextAsset raw_data = Resources.Load<TextAsset>(pathwithname);

                if (raw_data == null)
                {
                    XDebug.singleton.AddErrorLog("Deserialize file ", pathwithname, " Error!");
                }

                XmlSerializer formatter = new XmlSerializer(typeof(T));

                shareMemoryStream.Seek(0, SeekOrigin.Begin);
                shareMemoryStream.SetLength(0);
                shareMemoryStream.Write(raw_data.bytes, 0, raw_data.bytes.Length);
                shareMemoryStream.Seek(0, SeekOrigin.Begin);
                object obj = formatter.Deserialize(shareMemoryStream);

                XDataWrapper wrapper = XDataWrapper.CreateInstance<XDataWrapper>();
                wrapper.Data = obj;
                Resources.UnloadAsset(raw_data);
                data = wrapper;
                if (null != data) _xml_pool.Add(hash, data);
            }

            return (T)(data as XDataWrapper).Data;
        }

        public static void LoadErrorLog(string prefab)
        {
            XDebug.singleton.AddErrorLog("Load resource: ", prefab, " error!");
        }
    }
}
