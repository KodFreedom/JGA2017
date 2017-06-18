using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    //--------------------------------------------------------------------------
    //  定数
    //--------------------------------------------------------------------------
    public GameObject m_target;
    public float m_DampTime = 0.1f;
    public Vector3 m_vDefaultPos;

    //--------------------------------------------------------------------------
    //  列挙型
    //--------------------------------------------------------------------------
    enum MOVE_MODE
    {
        IMMEDIATE,
        SMOOTH
    }

    //--------------------------------------------------------------------------
    //  変数
    //--------------------------------------------------------------------------
    private MOVE_MODE m_moveMode;

    public void SetTarget(GameObject target)
    {
        m_target = target;
        m_moveMode = MOVE_MODE.IMMEDIATE;
    }

    public void SetTargetSmooth(GameObject target)
    {
        m_target = target;
        m_moveMode = MOVE_MODE.SMOOTH;
    }

	// Use this for initialization
	void Start ()
    {
        transform.position = m_vDefaultPos;
        m_moveMode = MOVE_MODE.IMMEDIATE;
    }
	
	void OnPreRender()
    {
        if(m_target)
        {
            if (m_moveMode == MOVE_MODE.IMMEDIATE)
            {
                //Vector3 vTarget = new Vector3(m_target.transform.position.x * 0.5f + m_vDefaultPos.x, m_target.transform.position.y + m_vDefaultPos.y, m_target.transform.position.z + m_vDefaultPos.z);
                //transform.position = Vector3.Lerp(transform.position, vTarget, m_DampTime);
                transform.position = new Vector3(m_target.transform.position.x * 0.5f + m_vDefaultPos.x, m_target.transform.position.y + m_vDefaultPos.y, m_target.transform.position.z + m_vDefaultPos.z);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, m_target.transform.position + m_vDefaultPos, m_DampTime);
            }
        }
	}
}
