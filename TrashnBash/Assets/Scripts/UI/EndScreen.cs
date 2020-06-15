using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public GameObject loseTrashText;
    public GameObject faintedRaccoonText;
    public GameObject winText;

    private void OnDisable()
    {
        winText.SetActive(false);
        faintedRaccoonText.SetActive(false);
        loseTrashText.SetActive(false);
    }

    public void EnableWinText(bool enabled)
    {
        winText.SetActive(enabled);
    }

    public void EnableFaintedRaccoonText(bool enabled)
    {
        faintedRaccoonText.SetActive(enabled);
    }

    public void EnableLoseText(bool enabled)
    {
        loseTrashText.SetActive(enabled);
    }

    public void ReturnButton()
    {
        switch (ServiceLocator.Get<GameManager>()._GameState)
        {
            case GameManager.GameState.GameWin:
                ServiceLocator.Get<GameManager>().SetGameWin();
                break;
            case GameManager.GameState.GameLose:
                ServiceLocator.Get<GameManager>().SetGameOver();
                break;
            default:
                break;
        }
    }
}
