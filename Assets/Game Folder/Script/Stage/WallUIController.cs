using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallUIController : MonoBehaviour {
    private Transform UI;
    private GameObject stage;

	// Use this for initialization
	void Start ()
    {
        stage = GameObject.Find("stage");
        UI = transform.GetChild(0);
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
		if(stage && UI)
        {
            UI.position = new Vector3(UI.position.x, stage.transform.position.y, UI.position.z);
        }
	}
}
