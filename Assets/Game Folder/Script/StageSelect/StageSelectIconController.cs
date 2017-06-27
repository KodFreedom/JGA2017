using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StageSelectIconController : MonoBehaviour
{
    //--------------------------------------------------------------------------
    //  定数
    //--------------------------------------------------------------------------
    public EventSystem m_eventSystem;
    private const float c_fNormalScaleMax = 1.2f;
    private const float c_fAnimScaleMax = 1.1f;

    //--------------------------------------------------------------------------
    //  変数
    //--------------------------------------------------------------------------
    private Image m_image;
    private Vector2 m_vNormalSize;
    private float m_fNormalScale;
    private float m_fAnimScale;
    private float m_fAnimScaleTarget;
    private float m_fAnimScaleSpeed;
    private int m_nIdx;

    // Use this for initialization
    private void Start()
    {
        m_image = gameObject.GetComponent<Image>();
        m_vNormalSize = m_image.rectTransform.sizeDelta;
        m_fNormalScale = 1f;
        m_fAnimScale = 1f;
        m_fAnimScaleTarget = c_fAnimScaleMax;
        m_fAnimScaleSpeed = 0.5f;
        m_nIdx = m_image.transform.GetSiblingIndex();
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_eventSystem.currentSelectedGameObject == gameObject)
        {//effect
            m_image.transform.SetSiblingIndex(10);
            m_fNormalScale = Mathf.MoveTowards(m_fNormalScale, c_fNormalScaleMax, 0.1f);
            m_fAnimScale = Mathf.MoveTowards(m_fAnimScale, m_fAnimScaleTarget, m_fAnimScaleSpeed * Time.deltaTime);

            if (m_fAnimScaleTarget == c_fAnimScaleMax && m_fAnimScale == m_fAnimScaleTarget)
            {
                m_fAnimScaleTarget = 1.0f;
                m_fAnimScaleSpeed = 0.25f;
            }
            else if (m_fAnimScaleTarget == 1.0f && m_fAnimScale == m_fAnimScaleTarget)
            {
                m_fAnimScaleTarget = c_fAnimScaleMax;
                m_fAnimScaleSpeed = 0.5f;
            }
        }
        else
        {
            m_image.transform.SetSiblingIndex(m_nIdx);
            m_fAnimScale = Mathf.MoveTowards(m_fAnimScale, 1f, 0.1f);
            m_fNormalScale = Mathf.MoveTowards(m_fNormalScale, 1f, 0.1f);
        }

        m_image.rectTransform.sizeDelta = m_vNormalSize * m_fNormalScale * m_fAnimScale;
    }
}
