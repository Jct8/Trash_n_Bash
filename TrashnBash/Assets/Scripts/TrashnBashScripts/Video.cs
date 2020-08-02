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
    public VideoClip videoClip1;
    public VideoClip videoClip2;
    private VideoClip videoClipToBePlayed;

    public float videoDelayTime;
    private float holdClickTime = 0.0f;
    public float holdClickTimeMax = 2.5f;

    private bool isStarted = false;
    private bool isLoading = false;
    private string videoToBePlayed = "cutscene1final.mp4";
    private string video1 = "cutscene1final.mp4";
    private string video2 = "cutscene2final.mp4";

    private Texture texture;
    void Start()
    {
        if(ServiceLocator.Get<GameManager>().videoNumbertoPlay == 1)
        {
            videoToBePlayed = video1;
            videoClipToBePlayed = videoClip1;
        }
        else if (ServiceLocator.Get<GameManager>().videoNumbertoPlay == 2)
        {
            videoToBePlayed = video2;
            videoClipToBePlayed = videoClip2;
        }

        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.isEditor)
        {
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = videoClipToBePlayed;
            StartCoroutine(Play());
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            StartCoroutine(PlayVideoCoroutine());
        }
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.isEditor)
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

    IEnumerator PlayVideoCoroutine()
    {
        Handheld.PlayFullScreenMovie(videoToBePlayed, Color.white, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.Fill); ;
        yield return new WaitForEndOfFrame();
        Debug.Log("Video playback completed.");
        LoadNewLevel();
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
