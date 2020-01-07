using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelReset : MonoBehaviour
{
    private void Start()
    {
        ServiceLocator.Get<LevelManager>().ClearLevel();
    }
}
