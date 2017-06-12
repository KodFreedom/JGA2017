using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectController : MonoBehaviour
{
    public void OnClickStage(int nIndex)
    {
        SceneManager.LoadScene(nIndex);
    }
}
