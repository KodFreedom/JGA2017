﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //--------------------------------------------------------------------------
    //  定数
    //--------------------------------------------------------------------------
    public float m_fMoveSpeedFalling;
    public float m_fMoveSpeedNormal;
    public float m_fJumpSpeed;
    public float m_fBouyant;
    public float m_fGravity;
    public float m_fMassFalling;
    public float m_fMassNormal;
    public float m_fStageClearForce;
    public float m_modelRotTime;
    public GameObject m_model;

    private RigidbodyConstraints m_constraintsClearMode = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
    private Quaternion m_qRotLeft = Quaternion.Euler(0f, 270f, 0f);
    private Quaternion m_qRotRight = Quaternion.Euler(0f, 90f, 0f);
    private const float c_fSpeedYMin = 0.01f;

    //--------------------------------------------------------------------------
    //  構造体定義
    //--------------------------------------------------------------------------
    enum STATUS
    {
        PLAYER_NORMAL = 0,
        PLAYER_LANDING,
        PLAYER_FALLING,
        PLAYER_CLEAR,
        PLAYER_GAMEOVER
    }

    //--------------------------------------------------------------------------
    //  変数
    //--------------------------------------------------------------------------
    private Rigidbody m_rb;
    private bool m_bJump;
    private bool m_bJumpPressed;
    private STATUS m_status;
    private float m_fDistoGround;
	private int m_nCnt;
    private InputManager m_Input;
    private int m_nCntClear;

    public void SetStatusNormal()
    {
        if (m_status == STATUS.PLAYER_CLEAR) { return; }
        if (m_status != STATUS.PLAYER_FALLING) { return; }
        m_status = STATUS.PLAYER_LANDING;
        m_rb.mass = m_fMassFalling;
        m_rb.velocity = Vector3.zero;
		m_nCnt = 3;
    }

    public void SetStatusFalling()
    {
        if (m_status == STATUS.PLAYER_CLEAR) { return; }
        if (m_status == STATUS.PLAYER_FALLING || m_status == STATUS.PLAYER_CLEAR) { return; }
        m_status = STATUS.PLAYER_FALLING;
        m_rb.mass = m_fMassFalling;
        m_rb.velocity = Vector3.zero;
    }

    public void SetStatusClear()
    {
        if (m_status == STATUS.PLAYER_CLEAR) { return; }
        m_status = STATUS.PLAYER_CLEAR;
        m_nCntClear = 600;
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        for (int nCnt = 0; nCnt < children.Length; nCnt++)
        {
            children[nCnt].localRotation = Quaternion.Euler(0f, 180f, 0f);
        }
        transform.SetParent(null, true);
        GameObject.Find("Main Camera").GetComponent<CameraController>().StageClearCamera();
        GameObject.FindGameObjectWithTag("StageSelecter").GetComponent<StageSelecter>().StageClear();

        //Effect
        Transform trans = transform.FindChild("Spiral_02.1.3 Portal");
        if(trans)
        {
            trans.gameObject.SetActive(true);
        }
    }

    public void SetStatusGameOver()
    {
        if (m_status == STATUS.PLAYER_GAMEOVER) { return; }
        m_status = STATUS.PLAYER_GAMEOVER;
    }

    // Use this for initialization
    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_status = STATUS.PLAYER_FALLING;
        m_rb.mass = m_fMassFalling;
        m_bJump = false;
        m_bJumpPressed = false;
        m_fDistoGround = GetComponent<Collider>().bounds.extents.y;
		m_nCnt = 0;
        m_Input = GameObject.Find("EventSystem").GetComponent<InputManager>();
    }

    private void FixedUpdate()
    {
        if (!GameManager.m_bPlay) { return; }

        float fMoveHorizontal = m_Input.GetAxis(InputManager.EBUTTON.Horizontal);
        float fMoveVertical = m_Input.GetAxis(InputManager.EBUTTON.Vertical);
        Vector3 vMovement = Vector3.zero;

        switch (m_status)
        {
            case STATUS.PLAYER_NORMAL:
                {
                    vMovement = new Vector3(fMoveHorizontal, 0.0f, 0.0f) * m_fMoveSpeedNormal;
                    float fGravity = m_fGravity * m_rb.mass * Time.deltaTime;

                    //ジャンプ
                    if (m_rb.velocity.y >= -c_fSpeedYMin &&
                        m_bJump && !m_bJumpPressed &&
                        m_Input.GetButton(InputManager.EBUTTON.Jump))
                    {
                        m_rb.velocity = new Vector3(m_rb.velocity.x, m_fJumpSpeed, m_rb.velocity.z);
                        m_bJump = false;
                        m_bJumpPressed = true;
                        AkSoundEngine.PostEvent("Jump", gameObject);
                    }
                    m_fGravity = 200.0f;
                    m_rb.AddForce(Vector3.down * fGravity);
                    m_rb.MovePosition(m_rb.position + vMovement);

                    //Model Rotation
                    RotModel(fMoveHorizontal);
                    break;
                }
            case STATUS.PLAYER_LANDING:
                {
                    //バグ防止ため加速度を上がる
                    if (m_nCnt > 0)
                    {
                        m_nCnt--;
                        m_fGravity = 1000.0f;
                    }
                    else
                    {
                        m_fGravity = 200.0f;

                        //着陸判定
                        CheckLanded();
                    }

                    //重力
                    m_rb.AddForce(Vector3.down * m_fGravity * m_rb.mass * Time.deltaTime);
                    break;
                }
            case STATUS.PLAYER_FALLING:
                {
                    //操作する時速度を0にする
                    if ((fMoveHorizontal != 0.0f || fMoveVertical != 0.0f)) { m_rb.velocity = Vector3.zero; }

                    //移動処理
                    vMovement = new Vector3(fMoveHorizontal * 0.75f, fMoveVertical, 0.0f) * m_fMoveSpeedFalling;
                    m_rb.AddForce(Vector3.up * m_fBouyant * m_rb.mass * Time.deltaTime);
                    m_rb.MovePosition(m_rb.position + vMovement);
                    //ModelRotation
                    RotModel(fMoveHorizontal);
                    break;
                }
            case STATUS.PLAYER_CLEAR:
                if(m_nCntClear > 0)
                {
                    m_rb.AddForce(Vector3.up * m_fBouyant * m_rb.mass * Time.deltaTime);
                    AkSoundEngine.PostEvent("fall_stop", gameObject);
                    m_nCntClear--;
                }
                else
                {
                    m_rb.velocity *= 0.95f;
                }
                break;
            case STATUS.PLAYER_GAMEOVER:
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (!GameManager.m_bPlay) { return; }
        if (m_status == STATUS.PLAYER_GAMEOVER) { return; }
        if (m_status == STATUS.PLAYER_NORMAL && IsGrounded())
        {
            m_bJump = true;
        }
        else
        {
            m_bJump = false;
        }
        m_rb.velocity = new Vector3(0f, m_rb.velocity.y, m_rb.velocity.z);
        if (m_Input.GetButtonUp(InputManager.EBUTTON.Jump)) { m_bJumpPressed = false; }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Goal") && m_status == STATUS.PLAYER_NORMAL)
        {//Stage Clear
            m_rb.constraints = m_constraintsClearMode;
            Vector3 vDir = new Vector3(0.0f, 0.0f, -1.0f);
            m_rb.AddForce(vDir * m_fStageClearForce);
            SetStatusClear();
            other.gameObject.GetComponent<GoalController>().StageClearEffect();
        }
    }

    private bool IsGrounded()
    {
        float fRadius = transform.localScale.x * 0.5f;
        Vector3[] avPos = new Vector3[3];
        avPos[0] = transform.position - new Vector3(fRadius * 0.2f, 0.0f, 0.0f);
        avPos[1] = transform.position;
        avPos[2] = transform.position + new Vector3(fRadius * 0.2f, 0.0f, 0.0f);

        if(Physics.Raycast(avPos[0], Vector3.down, m_fDistoGround + 0.005f, -1, QueryTriggerInteraction.Ignore)
            || Physics.Raycast(avPos[1], Vector3.down, m_fDistoGround + 0.005f, -1, QueryTriggerInteraction.Ignore)
            || Physics.Raycast(avPos[2], Vector3.down, m_fDistoGround + 0.005f, -1, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        return false;
    }

    private void CheckLanded()
    {
        if (IsGrounded() && m_rb.velocity.y >= -c_fSpeedYMin)
        {
            
            m_status = STATUS.PLAYER_NORMAL;
            m_rb.mass = m_fMassNormal;
        }
    }

    private void RotModel(float fMoveHorizontal)
    {
        if (!m_model) { return; }

        if(fMoveHorizontal < 0f)
        {//左向き
            m_model.transform.rotation = Quaternion.Lerp(m_model.transform.rotation, m_qRotLeft, m_modelRotTime);
        }
        else if(fMoveHorizontal > 0f)
        {//右向き
            m_model.transform.rotation = Quaternion.Lerp(m_model.transform.rotation, m_qRotRight, m_modelRotTime);
        }
    }
}
