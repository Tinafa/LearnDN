using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using XUtliPoolLib;

namespace XMainClient
{
    public sealed class XSceneLoader :MonoBehaviour
    {
        private enum LoadingPhase
        {
            Scene_Downlaod,
            Scene_Load,
            Scene_Build,
            Scene_Doc
        }

        private class AsyncSceneBuildRequest
        {
            public float Progress = 1;
            public IEnumerator Pedometer = null;
        }

        private AsyncOperation _op = null;
        private AsyncSceneBuildRequest _asbr = null;

        private uint _loading_scene_id = 0;
        private EXStage _loading_scene_stage = EXStage.Null;

        private bool _enabled = false;
        private bool _progress = true;

        private float _sub_progress = 0;

        private float _current_progress = 0;
        private float _target_progress = 0;

        public void LoadScene(string scene, EXStage eStage, bool process, uint nextsceneid, uint currrenrscene)
        {
            XGame.singleton.notLoadScene = false;
            XTimerMgr.singleton.update = false;
            _loading_scene_id = nextsceneid;
            _loading_scene_stage = eStage;

            _progress = process;

            ObjectPoolCache.Clear();

            XResourceLoaderMgr.singleton.ReleasePool();

            Application.backgroundLoadingPriority = UnityEngine.ThreadPriority.High;

            StartCoroutine(LoadLevelWithProgress(scene));
            XScene.singleton.OnSceneBeginLoad(_loading_scene_id);
        }

        IEnumerator LoadLevelWithProgress(string scene)
        {
            this.enabled = true;
            _enabled = true;

            _current_progress = 0;
            _target_progress = 0;

            yield return null;

            _op = SceneManager.LoadSceneAsync(scene);
            while (!_op.isDone)
            {
                yield return null;
            }

            _asbr = SceneBuildAsync(_loading_scene_id);
            yield return null;
            while (_asbr.Pedometer.MoveNext())
            {
                DisplayPrograss(LoadingPhase.Scene_Build, _asbr.Progress);
                yield return null;
            }

            if(_progress)
            {
                XGame.singleton.OnEnterScene(_loading_scene_id);

                DisplayPrograss(LoadingPhase.Scene_Doc, 0.3f);
                yield return null;
                IEnumerator iter = DocPreload(_loading_scene_id);
                while (iter.MoveNext())
                {
                    DisplayPrograss(LoadingPhase.Scene_Doc, 0.3f + 0.4f * _sub_progress);
                    yield return null;
                }

                DisplayPrograss(LoadingPhase.Scene_Doc, 0.9f);


                _enabled = false;
            }

            _progress = false;

            _op = null;

            XGame.singleton.notLoadScene = true;
            XGame.singleton.switchScene = false;
            XTimerMgr.singleton.update = true;
            Application.backgroundLoadingPriority = UnityEngine.ThreadPriority.Normal;

            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        private void DisplayPrograss(LoadingPhase phase, float progress)
        {
            if (!_progress) return;
        }

        private IEnumerator DocPreload(uint sceneid)
        {
            _sub_progress = 0;

            for (int i = 0; i < XGame.singleton.Doc.Components.Count; i++)
            {
                XGame.singleton.Doc.Components[i].OnEnterScene();

                _sub_progress = (float)i / (float)XGame.singleton.Doc.Components.Count;
            }

            yield return null;
            _sub_progress = 1;
        }

        private AsyncSceneBuildRequest SceneBuildAsync(uint sceneID)
        {
            AsyncSceneBuildRequest asbr = new AsyncSceneBuildRequest();
            asbr.Pedometer = Preloader(sceneID);

            return asbr;
        }

        IEnumerator Preloader(uint sceneID)
        {
            //IEnumerator ietr = null;
            _asbr.Progress = 0;
            yield return null;

            XScene.singleton.OnSceneLoaded(sceneID);
            _asbr.Progress = 0.05f;
            yield return null;

           /* ietr = PreLoadSceneRes(sceneID);
            while (ietr.MoveNext())
            {
                _asbr.Progress = 0.8f + _sub_progress * 0.1f;
                yield return null;
            }*/

            _asbr.Progress = 1;
        }

        void Update()
        {
            if (_enabled && _progress)
            {
                _current_progress += Mathf.Abs(_target_progress - _current_progress) * 0.5f;
                //> 显示加载进度
                //LoadingDlg.singleton.SetLoadingProgress(_current_progress);
            }
        }
    }
}
