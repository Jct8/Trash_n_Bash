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
    public VideoClip videoToPlay;

    public float videoDelayTime;
    private float holdClickTime = 0.0f;
    public float holdClickTimeMax = 2.5f;

    private bool isStarted = false;
    private bool isLoading = false;
    private Texture texture;
    void Start()
    {
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = videoToPlay;
        StartCoroutine(Play());
    }

    private void Update()
    {
        if (isStarted && !videoPlayer.isPlaying)
        {
            LoadNewLevel();
        }
        if (Input.GetKeyDown(KeyCode.Space) || CheckHoldDownClick())
        {
            videoPlayer.Pause();
            LoadNewLevel();
        }
    }

    public bool CheckHoldDownClick()
    {
        if (Input.GetMouseButton(0))
        {
            holdClickTime += Time.deltaTime;
            if (holdClickTime > holdClickTimeMax)
            {
                holdClickTime = 0.0f;
                return true;

            }
        }
        else
        {
            holdClickTime = 0.0f;
        }
        return false;
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
