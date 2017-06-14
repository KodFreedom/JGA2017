using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour {
    //--------------------------------------------------------------------------
    //  定数
    //--------------------------------------------------------------------------
    public Text m_loadingText;
    public Image m_progressBar;
    public Image m_fadeOverlay;

    public float m_fWaitOnLoadEnd = 0.25f;
    public float m_fFadeDuration = 0.25f;

    public LoadSceneMode m_fLoadSceneMode = LoadSceneMode.Single;
    public ThreadPriority m_fLoadThreadPriority;

    //--------------------------------------------------------------------------
    //  変数
    //--------------------------------------------------------------------------
    AsyncOperation m_operation;
    Scene m_currentScene;

    public static int s_nSceneToLoad = -1;
    // IMPORTANT! This is the build index of your loading scene. You need to change this to match your actual scene index
    static int s_nLoadingSceneIndex = 29;

    public static void LoadScene(int nLevelNum)
    {
        Application.backgroundLoadingPriority = ThreadPriority.High;
        s_nSceneToLoad = nLevelNum;
        SceneManager.LoadScene(s_nLoadingSceneIndex);
    }

    void Start()
    {
        if (s_nSceneToLoad < 0)
            return;

        m_fadeOverlay.gameObject.SetActive(true); // Making sure it's on so that we can crossfade Alpha
        m_currentScene = SceneManager.GetActiveScene();
        StartCoroutine(LoadAsync(s_nSceneToLoad));
    }

    private IEnumerator LoadAsync(int levelNum)
    {
        ShowLoadingVisuals();

        yield return null;

        FadeIn();
        StartOperation(levelNum);

        float lastProgress = 0f;

        // operation does not auto-activate scene, so it's stuck at 0.9
        while (DoneLoading() == false)
        {
            yield return null;

            if (Mathf.Approximately(m_operation.progress, lastProgress) == false)
            {
                m_progressBar.fillAmount = m_operation.progress;
                lastProgress = m_operation.progress;
            }
        }

        ShowCompletionVisuals();

        yield return new WaitForSeconds(m_fWaitOnLoadEnd);

        FadeOut();

        yield return new WaitForSeconds(m_fFadeDuration);

        if (m_fLoadSceneMode == LoadSceneMode.Additive)
            SceneManager.UnloadSceneAsync(m_currentScene.buildIndex);
        else
            m_operation.allowSceneActivation = true;
    }

    private void StartOperation(int levelNum)
    {
        Application.backgroundLoadingPriority = m_fLoadThreadPriority;
        m_operation = SceneManager.LoadSceneAsync(levelNum, m_fLoadSceneMode);


        if (m_fLoadSceneMode == LoadSceneMode.Single)
            m_operation.allowSceneActivation = false;
    }

    private bool DoneLoading()
    {
        return (m_fLoadSceneMode == LoadSceneMode.Additive && m_operation.isDone) || (m_fLoadSceneMode == LoadSceneMode.Single && m_operation.progress >= 0.9f);
    }

    void FadeIn()
    {
        m_fadeOverlay.CrossFadeAlpha(0, m_fFadeDuration, true);
    }

    void FadeOut()
    {
        m_fadeOverlay.CrossFadeAlpha(1, m_fFadeDuration, true);
    }

    void ShowLoadingVisuals()
    {
        m_progressBar.fillAmount = 0f;
        m_loadingText.text = "LOADING...";
    }

    void ShowCompletionVisuals()
    {
        m_progressBar.fillAmount = 1f;
        m_loadingText.text = "LOADING DONE";
    }
}
