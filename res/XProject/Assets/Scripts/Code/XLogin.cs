using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XMainClient;

public class XLogin : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Login()
    {
        XGame.singleton.SwitchTo(EXStage.Hall, 3);
    }
}
