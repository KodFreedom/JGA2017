using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    //--------------------------------------------------------------------------
    //  定数
    //--------------------------------------------------------------------------
    public GameObject m_target;
    public float m_DampTime = 0.1f;
    public Vector3 m_vDefaultPos;

    //--------------------------------------------------------------------------
    //  列挙型
    //--------------------------------------------------------------------------

    //--------------------------------------------------------------------------
    //  変数
    //--------------------------------------------------------------------------
    private Vector3 m_vVelocity;
    private float m_fPlayerPosYLocal;
    private int m_nCntOpening;
    private Vector3 m_vTime;

    //--------------------------------------------------------------------------
    //  関数
    //--------------------------------------------------------------------------
    public void ShakeCamera()
    {
        //m_nCntShake = 20;
    }

    public void StageClearCamera()
    {
        m_vDefaultPos = new Vector3(0f, 0f, -3f);
    }

    public void SetTarget(GameObject target)
    {
        m_target = target;
        transform.position = m_target.transform.position + m_vDefaultPos;
    }

    public void SetTargetSmooth(GameObject target)
    {
        m_target = target;
    }

	// Use this for initialization
	void Start ()
    {
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            transform.position = m_vDefaultPos;
        }
        else
        {
            GameObject player = GameObject.Find("Player Fbx");
            if (player)
            {
                transform.position = player.transform.position + new Vector3(0.0f, 0.0f, -1.0f);
                m_fPlayerPosYLocal = player.transform.localPosition.y;
                m_nCntOpening = 240;
                m_vTime.z = 2.0f;
                m_vTime.x = 1.0f;
                m_vTime.y = 1f;
            }
        }
    }
	
	void OnPreRender()
    {
        if(m_target)
        {
            Vector3 vPos;
            Vector3 vCurrent = transform.position;
            Vector3 vTarget = m_target.transform.position + m_vDefaultPos;
            //Vector3 vTime = Vector3.zero;

            if (m_nCntOpening > 0)
            {
                //vTime.z = 2.0f;
                //vTime.x = 1.0f;
                //vTime.y = 1f;
                m_fPlayerPosYLocal = Mathf.SmoothDamp(m_fPlayerPosYLocal, 0f, ref m_vVelocity.y, 1f);
                m_nCntOpening--;
            }
            else
            {
                //vTime.z = 0.5f;
                //vTime.x = 0.1f;
                m_fPlayerPosYLocal = 0f;
            }

            m_vTime.z = Mathf.Lerp(m_vTime.z, 0.5f, 0.015f);
            m_vTime.x = Mathf.Lerp(m_vTime.x, 0.1f, 0.01f);

            //Z
            vPos.z = Mathf.SmoothDamp(vCurrent.z, vTarget.z, ref m_vVelocity.z, m_vTime.z);

            //X
            vPos.x = Mathf.SmoothDamp(vCurrent.x, vTarget.x, ref m_vVelocity.x, m_vTime.x);

            //Y
            vPos.y = m_fPlayerPosYLocal + vTarget.y;

            transform.position = vPos;
        }
	}
}
