using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameButtonController : MonoBehaviour
{
    public GameObject m_buttonL;
    public GameObject m_buttonR;
    private float m_fAnimationDis = 3f;
    private EventSystem m_es;
    private float m_fSizeX;
    private float m_fAniX;
    private bool m_bClicked;

    public void OnClick()
    {
        m_bClicked = true;
    }

    // Use this for initialization
    private void Start ()
    {
        m_es = EventSystem.current;
        m_fSizeX = 300f;
        m_fAniX = 0f;
        m_bClicked = false;
    }

    // Update is called once per frame
    private void Update ()
    {
        Vector3 vPosL = m_buttonL.GetComponent<RectTransform>().localPosition;
        Vector3 vPosR = m_buttonR.GetComponent<RectTransform>().localPosition;
        GameObject obj = m_es.currentSelectedGameObject;
        if(obj)
        {
            RectTransform trans = obj.GetComponent<RectTransform>();
            float fY = vPosL.y;
            float fX = GetButtonSize(obj);

            fY = Mathf.Lerp(fY, trans.localPosition.y, 0.75f);
            vPosL.y = fY;
            vPosR.y = fY;

            m_fSizeX = Mathf.Lerp(m_fSizeX, fX, 0.75f);
        }

        if (!m_bClicked)
        {
            m_fAniX = Mathf.Lerp(m_fAniX, m_fAnimationDis, 0.1f);
            if ((m_fAnimationDis > 0f && m_fAniX >= m_fAnimationDis - 0.1f)
                || (m_fAnimationDis < 0f && m_fAniX <= m_fAnimationDis + 0.1f))
            {
                m_fAnimationDis *= -1f;
            }
        }
        else
        {
            m_fAniX = Mathf.Lerp(m_fAniX, -25f, 0.2f);
        }

        vPosL.x = -m_fSizeX * 0.5f - m_fAniX;
        vPosR.x = m_fSizeX * 0.5f + m_fAniX;

        m_buttonL.GetComponent<RectTransform>().localPosition = vPosL;
        m_buttonR.GetComponent<RectTransform>().localPosition = vPosR;
    }

    private void OnEnable()
    {
        m_bClicked = false;
        Vector3 vPosL = m_buttonL.GetComponent<RectTransform>().localPosition;
        Vector3 vPosR = m_buttonR.GetComponent<RectTransform>().localPosition;
        m_es = EventSystem.current;
        GameObject obj = m_es.currentSelectedGameObject;
        if (obj)
        {
            RectTransform trans = obj.GetComponent<RectTransform>();
            m_fSizeX = GetButtonSize(obj);
            vPosL.x = -m_fSizeX * 0.5f;
            vPosL.y = trans.localPosition.y;
            vPosR.x = m_fSizeX * 0.5f;
            vPosR.y = trans.localPosition.y;
            m_buttonL.GetComponent<RectTransform>().localPosition = vPosL;
            m_buttonR.GetComponent<RectTransform>().localPosition = vPosR;
        }
    }

    private float GetButtonSize(GameObject obj)
    {
        if(obj.name == "Retry")
        {
            return 300f;
        }

        if (obj.name == "Title")
        {
            return 300f;
        }

        if (obj.name == "Stage Select")
        {
            return 560f;
        }

        if (obj.name == "Next Stage")
        {
            return 500f;
        }

        if (obj.name == "Game Over")
        {
            return 500f;
        }

        return 0f;
    }
}
