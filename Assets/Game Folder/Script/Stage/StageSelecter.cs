using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StageSelecter : MonoBehaviour
{
    //--------------------------------------------------------------------------
    //  定数
    //--------------------------------------------------------------------------
    public Camera m_mainCamera;
    public GameObject m_wall;
    public GameObject m_stage;
    public GameObject m_player;
    public GameObject m_clearPanel;
    public GameObject m_failedPanel;
    public GameObject m_pausePanel;
    public EventSystem m_eventSystem;

    //--------------------------------------------------------------------------
    //  変数
    //--------------------------------------------------------------------------
    private bool m_bClear;
    private bool m_bFailed;
    private InputManager m_Input;
    private GameObject[] m_aObjs;
    private Vector3[] m_aVelocity;
    private bool[] m_aisKinematic;

    public void StageClear()
    {
        if (m_bClear || m_bFailed) { return; }
        m_bClear = true;
        m_wall.GetComponent<WallController>().DisableControl();
        m_stage.GetComponent<StageController>().DisableControl();
        m_mainCamera.GetComponent<CameraController>().SetTargetSmooth(m_player);

        //Show Clear Effect & UI
        m_clearPanel.SetActive(true);
        m_eventSystem.SetSelectedGameObject(m_clearPanel.transform.FindChild("Next Stage").gameObject);

        //Sound
        AkSoundEngine.PostEvent("game_clear", gameObject);
    }

    public void PrepareFailed()
    {
        m_wall.GetComponent<WallController>().DisableControl();
        m_stage.GetComponent<StageController>().DisableControl();
    }

    public void StageFailed()
    {
        if (m_bFailed || m_bClear) { return; }
        m_bFailed = true;
        m_wall.GetComponent<WallController>().DisableControl();
        m_stage.GetComponent<StageController>().DisableControl();
        m_player.GetComponent<PlayerController>().SetStatusGameOver();

        //Show Failed Effect & UI
        m_failedPanel.SetActive(true);
        m_eventSystem.SetSelectedGameObject(m_failedPanel.transform.FindChild("Retry").gameObject);

        //Sound
        AkSoundEngine.PostEvent("game_over", gameObject);

		//BGM
		AkSoundEngine.PostEvent("BGM_play_stop", gameObject);
    }
   
    // Use this for initialization
    private void Start()
    {
        m_clearPanel.SetActive(false);
        m_failedPanel.SetActive(false);
        m_pausePanel.SetActive(false);
        m_bClear = false;
        m_bFailed = false;
        m_Input = GameObject.Find("EventSystem").GetComponent<InputManager>();

        //BGM
        AkSoundEngine.PostEvent("BGM_play_start", gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
        if(!m_bClear && !m_bFailed)
        {
            if (m_Input.GetButtonDown(InputManager.EBUTTON.Pause))
            {
                if (!m_pausePanel.activeInHierarchy)
                {
                    PauseGame();
                }
                else
                {
                    ContinueGame();
                }
            }
        }
    }

    private void PauseGame()
    {
        GameManager.m_bPlay = false;
        SaveObjs();
        m_pausePanel.SetActive(true);
        GameObject selectObj = m_pausePanel.transform.FindChild("Retry").gameObject;
        m_eventSystem.SetSelectedGameObject(selectObj);
        
        //Disable scripts that still work while timescale is set to 0
        AkSoundEngine.PostEvent("sound_pause", gameObject);
        AkSoundEngine.PostEvent("fall_pause", gameObject);
        AkSoundEngine.PostEvent("pause_SE", gameObject);
    }

    private void ContinueGame()
    {
        GameManager.m_bPlay = true;
        LoadObjs();
        m_eventSystem.SetSelectedGameObject(null);
        m_pausePanel.GetComponent<GamePanelController>().DisableSelf();

        //enable the scripts again
        AkSoundEngine.PostEvent("sound_replay", gameObject);
        AkSoundEngine.PostEvent("fall_replay", gameObject);
        AkSoundEngine.PostEvent("pause_SE", gameObject);
    }

    private void SaveObjs()
    {
        m_aObjs = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        m_aVelocity = new Vector3[m_aObjs.Length];
        m_aisKinematic = new bool[m_aObjs.Length];

        for(int nCnt = 0;nCnt < m_aObjs.Length;nCnt++)
        {
            Rigidbody rb = m_aObjs[nCnt].GetComponent<Rigidbody>();
            if(rb)
            {
                m_aVelocity[nCnt] = rb.velocity;
                m_aisKinematic[nCnt] = rb.isKinematic;
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
    }

    private void LoadObjs()
    {
        for (int nCnt = 0; nCnt < m_aObjs.Length; nCnt++)
        {
            if (!m_aObjs[nCnt]) { continue; }
            Rigidbody rb = m_aObjs[nCnt].GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = m_aisKinematic[nCnt];
                rb.velocity = m_aVelocity[nCnt];
            }
        }
    }
}