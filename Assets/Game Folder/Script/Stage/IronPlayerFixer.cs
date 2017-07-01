using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronPlayerFixer : MonoBehaviour {
    private bool m_bLeft;
    private bool m_bRight;

    private void Update()
    {
        if(IsLeft() && IsRight())
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private bool IsLeft()
    {
       // Vector3 vCSize = gameObject.GetComponent<BoxCollider>().size;
        Vector3 vPos = transform.position;

        if (Physics.Raycast(vPos, Vector3.left, 0.1f, -1, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        return false;
    }

    private bool IsRight()
    {
      //  Vector3 vCSize = gameObject.GetComponent<BoxCollider>().size;
        Vector3 vPos = transform.position;

        if (Physics.Raycast(vPos, Vector3.right, 0.2f, -1, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        return false;
    }
}
