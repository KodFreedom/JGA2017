using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    //public Image m_fadeImg;
    public Image m_fadeImgL;
    public Image m_fadeImgR;
    public float m_fFadeMoveSpeed = 25.0f;

    private enum FADE_STATUS
    {
        NONE,
        FADE_IN,
        FADE_OUT
    }

    private int m_nIdxNextScene;
    private bool m_bCanChange;
    private FADE_STATUS m_status;
    //private float m_fAlpha;
    //private float m_fFadeRate = 1.0f / 60.0f;
    

    public void LoadStage(int nIndex)
    {
        Time.timeScale = 0;
        
        //m_fadeImg.gameObject.SetActive(true);
        m_fadeImgL.gameObject.SetActive(true);
        m_fadeImgR.gameObject.SetActive(true);
        gameObject.SetActive(true);
        m_nIdxNextScene = nIndex;

        if(m_status == FADE_STATUS.FADE_OUT) { return; }
        //m_fAlpha = 0.0f;
        //m_fadeImg.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        m_fadeImgL.canvas.sortingOrder = 1000;
        m_fadeImgL.rectTransform.sizeDelta = new Vector2(Screen.width * 2.0f, Screen.height * 3.0f);
        m_fadeImgR.rectTransform.sizeDelta = new Vector2(Screen.width * 2.0f, Screen.height * 3.0f);
        m_fadeImgL.rectTransform.anchoredPosition = new Vector2(-Screen.width * 3.0f, 0.0f);
        m_fadeImgR.rectTransform.anchoredPosition = new Vector2(Screen.width * 3.0f, 0.0f);
        m_bCanChange = false;
        m_status = FADE_STATUS.FADE_OUT;
    }

    private void Start()
    {
        m_bCanChange = false;
        //m_fadeImg.gameObject.SetActive(true);
        //m_fAlpha = 1.0f;
        //m_fadeImg.color = new Color(0.0f, 0.0f, 0.0f, m_fAlpha);
        m_fadeImgL.gameObject.SetActive(true);
        m_fadeImgR.gameObject.SetActive(true);
        m_fadeImgL.canvas.sortingOrder = 1000;
        m_fadeImgL.rectTransform.sizeDelta = new Vector2(Screen.width * 2.0f, Screen.height * 3.0f);
        m_fadeImgR.rectTransform.sizeDelta = new Vector2(Screen.width * 2.0f, Screen.height * 3.0f);
        m_fadeImgL.rectTransform.anchoredPosition = new Vector2(-Screen.width * 0.5f, 0.0f);
        m_fadeImgR.rectTransform.anchoredPosition = new Vector2(Screen.width * 0.5f, 0.0f);
        m_status = FADE_STATUS.FADE_IN;
    }

    private void Update()
    {
        if (m_bCanChange)
        {
            Time.timeScale = 1;
            UnloadScene();
            SceneManager.LoadScene(m_nIdxNextScene);
        }

        switch (m_status)
        {
            case FADE_STATUS.NONE:
                return;
            case FADE_STATUS.FADE_IN:
                //m_fadeImg.color = new Color(0.0f, 0.0f, 0.0f, m_fAlpha);
                //m_fAlpha -= m_fFadeRate;
                //if (m_fAlpha <= 0.0f)
                //{
                //    m_fAlpha = 0.0f;
                //    m_status = FADE_STATUS.NONE;
                //}
            {
                Vector2 vSpeed = new Vector2(m_fFadeMoveSpeed, 0.0f);
                m_fadeImgL.rectTransform.anchoredPosition -= vSpeed;
                m_fadeImgR.rectTransform.anchoredPosition += vSpeed;

                if (m_fadeImgL.rectTransform.anchoredPosition.x <= -Screen.width * 3.0f
                    || m_fadeImgR.rectTransform.anchoredPosition.x >= Screen.width * 3.0f)
                {
                    m_status = FADE_STATUS.NONE;
                }
                break;
            }
                
            case FADE_STATUS.FADE_OUT:
                //m_fAlpha += m_fFadeRate;
                //if (m_fAlpha >= 1.0f)
                //{
                //    m_fAlpha = 1.0f;
                //    m_bCanChange = true;
                //    m_status = FADE_STATUS.NONE;
                //}
                //m_fadeImg.color = new Color(0.0f, 0.0f, 0.0f, m_fAlpha);
            {
                Vector2 vSpeed = new Vector2(m_fFadeMoveSpeed, 0.0f);
                m_fadeImgL.rectTransform.anchoredPosition += vSpeed;
                m_fadeImgR.rectTransform.anchoredPosition -= vSpeed;
                if (m_fadeImgL.rectTransform.anchoredPosition.x >= -Screen.width * 0.5f
                    || m_fadeImgR.rectTransform.anchoredPosition.x <= Screen.width * 0.5f)
                {
                    m_status = FADE_STATUS.NONE;
                    m_bCanChange = true;
                }
                break;
            }

            default:
                break;
        }
    }

    private void UnloadScene()
    {
        //BGM
        AkSoundEngine.PostEvent("BGM_play_stop", gameObject);
        AkSoundEngine.PostEvent("BGM_stageselect_stop", gameObject);
        AkSoundEngine.PostEvent("BGM_title_stop", gameObject);
        Scene sc = SceneManager.GetActiveScene();
        GameObject[] objs = sc.GetRootGameObjects();
        for(int nCnt = 0;nCnt<objs.Length;nCnt++)
        {
           if(objs[nCnt].CompareTag("Stage") || objs[nCnt].CompareTag("WwiseManager"))
           {
                Debug.Log("Destroy it!!!!!");
                GameObject.DestroyImmediate(objs[nCnt]);
           }
        }
    }
}

