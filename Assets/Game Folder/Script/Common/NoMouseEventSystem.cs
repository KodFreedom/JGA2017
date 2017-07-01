using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NoMouseEventSystem : MonoBehaviour
{
    private GameObject m_lastSelect;

    public void DisableSelect()
    {
        m_lastSelect = null;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SelectObj(GameObject obj)
    {
        m_lastSelect = obj;
        EventSystem.current.SetSelectedGameObject(obj);
    }

	// Use this for initialization
	void Start ()
    {
        m_lastSelect = EventSystem.current.currentSelectedGameObject;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(m_lastSelect);
        }

        m_lastSelect = EventSystem.current.currentSelectedGameObject;
    }
}
