using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyButton : MonoBehaviour, IPointerExitHandler
{
    public Button button;
    public AudioClip onHighlightedSfx;
    public AudioClip onPressedSfx;

    void Awake()
    {
        this.button = GetComponent<Button>();
    }

    public void OnHighlightedButton()
    {
        AudioManager.instance.PlaySFX(onHighlightedSfx);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        button.OnDeselect(new BaseEventData(EventSystem.current));
    }

    public void OnPressedButton()
    {
        AudioManager.instance.PlaySFX(onPressedSfx);
    }


}
