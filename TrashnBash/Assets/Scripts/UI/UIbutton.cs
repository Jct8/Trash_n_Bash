using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIbutton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isButtonPressed;

    public enum ButtonType
    {
        Poison,
        Intimidate,
        Ult,
        Other
    }
    public ButtonType buttonType = ButtonType.Other;

    public void OnPointerDown(PointerEventData eventData)
    {
        isButtonPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isButtonPressed = false;
    }
}
