using System;
using System.Collections.Generic;
using UnityEngine;
using XUtliPoolLib;

namespace XMainClient
{
    public class XGameManager : MonoBehaviour
    {
        public float levelStartDelay = 2f;
        public GameObject welcome;
        public GameObject playerObj = null;
        public XRole player = null;
        public XPlayerController controller = null;

        public XTalk talk = null;

        [HideInInspector]
        public bool playerTurn = true;

        public static XGameManager instance = null;

        private bool doingSetup = true;

        private bool isGameStarted = false;
        public bool GameStarted { get { return isGameStarted; } }

        private void Awake()
        {
            if (null == instance)
                instance = this;
            if (instance != this)
                Destroy(gameObject);

            isGameStarted = false;
            InitGame();
        }

        void InitGame()
        {
            doingSetup = true;

            welcome = GameObject.Find("Welcome");

            Invoke("HideLevelImage", levelStartDelay);
        }

        void HideLevelImage()
        {
            welcome.SetActive(false);

            StartGame();
        }

        private void Update()
        {
            if (doingSetup)
                return;

            XTimeSys.singleton.Update();
        }

        void StartGame(int level=0)
        {
            //加载存档
            XSaveDoc save=null;
            if (XStorageSys.singleton.IsLoadSave)
            {
                save = XStorageSys.singleton.CurSaveDoc;
            }

            bool IsNewSave = false;
            if (save == null || save.TimeData.Time == 0) IsNewSave = true;
    
            //> Player
            //playerObj = XResourceLoaderMgr.singleton.CreateFromPrefab("Prefabs/Bear", Vector3.zero, Quaternion.identity) as GameObject;
            //playerObj.name = IsNewSave ? "Bear" : save.PlayerData.Name;

            XAttrComp attr = new XAttrComp();
            attr.Prefab = "Bear";
            attr.EntityID = 0;

            player = XEntityMgr.singleton.CreateRole(attr, Vector3.zero, Quaternion.identity, false);
            player.AttachComponent(attr);
            if(player != null)
            {
                playerObj = player.EngineObject.gameobject;
                playerObj.name = "River";
            }

            controller = playerObj.AddComponent<XPlayerController>();
            controller.SetHost(player);
            //XAttributes attr = playerObj.AddComponent<XAttributes>();

            //> Crab

            //> 新的游戏
            if (IsNewSave)
            {
                XTimeSys.singleton.BeginRecord(0);
                XWeatherSys.singleton.GetNewdayWeather();
            }
            //> 继续游戏
            else
            {
                XTimeSys.singleton.BeginRecord(save.TimeData.Time);
                XWeatherSys.singleton.SetWeatherState(save.WeatherData);
                playerObj.transform.localPosition = new Vector3(save.PlayerData.PosX, save.PlayerData.PosY, 0);
            }

            XTimeSys.singleton.RegisterDayNightHandler(OnChangeDayNight);
            XTimeSys.singleton.RegisterDayChangeHandler(OnChangeDay);
            XWeatherSys.singleton.BeginWeatherSys();

            talk = playerObj.AddComponent<XTalk>();
            talk.SetHost(playerObj.transform);
            talk.BeginSay(8f);

            doingSetup = false;
            isGameStarted = true;
        }

        void OnMinute(XDateTime time)
        {
            //XDebug.singleton.AddGreenLog("OnMinute : ", time.hh.ToString(),":", time.mm.ToString());
        }

        void OnChangeDayNight(bool bToDayTime)
        {
            XDebug.singleton.AddGreenLog("OnChangeDayNight : ", bToDayTime ? "天亮了":"黑夜来临");
        }

        void OnChangeDay(XDateTime time)
        {
            XWeatherSys.singleton.GetNewdayWeather();
            XDebug.singleton.AddGreenLog("OnChangeDay : ", "又一天 " , time.ToString());
        }
    }
}
