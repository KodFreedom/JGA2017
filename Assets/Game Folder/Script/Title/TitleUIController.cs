using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleUIController : MonoBehaviour
{
    public GameObject m_logo;
    public GameObject m_panel;
    private Image m_logoImg;
    private float[] m_fPanelImgA;
    private Image[] m_panelImg;
    private bool m_bEnablePanel;
    private int m_nCntOpening;

	// Use this for initialization
	private void Start ()
    {
        m_nCntOpening = 60;
        m_logoImg = m_logo.GetComponent<Image>();
        m_logoImg.color = new Color(1f, 1f, 1f, 0f);
        m_bEnablePanel = false;
        m_panel.SetActive(false);

        m_panelImg = m_panel.transform.GetComponentsInChildren<Image>();
        m_fPanelImgA = new float[m_panelImg.Length];
        for (int nCnt = 0;nCnt < m_panelImg.Length;nCnt++)
        {
            m_fPanelImgA[nCnt] = 0f;
            Color color = m_panelImg[nCnt].color;
            color.a = m_fPanelImgA[nCnt];
            m_panelImg[nCnt].color = color;
        }
	}
	
	// Update is called once per frame
	private void Update ()
    {
		if(m_nCntOpening > 0)
        {
            m_nCntOpening--;
            return;
        }

        m_logoImg.color = Color.Lerp(m_logoImg.color, Color.white, 0.02f);

        if(m_logoImg.color.gamma.a >= 0.99f && !m_bEnablePanel)
        {
            m_bEnablePanel = true;
            m_panel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(m_panel.transform.GetChild(0).gameObject);
        }

        if(m_bEnablePanel)
        {
            for (int nCnt = 0; nCnt < m_panelImg.Length; nCnt++)
            {
                m_fPanelImgA[nCnt] = Mathf.Lerp(m_fPanelImgA[nCnt], 1f, 0.1f);
                Color color = m_panelImg[nCnt].color;
                color.a = m_fPanelImgA[nCnt];
                m_panelImg[nCnt].color = color;
            }
        }
	}
}
