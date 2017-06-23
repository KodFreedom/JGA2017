using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageController : MonoBehaviour
{
    //--------------------------------------------------------------------------
    //  Enum
    //--------------------------------------------------------------------------
    private enum EFFECT_IDX
    {
        LEFT = 0,
        RIGHT
    }

    //--------------------------------------------------------------------------
    //  定数
    //--------------------------------------------------------------------------
    public float m_fVelocityX = 5.0f / 60.0f;
    public float m_fGravity = 0.5f / 1000.0f;
    public float m_fDamping = 0.99f;
    public GameObject m_objLeftWall;
    public GameObject m_objRightWall;
    public GameObject m_objPlane;
    public ParticleSystem[] m_particleWall;
    
    //UI
    public Image m_imgUIStage;
    public Image[] m_aImgUIStageNum;
    public Sprite[] m_aNumbers;
    private const float c_fUIStageYMax = 235.0f;
    private const float c_fUIStageYMin = -205.0f;

    //--------------------------------------------------------------------------
    //  変数
    //--------------------------------------------------------------------------
    private Vector3 m_vVelocity;
    private float m_fLTValue;
    private float m_fRTValue;
    private float m_fLTOld;
    private float m_fRTOld;
    private bool m_bPushedL;
    private bool m_bPushedR;
    private bool m_bCanControl;
    private bool m_bFirstNipped;
    private bool m_bFirstUnNipped;
    private Vector3 m_vLeftParticlePos;
    private Vector3 m_vRightParticlePos;

    public Vector3 GetVelocity()
    {
        return m_vVelocity;
    }

    public void DisableControl()
    {
        m_bCanControl = false;
    }

    // Use this for initialization
    void Start()
    {
        m_bCanControl = true;
        m_bPushedL = false;
        m_bPushedR = false;
        m_bFirstNipped = false;
        m_bFirstUnNipped = false;
        m_fLTValue = 0.0f;
        m_fRTValue = 0.0f;
        m_fLTOld = 0.0f;
        m_fRTOld = 0.0f;
        m_vVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        float fSizeX = gameObject.GetComponent<BoxCollider>().size.x;
        m_vLeftParticlePos = new Vector3(-fSizeX * 0.5f - 0.25f, 0.0f, 0.0f);
        m_vRightParticlePos = new Vector3(fSizeX * 0.5f + 0.25f, 0.0f, 0.0f);
    }

    private void Update()
    {
        if(Time.timeScale != 0)
        {
            m_fLTOld = m_fLTValue;
            m_fRTOld = m_fRTValue;

            if (m_bCanControl)
            {
                m_fLTValue = Input.GetAxis("LT");
                m_fRTValue = Input.GetAxis("RT");
            }
            else
            {
                m_fLTValue = 0.0f;
                m_fRTValue = 0.0f;
            }


            if (m_bPushedL && !m_bPushedR && m_fLTValue > 0.0f && m_fLTValue >= m_fLTOld && m_vVelocity.x <= 0.0f)
            {//Left Wall
                m_vVelocity = new Vector3(m_fVelocityX, m_vVelocity.y, 0.0f);

                //Effect & Soubd
                PlayWallContactEffect(transform.position + m_vLeftParticlePos, Quaternion.Euler(0f, -90.0f, 0f), EFFECT_IDX.LEFT);
                AkSoundEngine.PostEvent("left_wall", gameObject);
            }

            if (m_bPushedR && !m_bPushedL && m_fRTValue < 0.0f && m_fRTValue <= m_fRTOld && m_vVelocity.x >= 0.0f)
            {//Right Wall
                m_vVelocity = new Vector3(-m_fVelocityX, m_vVelocity.y, 0.0f);

                //Effect & Soubd
                PlayWallContactEffect(transform.position + m_vRightParticlePos, Quaternion.Euler(0f, 90.0f, 0f), EFFECT_IDX.RIGHT);
                AkSoundEngine.PostEvent("right_wall", gameObject);
            }

            m_vVelocity.y -= m_fGravity;
            m_vVelocity *= m_fDamping;
        }
    }

    void LateUpdate()
    {
        if (Time.timeScale != 0)
        {
            m_bPushedL = false;
            m_bPushedR = false;
            Vector3 vPos = transform.position;
            
            float fLengthWall = m_objLeftWall.transform.localScale.x;
            float fLengthStage = GetComponent<BoxCollider>().size.x;
            float fHeightStage = GetComponent<BoxCollider>().size.y;

            //左壁との当たり判定
            if (vPos.x - fLengthStage * 0.5f <= m_objLeftWall.transform.position.x + fLengthWall * 0.5f + m_fVelocityX)
            {
                m_bPushedL = true;
                if (m_vVelocity.x < 0.0f)
                {
                    m_vVelocity.x = 0.0f;
                }
            }

            //右壁との当たり判定
            if (vPos.x + fLengthStage * 0.5f >= m_objRightWall.transform.position.x - fLengthWall * 0.5f - m_fVelocityX)
            {
                m_bPushedR = true;
                if (m_vVelocity.x > 0.0f)
                {
                    m_vVelocity.x = 0.0f;
                }
            }

            //挟んだら処理する
            if ((m_bPushedL && m_bPushedR))
            {
                UpdateWhenBeingNipped();
            }
            else
            {
                UpdateWhenNotBeingNipped();
            }

            transform.position += m_vVelocity;

            //地面との当たり判定
            if (transform.position.y <= m_objPlane.transform.position.y + fHeightStage * 0.5f)
            {
                transform.position = new Vector3(transform.position.x, m_objPlane.transform.position.y + fHeightStage * 0.5f, transform.position.z);

                //Game Over
                AkSoundEngine.PostEvent("fall_stop", gameObject);
                GameOver();
                m_vVelocity *= 0.0f;
            }
        }
		float fValue = -transform.position.y * 0.1f;
		AkSoundEngine.SetRTPCValue ("room_height", fValue);
        UpdateUI();
    }

    private void UpdateWhenBeingNipped()
    {
        m_vVelocity *= 0.0f;
        GimicController[] gimic = GetComponentsInChildren<GimicController>();
        PlayerController player = GetComponentInChildren<PlayerController>();

        if(player)
        {
            player.SetStatusNormal();
        }
       

        for(int nCnt = 0;nCnt < gimic.Length;nCnt++)
        {
            gimic[nCnt].SetStatusNormal();
        }

        //Effect & Sound
        if(!m_bFirstNipped)
        {
            GameObject.Find("Main Camera").GetComponent<CameraController>().ShakeCamera();
            m_bFirstNipped = true;
            m_bFirstUnNipped = false;
            PlayWallContactEffect(transform.position + m_vLeftParticlePos, Quaternion.Euler(0f, -90.0f, 0f), EFFECT_IDX.LEFT);
            PlayWallContactEffect(transform.position + m_vRightParticlePos, Quaternion.Euler(0f, 90.0f, 0f), EFFECT_IDX.RIGHT);
            AkSoundEngine.PostEvent("LR_wall", gameObject);
            Debug.Log("fall stop");
            AkSoundEngine.PostEvent("fall_stop", gameObject);
        }
    }

    private void UpdateWhenNotBeingNipped()
    {
        GimicController[] gimic = GetComponentsInChildren<GimicController>();
        PlayerController player = GetComponentInChildren<PlayerController>();
       
        if(player)
        {
            player.SetStatusFalling();
        }

        for (int nCnt = 0; nCnt < gimic.Length; nCnt++)
        {
            gimic[nCnt].SetStatusFalling();
        }

        //Effect & Sound
        if(!m_bFirstUnNipped)
        {
            m_bFirstUnNipped = true;
            m_bFirstNipped = false;
            Debug.Log("fall start");
            AkSoundEngine.PostEvent("fall_start", gameObject);
        }
    }

    private void GameOver()
    {
        Transform[] childs = transform.GetComponentsInChildren<Transform>();
        for(int nCnt = 0;nCnt < childs.Length;nCnt++)
        {
            childs[nCnt].SetParent(null, true);

            Rigidbody rb = childs[nCnt].gameObject.GetComponent<Rigidbody>();
            if (!rb)
            {
                rb = childs[nCnt].gameObject.AddComponent<Rigidbody>();
            }

            rb.useGravity = true;
            rb.isKinematic = false;
            rb.mass = 1.0f;
            rb.AddForce(m_vVelocity);
            rb.constraints = RigidbodyConstraints.None;
        }

        Rigidbody srb = gameObject.GetComponent<Rigidbody>();
        if(srb)
        {
            srb.isKinematic = true;
        }

        GameObject.FindGameObjectWithTag("StageSelecter").GetComponent<StageSelecter>().StageFailed();
    }

    private void PlayWallContactEffect(Vector3 vPos, Quaternion qRot, EFFECT_IDX idx)
    {
        m_particleWall[(int)idx].transform.position = vPos;
        m_particleWall[(int)idx].transform.rotation = qRot;
        m_particleWall[(int)idx].Play();
    }

    private void UpdateUI()
    {
        if(m_aImgUIStageNum.Length == 0 || m_aNumbers.Length == 0) { return; }
        float fDis = 1000.0f + transform.position.y;
        float fY = c_fUIStageYMin + fDis * 0.001f * (c_fUIStageYMax - c_fUIStageYMin);

        //Num
        int nDis = (int)fDis >= 1000 ? 999 : (int)fDis <= 3 ? 0 : (int)fDis;
        for(int nCnt = 0;nCnt < 3;nCnt++)
        {
            int nNum = nDis % 10;
            nDis /= 10;
            m_aImgUIStageNum[nCnt].sprite = m_aNumbers[nNum];
        }

        //Base
        m_imgUIStage.rectTransform.anchoredPosition = new Vector2(m_imgUIStage.rectTransform.anchoredPosition.x, fY);
    }
}
