using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GimicController : MonoBehaviour {
    //--------------------------------------------------------------------------
    //  構造体定義
    //--------------------------------------------------------------------------
    protected enum STATUS
    {
        NORMAL = 0,
        FALLING = 1
    }

    //--------------------------------------------------------------------------
    //  定数
    //--------------------------------------------------------------------------
    public float m_fBouyant;
    public float m_fGravity;
    public Material m_mat;
    public PhysicMaterial m_phyMat;
    public ParticleSystem m_psLanding;

    //--------------------------------------------------------------------------
    //  変数
    //--------------------------------------------------------------------------
    protected Rigidbody m_rb;
    protected STATUS m_status;
    protected bool m_bIsFirstTouch;
    protected float m_fDistoGround;

    //--------------------------------------------------------------------------
    //  Public関数
    //--------------------------------------------------------------------------
    public virtual void SetStatusNormal()
    {
        if (m_status == STATUS.NORMAL) { return; }
        m_status = STATUS.NORMAL;
        m_rb.velocity = Vector3.zero;
        m_bIsFirstTouch = true;
    }

    public virtual void SetStatusFalling()
    {
        if (m_status == STATUS.FALLING) { return; }
        m_status = STATUS.FALLING;
        m_rb.velocity = Vector3.zero;
    }

    //--------------------------------------------------------------------------
    //  Protected関数
    //--------------------------------------------------------------------------
    protected virtual void Start ()
    {
        GetComponent<MeshRenderer>().material = m_mat;
        GetComponent<BoxCollider>().material = m_phyMat;
        m_rb = GetComponent<Rigidbody>();
        m_status = STATUS.FALLING;
        m_bIsFirstTouch = true;
        m_fDistoGround = GetComponent<Collider>().bounds.extents.y;
        m_psLanding.transform.parent = transform.parent;
        m_psLanding.transform.localScale = Vector3.one;
        ParticleSystem.ShapeModule shape = m_psLanding.shape;
        Vector3 vScale = transform.localScale;
        vScale.y = 0f;
        shape.box = vScale;
    }

    protected virtual void FixedUpdate()
    {
        if (Time.timeScale != 0)
        {
            Vector3 vForceDir;

            switch (m_status)
            {
                case STATUS.NORMAL:
                    vForceDir = new Vector3(0.0f, -1.0f, 0.0f);
                    m_rb.AddForce(vForceDir * m_fGravity * m_rb.mass * Time.deltaTime);
                    break;
                case STATUS.FALLING:
                    vForceDir = new Vector3(0.0f, 1.0f, 0.0f);
                    m_rb.AddForce(vForceDir * m_fBouyant * m_rb.mass * Time.deltaTime);
                    StageController sc = GetComponentInParent<StageController>();
                    if (sc)
                    {
                        m_rb.velocity = new Vector3(-sc.GetVelocity().x * 10.0f, m_rb.velocity.y, 0.0f);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    protected virtual void Update()
    {
        if (Time.timeScale != 0)
        {
            if (m_status == STATUS.NORMAL)
            {
                //Check Landing
                if (m_bIsFirstTouch && IsGrounded())
                {
                    m_bIsFirstTouch = false;

                    //Landing Effect & Sound
                    Vector3 vP = transform.position;
                    Vector3 vS = transform.localScale;
                    m_psLanding.transform.position = new Vector3(vP.x, vP.y - vS.y * 0.5f, vP.z);
                    m_psLanding.Play();
                    PlayHitSound();
                }
            }
        } 
    }

    protected virtual void OnCollisionEnter(Collision collision) { }

    protected virtual bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, m_fDistoGround + 0.05f, LayerMask.GetMask("StageParts"));
    }

    protected virtual void PlayHitSound() { }
}

