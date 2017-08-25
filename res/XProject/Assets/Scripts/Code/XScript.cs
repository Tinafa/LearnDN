using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XMainClient;

public class XScript : MonoBehaviour {

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {        

        XShell.singleton.Awake();
        XShell.singleton.Start();
    }
	
	// Update is called once per frame
	void Update () {
#if UNITY_STANDALONE || UNITY_WEBPLAYER
#endif
        XShell.singleton.Update();
    }

    void LateUpdate()
    {
        XShell.singleton.PostUpdate();
    }

    void OnApplicationQuit()
    {
        XShell.singleton.Quit();
    }
}
