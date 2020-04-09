using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class Video : MonoBehaviour
{
    public RawImage rawImage;
    public VideoPlayer videoPlayer;
    public GameObject fadeScreen;

    public float videoDelayTime;
    private bool isStarted = false;
    private bool isLoading = false;
    private Texture texture;
    void Start()
    {
        StartCoroutine(Play());
    }

    private void Update()
    {
        if (isStarted && !videoPlayer.isPlaying)
        {
            LoadNewLevel();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            videoPlayer.Pause();
            LoadNewLevel();
        }
    }

    IEnumerator Play()
    {
        videoPlayer.Prepare();
        WaitForSeconds time = new WaitForSeconds(videoDelayTime);
        while (!videoPlayer.isPrepared)
        {
            yield return time;
            break;
        }
        texture = rawImage.texture;
        rawImage.texture = videoPlayer.texture;
        rawImage.color = Color.white;
        videoPlayer.Play();
        isStarted = true;
    }

    private void LoadNewLevel()
    {
        if (!isLoading)
        {
            StartCoroutine(LoadLevelRoutine());
        }
    }

    private IEnumerator LoadLevelRoutine()
    {

        isLoading = true;
        fadeScreen.SetActive(true);
        fadeScreen.GetComponent<Animator>().Play("Fade");
        yield return new WaitForSeconds(2.0f);

        yield return SceneManager.LoadSceneAsync(ServiceLocator.Get<GameManager>().sceneToLoad);
    }

}
