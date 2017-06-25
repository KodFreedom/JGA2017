using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour {
    //--------------------------------------------------------------------------
    //  定数
    //--------------------------------------------------------------------------
    public float m_fPosX = 8.0f;
    public float m_fPosY = -500.0f;
    public float m_fSpeed = 3.0f / 60.0f;
    //public GameObject m_objStage;

    //--------------------------------------------------------------------------
    //  変数
    //--------------------------------------------------------------------------
    private Rigidbody m_rbLeftWall;
    private Rigidbody m_rbRightWall;
    private float m_fLTValue;
    private float m_fRTValue;
    private bool m_bCanControl;

    public void DisableControl()
    {
        m_bCanControl = false;
    }

    private void Awake()
    {
        m_rbLeftWall = GameObject.FindGameObjectWithTag("LeftWall").GetComponent<Rigidbody>();
        m_rbRightWall = GameObject.FindGameObjectWithTag("RightWall").GetComponent<Rigidbody>();

        m_rbLeftWall.isKinematic = false;
        m_rbRightWall.isKinematic = false;
    }

    // Use this for initialization
    private void Start()
    {
        m_rbLeftWall.position = new Vector3(-m_fPosX, m_fPosY, 0.0f);
        m_rbRightWall.position = new Vector3(m_fPosX, m_fPosY, 0.0f);

        m_bCanControl = true;
        m_fLTValue = 0.0f;
        m_fRTValue = 0.0f;
    }

    private void FixedUpdate()
    {
        if (!GameManager.m_bPlay) { return; }

        if (m_bCanControl)
        {
            Move();
        }
    }

    private void Move()
    {
        Vector3 vPosL = m_rbLeftWall.position;
        Vector3 vPosR = m_rbRightWall.position;

        //Left wall
        if (m_fLTValue > 0.0f)
        {
            vPosL.x += m_fSpeed;
        }
        else if (vPosL.x > -m_fPosX)
        {
            vPosL.x -= m_fSpeed*2;
        }

        //Right wall
        if (m_fRTValue < 0.0f)
        {
            vPosR.x -= m_fSpeed;
        }
        else if (vPosR.x < m_fPosX)
        {
            vPosR.x += m_fSpeed*2;
        }

        m_rbLeftWall.MovePosition(vPosL);
        m_rbRightWall.MovePosition(vPosR);
    }

    private void Update()
    {
        if (!GameManager.m_bPlay) { return; }

        if (m_bCanControl)
        {
            m_fLTValue = Input.GetAxis("LT");
            m_fRTValue = Input.GetAxis("RT");
        }
        //else
        //{
        //    m_fLTValue = 0.0f;
        //    m_fRTValue = 0.0f;
        //}
    }
}
