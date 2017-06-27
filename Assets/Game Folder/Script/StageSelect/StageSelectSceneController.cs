using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageSelectSceneController : MonoBehaviour {
    public SelectController m_controller;
    public GameObject m_mainPanel;
    public GameObject[] m_subPanels;
    public GameObject m_lastSelectedObj;
    private InputManager m_Input;

    public void ObjSelectedNow(GameObject obj)
    {
        m_lastSelectedObj = obj;
    }

    // Use this for initialization
    private void Start () {
        AkSoundEngine.PostEvent("BGM_stageselect_start", gameObject);
        m_Input = GameObject.Find("EventSystem").GetComponent<InputManager>();
    }

    private void Update()
    {
        if(m_Input.GetButtonDown(InputManager.EBUTTON.Cancel) && m_controller && m_mainPanel)
        {
            if(m_mainPanel.activeSelf)
            {
                //Title
                m_controller.OnClickStage(1);
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
                EventSystem.current.SetSelectedGameObject(m_lastSelectedObj/*m_mainPanel.transform.GetChild(0).gameObject*/);
            }
        }
    }
}
