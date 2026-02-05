using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractableObject : MonoBehaviour
{

    SpriteRenderer _spriteRenderer;
    public SpriteRenderer spriteRenderer
    {
        get {
            if (_spriteRenderer == null)
            {
                try
                {
                    _spriteRenderer = GetComponent<SpriteRenderer>();
                }
                catch
                {
                    Debug.LogError("SpriteRenderer component not found on " + gameObject.name);
                    return null;
                }
            }
            return _spriteRenderer;
        }
    }

    Outline _outline;
    private Outline outline
    {
        get
        {
            if (_outline == null)
            {
                try
                {
                    _outline = GetComponent<Outline>();
                }
                catch
                {
                    Debug.LogError("Outline component not found on " + gameObject.name);
                    return null;
                }
            }
            return _outline;
        }
    }
    public Color outlineColor;

    bool shouldInteract => !GameManager.Instance.isOnDialogue && !GameManager.Instance.isOnBoard;

    public List<UnityEvent> onClickEvents;

    // Start is called before the first frame update
    void Start()
    {
        outline.outlineColor = outlineColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        if (spriteRenderer != null && outline != null && shouldInteract)
        {
            //Add Outline with color
            outline.EnableOutline();
        }
    }

    private void OnMouseExit()
    {
        if (spriteRenderer != null && outline != null && shouldInteract)
        {
            //Remove Outline
            outline.DisableOutline();
        }
    }

    private void OnMouseDown()
    {
        if (!shouldInteract)
        {
            return;
        }
        foreach (var unityEvent in onClickEvents)
        {
            unityEvent.Invoke();
            outline.DisableOutline();
        }
    }
}
