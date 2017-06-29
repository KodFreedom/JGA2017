using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class InputManager : MonoBehaviour
{
    //--------------------------------------------------------------------------
    //  列挙型
    //--------------------------------------------------------------------------
    public enum EBUTTON
    {
        Jump,
        Pause,
        Submit,
        Cancel,
        DPadUp,
        DPadLeft,
        DPadRight,
        DPadDown,
        KeyUp,
        KeyLeft,
        KeyRight,
        KeyDown,
        Horizontal,
        Vertical,
        LT,
        RT,
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

    //XInput
    PlayerIndex m_playerIndex;

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

        // Find a PlayerIndex, for a single player game
        // Will find the first controller that is connected ans use it
        for (int nCnt = 0; nCnt < 4; nCnt++)
        {
            PlayerIndex testPlayerIndex = (PlayerIndex)nCnt;
            GamePadState testState = GamePad.GetState(testPlayerIndex);
            if (testState.IsConnected)
            {
                m_playerIndex = testPlayerIndex;
                break;
            }
        }
    }

    private void Update()
    {
        for (int nCnt = 0; nCnt < (int)EBUTTON.B_MAX; nCnt++)
        {
            m_aButtons[nCnt].fValueLast = m_aButtons[nCnt].fValueNow;
        }

        //方向キー
        float fHorizontal = Input.GetAxis("Horizontal");
        float fHorizontalDpad = Input.GetAxis("HorizontalDpad");
        float fHorizontalKeyboard = Input.GetAxis("HorizontalKeyboard");
        float fVertical = Input.GetAxis("Vertical");
        float fVerticalDpad = Input.GetAxis("VerticalDpad");
        float fVerticalKeyboard = Input.GetAxis("VerticalKeyboard");

        fHorizontal = Mathf.Abs(fHorizontal) >= Mathf.Abs(fHorizontalDpad) ? fHorizontal : fHorizontalDpad;
        fHorizontal = Mathf.Abs(fHorizontal) >= Mathf.Abs(fHorizontalKeyboard) ? fHorizontal : fHorizontalKeyboard;
        fVertical = Mathf.Abs(fVertical) >= Mathf.Abs(fVerticalDpad) ? fVertical : fVerticalDpad;
        fVertical = Mathf.Abs(fVertical) >= Mathf.Abs(fVerticalKeyboard) ? fVertical : fVerticalKeyboard;

        //LT,RT
        float fLT = Input.GetAxis("LT");
        float fLTKeyboard = Input.GetAxis("LTKeyboard");
        float fRT = Input.GetAxis("RT");
        float fRTKeyboard = Input.GetAxis("RTKeyboard");

        fLT = Mathf.Abs(fLT) >= Mathf.Abs(fLTKeyboard) ? fLT : fLTKeyboard;
        fRT = Mathf.Abs(fRT) >= Mathf.Abs(fRTKeyboard) ? fRT : fRTKeyboard;

        //データ更新
        m_aButtons[(int)EBUTTON.Jump].fValueNow = Input.GetAxis("Jump");
        m_aButtons[(int)EBUTTON.Pause].fValueNow = Input.GetAxis("Pause");
        m_aButtons[(int)EBUTTON.Submit].fValueNow = Input.GetAxis("Submit");
        m_aButtons[(int)EBUTTON.Cancel].fValueNow = Input.GetAxis("Cancel");
        m_aButtons[(int)EBUTTON.DPadUp].fValueNow = fVerticalDpad > 0f ? 1f : 0f;
        m_aButtons[(int)EBUTTON.DPadDown].fValueNow = fVerticalDpad < 0f ? 1f : 0f;
        m_aButtons[(int)EBUTTON.DPadLeft].fValueNow = fHorizontalDpad < 0f ? 1f : 0f;
        m_aButtons[(int)EBUTTON.DPadRight].fValueNow = fHorizontalDpad > 0f ? 1f : 0f;
        m_aButtons[(int)EBUTTON.KeyUp].fValueNow = fVerticalKeyboard > 0f ? 1f : 0f;
        m_aButtons[(int)EBUTTON.KeyDown].fValueNow = fVerticalKeyboard < 0f ? 1f : 0f;
        m_aButtons[(int)EBUTTON.KeyLeft].fValueNow = fHorizontalKeyboard < 0f ? 1f : 0f;
        m_aButtons[(int)EBUTTON.KeyRight].fValueNow = fHorizontalKeyboard > 0f ? 1f : 0f;
        m_aButtons[(int)EBUTTON.Horizontal].fValueNow = fHorizontal;
        m_aButtons[(int)EBUTTON.Vertical].fValueNow = fVertical;
        m_aButtons[(int)EBUTTON.LT].fValueNow = fLT;
        m_aButtons[(int)EBUTTON.RT].fValueNow = fRT;
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

    public void Vibration(bool bLeft, bool bRight)
    {
        // SetVibration should be sent in a slower rate.
        // Set vibration according to triggers
        GamePad.SetVibration(m_playerIndex, bLeft?1f:0f, bRight?1f:0f);
    }
}
