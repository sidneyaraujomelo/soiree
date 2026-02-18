using DG.Tweening;
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

    bool shouldInteract => !GameManager.Instance.isOnDialogue && !GameManager.Instance.isOnBoard && hasBeenPresented;

    public List<UnityEvent> onClickEvents;

    private Vector3 startPosition;
    private Vector3 hiddenPosition;
    bool hasBeenPresented = false;

    // Start is called before the first frame update
    void Start()
    {
        outline.outlineColor = outlineColor;
    }

    private void Awake()
    {
        startPosition = this.transform.localPosition;
        //Debug.Log(startPosition);
        this.transform.localPosition = new Vector3(-14, startPosition.y, startPosition.z);
        hiddenPosition = this.transform.localPosition;
        //Debug.Log(this.transform.localPosition);
    }

    public void Present(float duration, float delay)
    {
        transform.DOLocalMove(startPosition, duration).SetDelay(delay).SetEase(Ease.Flash).Play().OnComplete(() => { hasBeenPresented = true; });   
    }

    public void Hide(float duration, float delay)
    {
        transform.DOLocalMove(hiddenPosition, duration).SetDelay(delay).SetEase(Ease.Flash).Play();
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
