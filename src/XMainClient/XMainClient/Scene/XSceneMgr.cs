using System;
using System.Collections.Generic;
using UnityEngine;
using XUtliPoolLib;

namespace XMainClient
{
    public sealed class XSceneMgr : XSingleton<XSceneMgr>
    {
        private XTableAsyncLoader _async_loader = null;

        private SceneTable _reader = new SceneTable();

        public override bool Init()
        {
            if (_async_loader == null)
            {
                _async_loader = new XTableAsyncLoader();

                _async_loader.AddTask(@"/Resources/Table/Scene.txt", _reader);

                _async_loader.Execute();
            }
            return true;
        }

        public string GetUnitySceneFile(uint sceneID)
        {
            SceneTable.RowData data = _reader.GetBySceneID((int)sceneID);
            if (data != null)
                return data.scenefile;

            return "";
        }
    }
}
