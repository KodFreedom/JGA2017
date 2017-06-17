using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        AkSoundEngine.PostEvent("BGM_title_start", gameObject);
    }
}
