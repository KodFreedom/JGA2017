using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour {
    private bool m_bPlayed;
    private Transform m_image;
    private float m_fRotZ = 0.1f;
    private float m_fSpeed;

    public void StageClearEffect()
    {
        if(!m_bPlayed)
        {
            m_bPlayed = true;

            //Effect
            GameObject obj = transform.FindChild("Particle System").gameObject;
            if (obj)
            {
                obj.SetActive(true);
                ParticleSystem ps = obj.GetComponent<ParticleSystem>();
                ps.Play();
            }
        }
        
    }

	void Start ()
    {
        m_bPlayed = false;
        m_image = transform.FindChild("Image");
    }

    private void Update()
    {
        Quaternion rot = m_image.localRotation;
        rot.z = Mathf.SmoothDamp(rot.z, m_fRotZ, ref m_fSpeed, 1.0f);
        //rot.z = Mathf.Lerp(rot.z, m_fRotZ, 0.01f);
        if(Mathf.Abs(rot.z) >= Mathf.Abs(m_fRotZ) - 0.01f) { m_fRotZ *= -1f; }
        m_image.localRotation = rot;
    }
}
