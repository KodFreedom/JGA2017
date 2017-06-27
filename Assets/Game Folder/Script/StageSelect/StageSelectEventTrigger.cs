using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectEventTrigger : MonoBehaviour
{
    public Canvas m_canvas;
    public void OnSelected()
    {
        m_canvas.GetComponent<StageSelectSceneController>().ObjSelectedNow(gameObject);
    }
}
