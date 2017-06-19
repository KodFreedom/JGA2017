﻿using System.Collections;
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
    }
   
    // Use this for initialization
    private void Start()
    {
        Time.timeScale = 1;
        m_clearPanel.SetActive(false);
        m_failedPanel.SetActive(false);
        m_pausePanel.SetActive(false);
        m_bClear = false;
        m_bFailed = false;

        //BGM
        AkSoundEngine.PostEvent("BGM_play_start", gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
        if(!m_bClear && !m_bFailed)
        {
            if (Input.GetKeyDown("joystick button 7") || Input.GetKeyDown(KeyCode.Escape))
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
        Time.timeScale = 0;
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
        Time.timeScale = 1;
        m_eventSystem.SetSelectedGameObject(null);
        m_pausePanel.SetActive(false);
        //enable the scripts again
        AkSoundEngine.PostEvent("sound_replay", gameObject);
        AkSoundEngine.PostEvent("fall_replay", gameObject);
        AkSoundEngine.PostEvent("pause_SE", gameObject);
    }

}