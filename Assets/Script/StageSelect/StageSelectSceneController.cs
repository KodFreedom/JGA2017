using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageSelectSceneController : MonoBehaviour {
    public SelectController m_controller;
    public GameObject m_mainPanel;
    public GameObject[] m_subPanels;

    // Use this for initialization
    void Start () {
        AkSoundEngine.PostEvent("BGM_stageselect_start", gameObject);
    }

    private void Update()
    {
        if(Input.GetKeyDown("joystick button 1") && m_controller && m_mainPanel)
        {
            if(m_mainPanel.activeSelf)
            {
                //Title
                m_controller.OnClickStage(0);
            }
            else
            {
                for(int nCnt = 0;nCnt < m_subPanels.Length;nCnt++)
                {
                    if(m_subPanels[nCnt].activeSelf)
                    {
                        m_subPanels[nCnt].SetActive(false);
                    }
                }

                m_mainPanel.SetActive(true);
                EventSystem.current.SetSelectedGameObject(m_mainPanel.transform.GetChild(0).gameObject);
            }
        }
    }
}
