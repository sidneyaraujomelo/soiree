using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    public static InteractableManager instance;

    public List<InteractableObject> interactableObjects;

    public float presentationDuration;
    public float presentationDelay;

    private void Awake()
    {
        instance = this;
    }

    public void PresentCharacters()
    {
        for (int i = 0; i < interactableObjects.Count; i++)
        {
            InteractableObject interactable = interactableObjects[i];
            interactable.Present(presentationDuration, i*presentationDelay);
        }
    }

    public void HideCharacters()
    {
        for (int i = interactableObjects.Count-1; i >= 0 ; i--)
        {
            InteractableObject interactable = interactableObjects[i];
            interactable.Hide(presentationDuration, i * presentationDelay);
        }
    }
}
