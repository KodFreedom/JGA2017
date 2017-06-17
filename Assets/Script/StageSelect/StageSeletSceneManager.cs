using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageSeletSceneManager : MonoBehaviour
{
    //--------------------------------------------------------------------------
    //  定数
    //--------------------------------------------------------------------------
    public GameObject m_panelNow;
    public GameObject m_panelNext;
    public EventSystem m_eventSystem;

    public void ChangePanel()
    {
        m_panelNow.SetActive(false);
        m_panelNext.SetActive(true);
        m_eventSystem.SetSelectedGameObject(m_panelNext.transform.GetChild(0).gameObject);
    }
}
