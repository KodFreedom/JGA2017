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
    public ParticleSystem m_particleFalling;
    public PhysicMaterial m_gameoverPM;
    
    //UI
    public Image m_imgUIStage;
    public Image[] m_aImgUIStageNum;
    public Sprite[] m_aNumbers;
    private const float c_fUIStageYMax = 331.0f;
    private const float c_fUIStageYMin = -310.0f;
    private const int c_nUIEffectTime = 40;

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
    private bool m_bFirstGameover;
    private Vector3 m_vLeftParticlePos;
    private Vector3 m_vRightParticlePos;
    private int m_nCntUIEffect;
    private int m_nCntEffect500;
    private bool m_bEffected500;
    private bool m_bEffectFinal;
    private InputManager m_Input;
    private int m_nCntVibrationL;
    private int m_nCntVibrationR;
    private int m_nCntVibrationLR;

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
        m_bFirstGameover = true;
        m_fLTValue = 0.0f;
        m_fRTValue = 0.0f;
        m_fLTOld = 0.0f;
        m_fRTOld = 0.0f;
        m_vVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        float fSizeX = gameObject.GetComponent<BoxCollider>().size.x;
        m_vLeftParticlePos = new Vector3(-fSizeX * 0.5f - 0.25f, 0.0f, 0.0f);
        m_vRightParticlePos = new Vector3(fSizeX * 0.5f + 0.25f, 0.0f, 0.0f);

        //UI
        m_nCntUIEffect = 0;
        m_nCntEffect500 = 0;
        m_bEffected500 = false;
        m_bEffectFinal = false;

        //Input
        m_Input = GameObject.Find("EventSystem").GetComponent<InputManager>();
        m_nCntVibrationL = 0;
        m_nCntVibrationR = 0;
        m_nCntVibrationLR = 0;

        //Particle
        Vector3 vSize = gameObject.GetComponent<BoxCollider>().size;
        //m_particleFalling.transform.parent = transform;
        m_particleFalling.transform.localPosition = new Vector3(0f, vSize.y * 0.5f, 0f);
        ParticleSystem.ShapeModule shape = m_particleFalling.shape;
        shape.radius = vSize.x * 0.5f;
    }

    private void FixedUpdate()
    {
        if (!GameManager.m_bPlay) { return; }

        //if(Time.timeScale != 0)
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

                //Effect & Sound
                PlayWallContactEffect(transform.position + m_vLeftParticlePos, Quaternion.Euler(0f, -90.0f, 0f), EFFECT_IDX.LEFT);
                AkSoundEngine.PostEvent("left_wall", gameObject);

                //VibrationL
                m_nCntVibrationL = 10;
            }

            if (m_bPushedR && !m_bPushedL && m_fRTValue < 0.0f && m_fRTValue <= m_fRTOld && m_vVelocity.x >= 0.0f)
            {//Right Wall
                m_vVelocity = new Vector3(-m_fVelocityX, m_vVelocity.y, 0.0f);

                //Effect & Sound
                PlayWallContactEffect(transform.position + m_vRightParticlePos, Quaternion.Euler(0f, 90.0f, 0f), EFFECT_IDX.RIGHT);
                AkSoundEngine.PostEvent("right_wall", gameObject);

                //VibrationR
                m_nCntVibrationR = 10;
            }

            m_vVelocity.y -= m_fGravity;
            m_vVelocity *= m_fDamping;
        }
    }

    private void Update()
    {
        if (!GameManager.m_bPlay) { return; }

        //if (Time.timeScale != 0)
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

            //地面との距離が10以下だったら挟めない
            if(transform.position.y <= m_objPlane.transform.position.y + fHeightStage * 0.5f + 10f)
            {
                GameObject.FindGameObjectWithTag("StageSelecter").GetComponent<StageSelecter>().PrepareFailed();
            }

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

        //BGM
		float fValue = -transform.position.y * 0.1f;
		AkSoundEngine.SetRTPCValue ("room_height", fValue);

        //Particle
        ParticleSystem.MainModule main = m_particleFalling.main;
        main.startSpeed = -m_vVelocity.y * 5.0f;

        //Vibration
        UpdateVibration();

        //UI
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
            m_particleFalling.Stop();

            //VibrationLR
            m_nCntVibrationLR = 20;
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
            m_particleFalling.Play();
            ParticleSystem.MainModule main = m_particleFalling.main;
            main.startSpeed = 0f;
            Debug.Log("fall start");
            AkSoundEngine.PostEvent("fall_start", gameObject);
        }
    }

    private void GameOver()
    {
        if (!m_bFirstGameover) { return; }
        m_bFirstGameover = false;
        Transform[] childs = transform.GetComponentsInChildren<Transform>();
        for(int nCnt = 0;nCnt < childs.Length;nCnt++)
        {
            if(childs[nCnt].name == "group3" || childs[nCnt].name == "polySurface1" || childs[nCnt].name == "polySurface2" || childs[nCnt].name == "polySurface18") { continue; }
            childs[nCnt].SetParent(null, true);

            //Collider
            Collider cd = childs[nCnt].gameObject.GetComponent<Collider>();
            if (!cd || cd.isTrigger) { continue; }
            cd.material = m_gameoverPM;

            Rigidbody rb = childs[nCnt].gameObject.GetComponent<Rigidbody>();
            if (!rb) { rb = childs[nCnt].gameObject.AddComponent<Rigidbody>(); }
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.mass = 1.0f;
            
            rb.AddForce(Vector3.down * 2000.0f);
            rb.constraints = RigidbodyConstraints.None;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        Rigidbody srb = gameObject.GetComponent<Rigidbody>();
        if(srb)
        {
            Destroy(srb);
            //srb.isKinematic = true;
        }

        GameObject.FindGameObjectWithTag("StageSelecter").GetComponent<StageSelecter>().StageFailed();

        //Particle
        m_particleFalling.Stop();

        //Vibration
        m_nCntVibrationLR = 60;
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
        int nDis = (int)fDis >= 1000 ? 999 : (int)fDis <= 2 ? 0 : (int)fDis;

        //Effect Cnt
        if (nDis <= 500 && !m_bEffected500)
        {
            m_bEffected500 = true;
            m_nCntEffect500 = c_nUIEffectTime;
        }
        else if(nDis <= 150)
        {
            m_bEffectFinal = true;
        }

        for(int nCnt = 0;nCnt < 3;nCnt++)
        {
            int nNum = nDis % 10;
            nDis /= 10;
            m_aImgUIStageNum[nCnt].sprite = m_aNumbers[nNum];
        }

        //Base
        m_imgUIStage.rectTransform.anchoredPosition = new Vector2(m_imgUIStage.rectTransform.anchoredPosition.x, fY);

        //Effect
        if(m_nCntEffect500 > 0 || m_bEffectFinal)
        {
            int nBase = c_nUIEffectTime / 4;
            float fRate = 0.0f;
            m_nCntEffect500--;
            m_nCntUIEffect = (m_nCntUIEffect + 1) % c_nUIEffectTime;

            if (m_nCntUIEffect < nBase)
            {
                fRate = (float)(m_nCntUIEffect % nBase) / nBase;
            }
            else if (m_nCntUIEffect < nBase * 2)
            {
                fRate = 1f - (float)(m_nCntUIEffect % nBase) / nBase;
            }
            else if(m_nCntUIEffect < nBase * 3)
            {
                fRate = (float)(m_nCntUIEffect % nBase) / nBase;
            }
            else
            {
                fRate = 1f - (float)(m_nCntUIEffect % nBase) / nBase;
            }
            Color cColor = Color.Lerp(Color.white, Color.red, fRate);

            if(fDis <= 2f)
            {//GameOver
                cColor = Color.white;
            }
            for (int nCnt = 0; nCnt < 3; nCnt++)
            {
                m_aImgUIStageNum[nCnt].color = cColor;
            }
        }
    }

    private void UpdateVibration()
    {
        bool bLeft = false;
        bool bRight = false;
        if (m_nCntVibrationL > 0)
        {
            m_nCntVibrationL--;
            bLeft = true;
        }
        if (m_nCntVibrationR > 0)
        {
            m_nCntVibrationR--;
            bRight = true;
        }
        if (m_nCntVibrationLR > 0)
        {
            m_nCntVibrationLR--;
            bLeft = true;
            bRight = true;
        }
        m_Input.Vibration(bLeft, bRight);
    }
}
