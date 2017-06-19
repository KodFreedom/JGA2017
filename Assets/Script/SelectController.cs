using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectController : MonoBehaviour
{
    public void OnClickStage(int nIndex)
    {
        GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeController>().LoadStage(nIndex);
        AkSoundEngine.PostEvent("enter_SE", gameObject);
        //LoadingManager.LoadScene(nIndex);
        //SceneManager.LoadScene(nIndex);
    }

    public void OnSelected()
    {
        AkSoundEngine.PostEvent("select_SE", gameObject);
    }
}
