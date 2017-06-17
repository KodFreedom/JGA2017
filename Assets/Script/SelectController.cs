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
        //LoadingManager.LoadScene(nIndex);
        //SceneManager.LoadScene(nIndex);
        //BGM
        AkSoundEngine.PostEvent("BGM_play_stop", gameObject);
        AkSoundEngine.PostEvent("BGM_stageselect_stop", gameObject);
        AkSoundEngine.PostEvent("BGM_title_stop", gameObject);
    }

}
