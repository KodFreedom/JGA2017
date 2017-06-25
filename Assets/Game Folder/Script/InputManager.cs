using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //--------------------------------------------------------------------------
    //  列挙型
    //--------------------------------------------------------------------------
    public enum EBUTTON
    {
        Jump,
        Pause,
        DPadUp,
        DPadLeft,
        DPadRight,
        DPadDown,
        Horizontal,
        Vertical,
        B_MAX
    }

    //--------------------------------------------------------------------------
    //  構造体
    //--------------------------------------------------------------------------
    struct BUTTON
    {
        public float fValueLast;
        public float fValueNow;
    }

    //--------------------------------------------------------------------------
    //  変数
    //--------------------------------------------------------------------------
    private BUTTON[] m_aButtons;

    //--------------------------------------------------------------------------
    //  関数
    //--------------------------------------------------------------------------
    private void Start()
    {
        m_aButtons = new BUTTON[(int)EBUTTON.B_MAX];
        for (int nCnt = 0;nCnt < (int)EBUTTON.B_MAX;nCnt++)
        {
            m_aButtons[nCnt].fValueLast = 0f;
            m_aButtons[nCnt].fValueNow = 0f;
        }
    }

    private void Update()
    {
        for (int nCnt = 0; nCnt < (int)EBUTTON.B_MAX; nCnt++)
        {
            m_aButtons[nCnt].fValueLast = m_aButtons[nCnt].fValueNow;
        }

        float fHorizontal = Input.GetAxis("Horizontal");
        float fHorizontalDpad = Input.GetAxis("HorizontalDpad");
        float fVertical = Input.GetAxis("Vertical");
        float fVerticalDpad = Input.GetAxis("VerticalDpad");

        m_aButtons[(int)EBUTTON.Jump].fValueNow = Input.GetAxis("Jump");
        m_aButtons[(int)EBUTTON.Pause].fValueNow = Input.GetAxis("Pause");
        m_aButtons[(int)EBUTTON.DPadUp].fValueNow = fVerticalDpad > 0f ? 1f : 0f;
        m_aButtons[(int)EBUTTON.DPadDown].fValueNow = fVerticalDpad < 0f ? 1f : 0f;
        m_aButtons[(int)EBUTTON.DPadLeft].fValueNow = fHorizontalDpad < 0f ? 1f : 0f;
        m_aButtons[(int)EBUTTON.DPadRight].fValueNow = fHorizontalDpad > 0f ? 1f : 0f;
        m_aButtons[(int)EBUTTON.Horizontal].fValueNow = Mathf.Abs(fHorizontal) >= Mathf.Abs(fHorizontalDpad) ? fHorizontal : fHorizontalDpad;
        m_aButtons[(int)EBUTTON.Vertical].fValueNow = Mathf.Abs(fVertical) >= Mathf.Abs(fVerticalDpad) ? fVertical : fVerticalDpad;

        Debug.Log("Last" + m_aButtons[(int)EBUTTON.Pause].fValueLast.ToString());
        Debug.Log("Now" + m_aButtons[(int)EBUTTON.Pause].fValueNow.ToString());
    }

    public bool GetButtonDown(EBUTTON button)
    {
        if(m_aButtons[(int)button].fValueLast == 0f && m_aButtons[(int)button].fValueNow != 0f)
        {
            return true;
        }
        return false;
    }

    public bool GetButtonUp(EBUTTON button)
    {
        if (m_aButtons[(int)button].fValueLast != 0f && m_aButtons[(int)button].fValueNow == 0f)
        {
            return true;
        }
        return false;
    }

    public bool GetButton(EBUTTON button)
    {
        if (m_aButtons[(int)button].fValueNow != 0.0f)
        {
            return true;
        }
        return false;
    }

    public float GetAxis(EBUTTON Axis)
    {
        return m_aButtons[(int)Axis].fValueNow;
    }
}
