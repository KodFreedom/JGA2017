using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialController : MonoBehaviour {
	public GameObject m_MovieObj;
	public FadeController Fade;
	private VideoPlayer m_video;
	// Use this for initialization
	void Start () {
		Debug.Log ("Tutorial BGN");
		m_video = m_MovieObj.GetComponent<VideoPlayer> ();
		AkSoundEngine.PostEvent("BGM_play_start", gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if ( !m_video.isPlaying )
		{
			Fade.LoadStage (2);
		}
	}
}
