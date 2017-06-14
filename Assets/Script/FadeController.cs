using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public Image m_fadeImg;

    private enum FADE_STATUS
    {
        NONE,
        FADE_IN,
        FADE_OUT
    }

    private int m_nIdxNextScene;
    private bool m_bCanChange;
    private FADE_STATUS m_status;
    private float m_fAlpha;
    private float m_fFadeRate = 1.0f / 60.0f;

    public void LoadStage(int nIndex)
    {
        Time.timeScale = 0;
        m_fadeImg.gameObject.SetActive(true);
        gameObject.SetActive(true);
        m_nIdxNextScene = nIndex;

        if(m_status == FADE_STATUS.FADE_OUT) { return; }
        m_fAlpha = 0.0f;
        m_fadeImg.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        m_bCanChange = false;
        m_status = FADE_STATUS.FADE_OUT;
    }

    private void Start()
    {
        m_bCanChange = false;
        m_fadeImg.gameObject.SetActive(true);
        m_fAlpha = 1.0f;
        m_fadeImg.color = new Color(0.0f, 0.0f, 0.0f, m_fAlpha);
        m_status = FADE_STATUS.FADE_IN;
    }

    private void Update()
    {
        if (m_bCanChange)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(m_nIdxNextScene);
        }

        switch (m_status)
        {
            case FADE_STATUS.NONE:
                return;
            case FADE_STATUS.FADE_IN:
                m_fadeImg.color = new Color(0.0f, 0.0f, 0.0f, m_fAlpha);
                m_fAlpha -= m_fFadeRate;
                if (m_fAlpha <= 0.0f)
                {
                    m_fAlpha = 0.0f;
                    m_status = FADE_STATUS.NONE;
                }
                break;
            case FADE_STATUS.FADE_OUT:
                m_fAlpha += m_fFadeRate;
                if (m_fAlpha >= 1.0f)
                {
                    m_fAlpha = 1.0f;
                    m_bCanChange = true;
                    m_status = FADE_STATUS.NONE;
                }
                m_fadeImg.color = new Color(0.0f, 0.0f, 0.0f, m_fAlpha);
                break;
            default:
                break;
        }
    }
}
