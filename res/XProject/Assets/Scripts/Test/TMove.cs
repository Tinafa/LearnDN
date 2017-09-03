using UnityEngine;
using System.Collections;

public class TMove : MonoBehaviour {
    float x = 0f;
	// Use this for initialization
	void Start () {
        x = this.transform.localPosition.x;
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 position = this.transform.localPosition;
        position.x += Time.deltaTime * 1f;
        if (position.x > 14)
        {
            position.x = x;
        }
        this.transform.localPosition = position;
	}
}
