using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour {
    private bool m_bPlayed;

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
    }
}
