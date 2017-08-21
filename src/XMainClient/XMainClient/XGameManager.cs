using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMainClient
{
    public class XGameManager : MonoBehaviour
    {
        public float levelStartDelay = 2f;
        public GameObject welcome;
        public GameObject playerObj = null;
        public XPlayer player = null;
        public XPlayerController controller = null;

        public XTalk talk = null;

        [HideInInspector]
        public bool playerTurn = true;

        public static XGameManager instance = null;

        private bool doingSetup = true;

        private void Awake()
        {
            if (null == instance)
                instance = this;
            if (instance != this)
                Destroy(gameObject);

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
            doingSetup = false;

            LoadLevel();
        }

        private void Update()
        {
            if (doingSetup || playerTurn)
                return;
        }

        void LoadLevel(int level=0)
        {
            GameObject prefab = Resources.Load("Prefabs/Player") as GameObject;
            playerObj = Instantiate(prefab,Vector3.zero, Quaternion.identity);
            player = playerObj.AddComponent<XPlayer>();
            controller = playerObj.AddComponent<XPlayerController>();
            controller.player = player;

            talk = playerObj.AddComponent<XTalk>();
            talk.SetHost(playerObj.transform);
            talk.BeginSay(2f);
        }
    }
}
