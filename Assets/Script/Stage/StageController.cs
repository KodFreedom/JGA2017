﻿using System.Collections;
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
    public Vector3 m_vLeftParticlePos = new Vector3(-3.5f, 0.0f);
    public Vector3 m_vRightParticlePos = new Vector3(3.5f, 0.0f);
    public ParticleSystem[] m_particleWall;

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
    private bool m_bPlayedNippedEffect;

    public Vector3 GetVelocity()
    {
        return m_vVelocity;
    }

    public void DisableControl()
    {
        m_bCanControl = false;
    }

    private void Awake()
    {
        
    }

    // Use this for initialization
    void Start()
    {
        m_bCanControl = true;
        m_bPushedL = false;
        m_bPushedR = false;
        m_bPlayedNippedEffect = false;
        m_fLTValue = 0.0f;
        m_fRTValue = 0.0f;
        m_fLTOld = 0.0f;
        m_fRTOld = 0.0f;
        m_vVelocity = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void FixedUpdate()
    {
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

                //Effect
                PlayWallContactEffect(transform.position + m_vLeftParticlePos, Quaternion.Euler(0f, -90.0f, 0f), EFFECT_IDX.LEFT);
            }

            if (m_bPushedR && !m_bPushedL && m_fRTValue < 0.0f && m_fRTValue <= m_fRTOld && m_vVelocity.x >= 0.0f)
            {//Right Wall
                m_vVelocity = new Vector3(-m_fVelocityX, m_vVelocity.y, 0.0f);

                //Effect
                PlayWallContactEffect(transform.position + m_vRightParticlePos, Quaternion.Euler(0f, 90.0f, 0f), EFFECT_IDX.RIGHT);
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
            Debug.Log(m_objLeftWall.transform.position.ToString());
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
                GameOver();
                m_vVelocity *= 0.0f;

            }
        } 
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

        //Effect
        if(!m_bPlayedNippedEffect)
        {
            m_bPlayedNippedEffect = true;
            PlayWallContactEffect(transform.position + m_vLeftParticlePos, Quaternion.Euler(0f, -90.0f, 0f), EFFECT_IDX.LEFT);
            PlayWallContactEffect(transform.position + m_vRightParticlePos, Quaternion.Euler(0f, 90.0f, 0f), EFFECT_IDX.RIGHT);
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

        m_bPlayedNippedEffect = false;
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
            rb.velocity += m_vVelocity;
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
}
