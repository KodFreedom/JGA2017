using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StageSelectEffecter : MonoBehaviour {
    public Sprite[] m_aScreenshot;
    public GameObject[] m_aChild;
    public EventSystem m_eventSystem;
    public Image m_screenshot;
    public Sprite m_noneTexture;

    // Update is called once per frame
    void Update () {
		for(int nCnt = 0;nCnt < m_aChild.Length;nCnt++)
        {
            if(m_eventSystem.currentSelectedGameObject == m_aChild[nCnt])
            {
                m_screenshot.sprite = m_aScreenshot[nCnt];
                break;
            }
            else
            {
                m_screenshot.sprite = m_noneTexture;
            }
        }
	}
}
