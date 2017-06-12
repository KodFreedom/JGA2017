using System.Collections;
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
    private RigidbodyConstraints m_constraintsClearMode = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;

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
    private STATUS m_status;
    private float m_fDistoGround;

    public void SetStatusNormal()
    {
        if (m_status == STATUS.PLAYER_CLEAR) { return; }
        if (m_status != STATUS.PLAYER_FALLING) { return; }
        m_status = STATUS.PLAYER_LANDING;
    }

    public void SetStatusFalling()
    {
        if (m_status == STATUS.PLAYER_CLEAR) { return; }
        if (m_status == STATUS.PLAYER_FALLING || m_status == STATUS.PLAYER_CLEAR) { return; }
        m_status = STATUS.PLAYER_FALLING;
    }

    public void SetStatusClear()
    {
        if (m_status == STATUS.PLAYER_CLEAR) { return; }
        m_status = STATUS.PLAYER_CLEAR;
        transform.SetParent(null, true);
        GameObject.FindGameObjectWithTag("StageSelecter").GetComponent<StageSelecter>().StageClear();
    }

    public void SetStatusGameOver()
    {
        if (m_status == STATUS.PLAYER_GAMEOVER) { return; }
        m_status = STATUS.PLAYER_GAMEOVER;
    }

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    private void Start()
    {
        m_status = STATUS.PLAYER_FALLING;
        m_bJump = false;
        m_fDistoGround = GetComponent<Collider>().bounds.extents.y;
    }

    private void FixedUpdate()
    {
        float fMoveHorizontal = Input.GetAxis("Horizontal");
        float fMoveVertical = Input.GetAxis("Vertical");
        Vector3 vMovement = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 vForceDir;

        if(m_status == STATUS.PLAYER_FALLING && (fMoveHorizontal != 0.0f || fMoveVertical != 0.0f))
        {
            m_rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        }

        switch (m_status)
        {
            case STATUS.PLAYER_NORMAL:
                m_rb.mass = m_fMassNormal;
                vMovement = new Vector3(fMoveHorizontal, 0.0f, 0.0f) * m_fMoveSpeedNormal;
                if (m_bJump && Input.GetKeyDown("joystick button 0"))
                {
                    m_rb.velocity += new Vector3(0.0f, m_fJumpSpeed, 0.0f);
                    m_bJump = false;
                    AkSoundEngine.PostEvent("Jump", gameObject);
                }
                vForceDir = new Vector3(0.0f, -1.0f, 0.0f);
                m_rb.AddForce(vForceDir * m_fGravity * m_rb.mass * Time.deltaTime);
                m_rb.MovePosition(m_rb.position + vMovement);
                break;
            case STATUS.PLAYER_LANDING:
                m_rb.mass = m_fMassFalling;
                //vMovement = new Vector3(fMoveHorizontal, -fMoveVertical, 0.0f) * m_fMoveSpeedFalling;
                vForceDir = new Vector3(0.0f, -1.0f, 0.0f);
                m_rb.AddForce(vForceDir * m_fGravity * m_rb.mass * Time.deltaTime);
                //m_rb.MovePosition(m_rb.position + vMovement);
                break;
            case STATUS.PLAYER_FALLING:
                m_rb.mass = m_fMassFalling;
                vMovement = new Vector3(fMoveHorizontal, fMoveVertical, 0.0f) * m_fMoveSpeedFalling;
                vForceDir = new Vector3(0.0f, 1.0f, 0.0f);
                m_rb.AddForce(vForceDir * m_fBouyant * m_rb.mass * Time.deltaTime);
                m_rb.MovePosition(m_rb.position + vMovement);
                break;
            case STATUS.PLAYER_CLEAR:
                vForceDir = new Vector3(0.0f, 1.0f, 0.0f);
                m_rb.AddForce(vForceDir * m_fBouyant * m_rb.mass * Time.deltaTime);
                break;
            case STATUS.PLAYER_GAMEOVER:
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (Time.timeScale != 0)
        {
            if (m_status != STATUS.PLAYER_FALLING && IsGrounded())
            {
                m_bJump = true;
                if (m_status == STATUS.PLAYER_LANDING)
                {
                    m_status = STATUS.PLAYER_NORMAL;
                }
            }
            else
            {
                m_bJump = false;
            }
        }  
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Goal") && m_status == STATUS.PLAYER_NORMAL)
        {//Stage Clear
            m_rb.constraints = m_constraintsClearMode;
            Vector3 vDir = new Vector3(0.0f, 0.0f, -1.0f);
            m_rb.AddForce(vDir * m_fStageClearForce);
            SetStatusClear();
        }
    }

    private bool IsGrounded()
    {
        float fRadius = transform.localScale.x * 0.5f;
        Vector3[] avPos = new Vector3[3];
        avPos[0] = transform.position - new Vector3(fRadius, 0.0f, 0.0f);
        avPos[1] = transform.position;
        avPos[2] = transform.position + new Vector3(fRadius, 0.0f, 0.0f);

        if(Physics.Raycast(avPos[0], -Vector3.up, m_fDistoGround + 0.1f, -1, QueryTriggerInteraction.Ignore)
            || Physics.Raycast(avPos[1], -Vector3.up, m_fDistoGround + 0.1f, -1, QueryTriggerInteraction.Ignore)
            || Physics.Raycast(avPos[2], -Vector3.up, m_fDistoGround + 0.1f, -1, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        return false;
    }
}
