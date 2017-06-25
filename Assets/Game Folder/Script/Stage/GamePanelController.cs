using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePanelController : MonoBehaviour
{
    private bool m_bAlive = false;

    private void Update () {
        if(m_bAlive)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.2f);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.2f);
            if (transform.localScale.magnitude <= 0.01f)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        m_bAlive = true;
        transform.localScale = Vector3.zero;
    }

    public void DisableSelf()
    {
        m_bAlive = false;
    }
}
