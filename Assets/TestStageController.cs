using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestStageController : MonoBehaviour
{
    //--------------------------------------------------------------------------
    //  定数
    //--------------------------------------------------------------------------
    public float m_fGravity = 0.5f / 1000.0f;
    public float m_fDamping = 0.99f;
    public GameObject m_objPlane;
    public PhysicMaterial m_gameoverPM;

    //--------------------------------------------------------------------------
    //  変数
    //--------------------------------------------------------------------------
    private Vector3 m_vVelocity;
    private bool m_bFirstGameover;
    private bool m_bEffectFinal;

    public Vector3 GetVelocity()
    {
        return m_vVelocity;
    }

    // Use this for initialization
    void Start()
    {
        m_bFirstGameover = true;
        m_vVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        float fSizeX = gameObject.GetComponent<BoxCollider>().size.x;
    }

    private void FixedUpdate()
    {
        if (!GameManager.m_bPlay) { return; }

        m_vVelocity.y -= m_fGravity;
        m_vVelocity *= m_fDamping;
    }

    private void Update()
    {
        if (!GameManager.m_bPlay) { return; }

        //if (Time.timeScale != 0)
        {
            Vector3 vPos = transform.position;

            float fLengthStage = GetComponent<BoxCollider>().size.x;
            float fHeightStage = GetComponent<BoxCollider>().size.y;

            UpdateWhenNotBeingNipped();

            transform.position += m_vVelocity;

            //地面との当たり判定
            if (transform.position.y <= m_objPlane.transform.position.y + fHeightStage * 0.5f)
            {
                transform.position = new Vector3(transform.position.x, m_objPlane.transform.position.y + fHeightStage * 0.5f, transform.position.z);
                GameOver();
                m_vVelocity *= 0.0f;
            }
        }
    }

    private void UpdateWhenNotBeingNipped()
    {
        GimicController[] gimic = GetComponentsInChildren<GimicController>();
        PlayerController player = GetComponentInChildren<PlayerController>();

        if (player)
        {
            player.SetStatusFalling();
        }

        for (int nCnt = 0; nCnt < gimic.Length; nCnt++)
        {
            gimic[nCnt].SetStatusFalling();
        }
    }

    private void GameOver()
    {
        if (!m_bFirstGameover) { return; }
        m_bFirstGameover = false;
        Transform[] childs = transform.GetComponentsInChildren<Transform>();
        for (int nCnt = 0; nCnt < childs.Length; nCnt++)
        {
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
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        Rigidbody srb = gameObject.GetComponent<Rigidbody>();
        if (srb)
        {
            Destroy(srb);
            //srb.isKinematic = true;
        }
    }
}
