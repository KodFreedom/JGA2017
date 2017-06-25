using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectController : MonoBehaviour
{
    public void OnClickStage(int nIndex)
    {
        GameButtonController bc = gameObject.GetComponentInParent<GameButtonController>();
        if (bc)
        {
            bc.OnClick();
        }
        GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeController>().LoadStage(nIndex);
    }

    public void OnSelected()
    {
        AkSoundEngine.PostEvent("select_SE", gameObject);
    }
}
