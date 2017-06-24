using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoController : MonoBehaviour {
	
	public SelectController Fade;
	int Time = 0;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Time++;
		if ( Time == 300 )
		{
			Fade.OnClickStage (1);
		}
	}
}
