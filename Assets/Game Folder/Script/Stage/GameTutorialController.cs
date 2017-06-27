using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTutorialController : MonoBehaviour
{
    enum STATUS
    {
        START,
        FADE_IN,
        WAIT,
        FADE_OUT,
    }

    public Image m_tutorial;

    private STATUS m_status;
    private bool m_bChecked;
    private InputManager m_Input;
    private Vector2 m_vStartPos = new Vector2(0f, 1024f);
    private Vector2 m_vStopPos = Vector2.zero;
    private int m_nCnt;

    // Use this for initialization
    private void Start ()
    {
        GameManager.m_bPlay = false;
        m_bChecked = false;
        m_nCnt = 60;
        m_status = STATUS.START;
        m_Input = GameObject.Find("EventSystem").GetComponent<InputManager>();
        m_tutorial.rectTransform.anchoredPosition = m_vStartPos;
    }

    // Update is called once per frame
    private void Update ()
    {
        if (m_bChecked) { return; }
        GameManager.m_bPlay = false;
        switch (m_status)
        {
            case STATUS.START:
                m_nCnt--;
                if(m_nCnt == 0)
                {
                    m_status = STATUS.FADE_IN;
                }
                break;
            case STATUS.FADE_IN:
                m_tutorial.rectTransform.anchoredPosition = Vector2.Lerp(m_tutorial.rectTransform.anchoredPosition, m_vStopPos, 0.15f);
                if(Vector2.Distance(m_tutorial.rectTransform.anchoredPosition, m_vStopPos) <= 0.5f)
                {
                    m_status = STATUS.WAIT;
                }
                break;
            case STATUS.WAIT:
                if(m_Input.GetButtonDown(InputManager.EBUTTON.Jump))
                {
                    m_status = STATUS.FADE_OUT;
                }
                break;
            case STATUS.FADE_OUT:
                m_tutorial.rectTransform.anchoredPosition = Vector2.Lerp(m_tutorial.rectTransform.anchoredPosition, m_vStartPos, 0.2f);
                if (Vector2.Distance(m_tutorial.rectTransform.anchoredPosition, m_vStartPos) <= 0.5f)
                {
                    m_bChecked = true;
                    GameManager.m_bPlay = true;
                }
                break;
        }
        

    }
}
