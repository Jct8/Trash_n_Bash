using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    public List<GameObject> starList;
    public AudioClip winMusic;
    public Button returnButton;

    private void Awake()
    {
        ServiceLocator.Get<AudioManager>().musicSource.Stop();
        ServiceLocator.Get<AudioManager>().musicSource.clip = winMusic;
        ServiceLocator.Get<AudioManager>().musicSource.volume = 0.5f;
        ServiceLocator.Get<AudioManager>().musicSource.Play();
        ServiceLocator.Get<AudioManager>().musicSource.loop = false;
        for (int i = 0; i< ServiceLocator.Get<LevelManager>().GetStarRating();i++)
        {
            starList[i].SetActive(true);
        }
        returnButton.onClick.AddListener(ReturnToMainMenu);
        GameObject.Find("EnemyText").GetComponent<Text>().text = "Enemies Defeated: " + ServiceLocator.Get<LevelManager>().enemyDeathCount;
        GameObject.Find("TrashLeft").GetComponent<Text>().text = "Trash Left: " + ServiceLocator.Get<LevelManager>().towerHealth;
        GameObject.Find("PlayerHealthC").GetComponent<Text>().text = "Player Health: " + ServiceLocator.Get<LevelManager>().playerHealth;
    }

    void ReturnToMainMenu()
    {
        ServiceLocator.Get<LevelManager>().ClearLevel();
        SceneManager.LoadScene("MainMenu");
    }
}
