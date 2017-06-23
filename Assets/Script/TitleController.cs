using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour {
    public GameObject m_stage;
    private Vector3 m_vVelocity;

	// Use this for initialization
	void Start () {
        m_vVelocity = Vector3.zero;
		Debug.Log ("Title BGN");
        AkSoundEngine.PostEvent("BGM_title_start", gameObject);
    }

    private void Update()
    {
        if(m_stage)
        {
            m_vVelocity.y += 0.01f;
            m_stage.transform.position -= m_vVelocity;
            m_vVelocity *= 0.98f;

            if (m_stage.transform.position.y <= -800f)
            {
                m_stage.transform.position = Vector3.zero;
            }
        }
    }
}
