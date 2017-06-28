using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraController : MonoBehaviour
{
    public GameObject m_stage;
    private float m_fSpeed;
    private float m_fAcc;
    private Vector3 m_vLookAt;

	// Use this for initialization
	void Start ()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        m_vLookAt = new Vector3(0f, -998f, 0f);
        m_fSpeed = 0f;
        m_fAcc = 0f;
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        if (m_stage && m_stage.transform.position.y <= -900.0f)
        {
            m_fAcc += 0.0001f;
            m_fSpeed = Mathf.Lerp(m_fSpeed, 0.3f, m_fAcc);
            m_vLookAt = Vector3.Lerp(m_vLookAt, m_stage.transform.position, m_fSpeed);
        }

        transform.LookAt(m_vLookAt);
	}
}
