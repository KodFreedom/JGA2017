using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GimicController : MonoBehaviour {
    //--------------------------------------------------------------------------
    //  構造体定義
    //--------------------------------------------------------------------------
    enum STATUS
    {
        GIMIC_NORMAL = 0,
        GIMIC_FALLING = 1
    }

    public enum TYPE
    {
        CUBE,
        IRON,
        ICE,
        GAM,
        GOM,
        HORIZONTAL,
        VERTICAL
    }

    //--------------------------------------------------------------------------
    //  定数
    //--------------------------------------------------------------------------
    public float m_fBouyant;
    public float m_fGravity;
    public float m_fIceBreakHeight;
    public TYPE m_type;
    public Material m_matCube;
    public Material m_matIron;
    public Material m_matIce;
    public Material m_matGam;
    public Material m_matGom;
    public Material m_matHorizontal;
    public Material m_matVertical;
    public PhysicMaterial m_physicMatNormal;
    public PhysicMaterial m_physicMatGom;
    public ParticleSystem m_landingEffect;

    //--------------------------------------------------------------------------
    //  変数
    //--------------------------------------------------------------------------
    private Rigidbody   m_rb;
    private STATUS      m_status;
    private int         m_nLife;
    private Vector3     vOriginPos;
    private Quaternion  qOriginRot;

    //ice用変数
    private bool        m_bFirstTouch;
    private float       m_fOldY;
    private float       m_fCntHeight;
    private float       m_fDistoGround;

    public void SetStatusNormal()
    {
        if(m_status != STATUS.GIMIC_FALLING) { return; }
        m_status = STATUS.GIMIC_NORMAL;
        m_rb.velocity *= 0.0f;
        m_bFirstTouch = false;
        if (m_type == TYPE.ICE)
        {
            m_fCntHeight = 0.0f;
            m_fOldY = m_rb.position.y;
        }
    }

    public void SetStatusFalling()
    {
        if(m_status == STATUS.GIMIC_FALLING) { return; }
        m_status = STATUS.GIMIC_FALLING;
        if(m_type == TYPE.HORIZONTAL || m_type == TYPE.VERTICAL)
        {
            m_rb.velocity *= -1.0f;
        }
    }

    public void ResetSelf()
    {
        transform.position = vOriginPos;
        transform.rotation = qOriginRot;
        m_rb.velocity = Vector3.zero;
        m_rb.angularVelocity = Vector3.zero;
        Start();
    }

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        vOriginPos = transform.position;
        qOriginRot = transform.rotation;
    }

    // Use this for initialization
    private void Start ()
    {
        m_status = STATUS.GIMIC_FALLING;
        m_bFirstTouch = false;
        m_fDistoGround = GetComponent<Collider>().bounds.extents.y;
        m_landingEffect.transform.parent = transform.parent;

        switch (m_type)
        {
            case TYPE.CUBE:
                m_nLife = -1;
                gameObject.GetComponent<MeshRenderer>().material = m_matCube;
                gameObject.GetComponent<BoxCollider>().material = m_physicMatNormal;
                break;
            case TYPE.IRON:
                m_nLife = -1;
                gameObject.GetComponent<MeshRenderer>().material = m_matIron;
                gameObject.GetComponent<BoxCollider>().material = m_physicMatNormal;
                break;
            case TYPE.ICE:
                m_nLife = 2;
                gameObject.GetComponent<MeshRenderer>().material = m_matIce;
                gameObject.GetComponent<BoxCollider>().material = m_physicMatNormal;
                break;
            case TYPE.GAM:
                m_nLife = -1;
                gameObject.GetComponent<MeshRenderer>().material = m_matGam;
                gameObject.GetComponent<BoxCollider>().material = m_physicMatNormal;
                break;
            case TYPE.GOM:
                m_nLife = -1;
                gameObject.GetComponent<MeshRenderer>().material = m_matGom;
                gameObject.GetComponent<BoxCollider>().material = m_physicMatGom;
                break;
            case TYPE.HORIZONTAL:
                m_nLife = -1;
                gameObject.GetComponent<MeshRenderer>().material = m_matHorizontal;
                gameObject.GetComponent<BoxCollider>().material = m_physicMatNormal;
                break;
            case TYPE.VERTICAL:
                m_nLife = -1;
                gameObject.GetComponent<MeshRenderer>().material = m_matVertical;
                gameObject.GetComponent<BoxCollider>().material = m_physicMatNormal;
                break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        Vector3 vForceDir;

        switch (m_status)
        {
            case STATUS.GIMIC_NORMAL:
                m_fOldY = m_rb.position.y;
                vForceDir = new Vector3(0.0f, -1.0f, 0.0f);
                m_rb.AddForce(vForceDir * m_fGravity * m_rb.mass * Time.deltaTime);
                break;
			case STATUS.GIMIC_FALLING:
				vForceDir = new Vector3 (0.0f, 1.0f, 0.0f);
				m_rb.AddForce (vForceDir * m_fBouyant * m_rb.mass * Time.deltaTime);
                StageController sc = GetComponentInParent<StageController>();
                if(sc)
                {
                    m_rb.velocity = new Vector3(-GetComponentInParent<StageController>().GetVelocity().x * 10.0f, m_rb.velocity.y, 0.0f);
                }
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (Time.timeScale != 0)
        {
            if(m_status == STATUS.GIMIC_NORMAL)
            {
                if (m_type == TYPE.ICE)
                {
                    if (m_fOldY > m_rb.position.y)
                    {
                        m_fCntHeight += m_fOldY - m_rb.position.y;
                    }

                    //ライフ減らす
                    if (IsGrounded() && !m_bFirstTouch)
                    {
                        m_bFirstTouch = true;

                        if (m_fCntHeight >= m_fIceBreakHeight)
                        {
                            m_fCntHeight = 0.0f;
                            m_nLife--;

                            //Sound
                            if (m_nLife != 0) { AkSoundEngine.PostEvent("ice_hibi", gameObject); }
                        }
                        else
                        {
                            AkSoundEngine.PostEvent("ice_hit_wall", gameObject);
                        }

                        if (m_nLife == 0)
                        {
                            AkSoundEngine.PostEvent("ice_break", gameObject);
                            gameObject.SetActive(false);
                        }
                    }
                }
            }
        } 
    }

    private void OnCollisionEnter(Collision collision)
    {
        //ガムかどうかをチェック,
        //ガムだったらFixedJointを作る
        if(m_type != TYPE.IRON && collision.gameObject.CompareTag("Gimic"))
        {
            GimicController gimic = collision.gameObject.GetComponent<GimicController>();
            if(gimic.m_type == TYPE.GAM)
            {
                FixedJoint joint = gameObject.GetComponent<FixedJoint>();
                if(!joint)
                {
                    gameObject.AddComponent<FixedJoint>();
                    gameObject.GetComponent<FixedJoint>().connectedBody = collision.rigidbody;
                }
            }
        }

        ////Sound
        //if(m_status == STATUS.GIMIC_NORMAL && !collision.gameObject.CompareTag("Player"))
        //{
        //    switch (m_type)
        //    {
        //        case TYPE.CUBE:
        //            AkSoundEngine.PostEvent("cube_hit_wall", gameObject);
        //            break;
        //        case TYPE.IRON:
        //            AkSoundEngine.PostEvent("iron_hit_wall", gameObject);
        //            break;
        //        case TYPE.ICE:
        //            //AkSoundEngine.PostEvent("ice_hit_wall", gameObject);
        //            break;
        //        case TYPE.GAM:
        //            AkSoundEngine.PostEvent("gam_hit_gam", gameObject);
        //            break;
        //        case TYPE.GOM:
        //            AkSoundEngine.PostEvent("gom_hit_gom", gameObject);
        //            break;
        //        default:
        //            AkSoundEngine.PostEvent("cube_hit_wall", gameObject);
        //            break;
        //    }
        //}
    }

    private void OnCollisionStay(Collision collision)
    {
        if (m_status == STATUS.GIMIC_NORMAL && m_rb.velocity.y == 0f)
        {
            if (IsGrounded() && !m_bFirstTouch && !collision.gameObject.CompareTag("Player"))
            {
                m_bFirstTouch = true;
                Vector3 vP = transform.position;
                Vector3 vS = transform.localScale;
                m_landingEffect.transform.position = new Vector3(vP.x, vP.y - vS.y * 0.5f, vP.z);
                m_landingEffect.Play();

                switch (m_type)
                {
                    case TYPE.CUBE:
                        AkSoundEngine.PostEvent("cube_hit_wall", gameObject);
                        break;
                    case TYPE.IRON:
                        AkSoundEngine.PostEvent("iron_hit_wall", gameObject);
                        break;
                    case TYPE.ICE:
                        //AkSoundEngine.PostEvent("ice_hit_wall", gameObject);
                        break;
                    case TYPE.GAM:
                        AkSoundEngine.PostEvent("gam_hit_gam", gameObject);
                        break;
                    case TYPE.GOM:
                        AkSoundEngine.PostEvent("gom_hit_gom", gameObject);
                        break;
                    default:
                        AkSoundEngine.PostEvent("cube_hit_wall", gameObject);
                        break;
                }
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, m_fDistoGround + 0.05f);
    }
}
