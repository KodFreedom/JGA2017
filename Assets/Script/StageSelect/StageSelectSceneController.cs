using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectSceneController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        AkSoundEngine.PostEvent("BGM_stageselect_start", gameObject);
    }
}
