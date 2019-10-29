using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Slider loadingBar;
    public Text loadingStep;

    public void UpdateLoadingBar(float percent)
    {
        loadingBar.value = percent;
    }

    public void UpdateLoadingStep(string message)
    {
        loadingStep.text = message;
    }
}
