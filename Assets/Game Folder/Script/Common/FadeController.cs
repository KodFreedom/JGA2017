using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FadeController : MonoBehaviour
{
    //--------------------------------------------------------------------------
    //  列挙型
    //--------------------------------------------------------------------------
    private enum FADE_STATUS
    {
        NONE,
        FADE_IN,
        FADE_OUT
    }

    //--------------------------------------------------------------------------
    //  定数
    //--------------------------------------------------------------------------
    public Image m_fadeImgL;
    public Image m_fadeImgR;
    public float m_fFadeMoveSpeed = 25.0f;

    //--------------------------------------------------------------------------
    //  変数
    //--------------------------------------------------------------------------
    private int m_nIdxNextScene;
    private bool m_bCanChange;
    private FADE_STATUS m_status;
    private Vector2 m_vCanvuaSize;
	private bool m_bPlayed;
	private int m_nCntChange;
    private bool m_bExit;

    public void ExitApp()
    {
        m_bExit = true;
        LoadStage(0);
    }

    public void LoadStage(int nIndex)
    {
		if(m_bCanChange || m_status != FADE_STATUS.NONE) { return; }

        //ゲームを停止させる
        //Time.timeScale = 0;
        GameManager.m_bPlay = false;

        //Eventsystem停止
        EventSystem es = EventSystem.current;
        if(es)
        {
            //Effect
            GameObject obj = es.currentSelectedGameObject;
            if(obj)
            {
                GameButtonController bc = obj.transform.parent.GetComponent<GameButtonController>();
                if(bc)
                {
                    bc.gameObject.SetActive(true);
                }
            }

            //Stop
            NoMouseEventSystem nmes = es.gameObject.GetComponent<NoMouseEventSystem>();
            if(nmes)
            {
                es.gameObject.GetComponent<NoMouseEventSystem>().DisableSelect();
            }
        }

        //初期化
        m_fadeImgL.gameObject.SetActive(true);
        m_fadeImgR.gameObject.SetActive(true);
        gameObject.SetActive(true);
        m_nIdxNextScene = nIndex;
        m_bCanChange = false;
		m_bPlayed = false;
        m_status = FADE_STATUS.FADE_OUT;
		m_nCntChange = 0;

        //イメージ位置初期化
        m_fadeImgL.rectTransform.anchoredPosition = new Vector2(-m_vCanvuaSize.x * 0.5f - m_fFadeMoveSpeed * 2f, 0f);
        m_fadeImgR.rectTransform.anchoredPosition = new Vector2(m_vCanvuaSize.x * 1f + m_fFadeMoveSpeed * 2f, 0f);
    }

    private void Start()
    {
        GameManager.m_bPlay = true;

        //初期化
        m_bCanChange = false;
        m_fadeImgL.canvas.sortingOrder = 1000;
        m_fadeImgL.gameObject.SetActive(true);
        m_fadeImgR.gameObject.SetActive(true);
        m_status = FADE_STATUS.FADE_IN;

        //Canvasのサイズを計算するm
        Canvas canvas = GetComponentInParent<Canvas>();
        m_vCanvuaSize = canvas.GetComponent<CanvasScaler>().referenceResolution;
        
        //イメージサイズと位置初期化(画像の位置は左下頂点)
        m_fadeImgL.rectTransform.sizeDelta = new Vector2(m_vCanvuaSize.x * 0.5f + m_fFadeMoveSpeed, m_vCanvuaSize.y);
        m_fadeImgR.rectTransform.sizeDelta = new Vector2(m_vCanvuaSize.x * 0.5f + m_fFadeMoveSpeed, m_vCanvuaSize.y);
        m_fadeImgL.rectTransform.anchoredPosition = new Vector2(0f + m_fFadeMoveSpeed * 1f, 0f);
        m_fadeImgR.rectTransform.anchoredPosition = new Vector2(m_vCanvuaSize.x * 0.5f - m_fFadeMoveSpeed * 2f, 0f);
    }

    private void Update()
    {
        if (m_bCanChange)
        {
			if (!m_bPlayed) {
				m_bPlayed = true;
				AkSoundEngine.PostEvent("enter_SE", gameObject);
			}

			if (m_nCntChange > 0)
            {
				m_nCntChange--;
				if (m_nCntChange == 0)
                {
                    if(m_bExit)
                    {
                        Application.Quit();
                        m_bExit = false;
                    }
                    else
                    {
                        Time.timeScale = 1;
                        UnloadScene();
                        SceneManager.LoadScene(m_nIdxNextScene);
                    }
				}
			}
        }

        switch (m_status)
        {
            case FADE_STATUS.NONE:
                return;
            case FADE_STATUS.FADE_IN:
            {
                Vector2 vSpeed = new Vector2(m_fFadeMoveSpeed, 0.0f);
                m_fadeImgL.rectTransform.anchoredPosition -= vSpeed;
                m_fadeImgR.rectTransform.anchoredPosition += vSpeed;

                if (m_fadeImgL.rectTransform.anchoredPosition.x <= -m_vCanvuaSize.x * 0.5f - m_fFadeMoveSpeed
                    || m_fadeImgR.rectTransform.anchoredPosition.x >= m_vCanvuaSize.x * 1.0f + m_fFadeMoveSpeed)
                {
                    m_status = FADE_STATUS.NONE;
                }
                break;
            }
                
            case FADE_STATUS.FADE_OUT:
            {
                Vector2 vSpeed = new Vector2(m_fFadeMoveSpeed, 0.0f);
                m_fadeImgL.rectTransform.anchoredPosition += vSpeed;
                m_fadeImgR.rectTransform.anchoredPosition -= vSpeed;

                if (m_fadeImgL.rectTransform.anchoredPosition.x >= 0.0f - m_fFadeMoveSpeed
                    || m_fadeImgR.rectTransform.anchoredPosition.x <= m_vCanvuaSize.x * 0.5f + m_fFadeMoveSpeed)
                {
                    m_status = FADE_STATUS.NONE;
                    m_bCanChange = true;
					m_nCntChange = 30;
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
        AkSoundEngine.SetRTPCValue("room_height", 0f);
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

