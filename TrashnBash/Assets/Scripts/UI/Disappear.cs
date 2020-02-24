using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Disappear : MonoBehaviour
{
    public Button _Button;
    // Start is called before the first frame update
    void Start()
    {
        _Button.onClick.AddListener(DisableButton);
    }

    void DisableButton()
    {
        gameObject.SetActive(false);
    }
}
