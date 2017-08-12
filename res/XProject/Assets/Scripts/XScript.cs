using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XMainClient;

public class XScript : MonoBehaviour {

    private GameManager gameManager;
    private SoundManager soundManager;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

    }

    // Use this for initialization
    void Start () {
        gameManager = gameObject.AddComponent<XMainClient.GameManager>();
        soundManager = gameObject.AddComponent<SoundManager>();
    }
	
	// Update is called once per frame
	void Update () {
#if UNITY_STANDALONE || UNITY_WEBPLAYER
#endif
    }
}
