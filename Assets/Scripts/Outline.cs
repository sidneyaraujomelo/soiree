using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;
    public float outlineWidth;
    public Color outlineColor;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        DisableOutline();
    }
    
    public void EnableOutline()
    {
        _spriteRenderer.material.SetFloat("_OutlineWidth", outlineWidth);
        _spriteRenderer.material.SetColor("_OutlineColor", outlineColor);
    }

    public void DisableOutline()
    {
        _spriteRenderer.material.SetFloat("_OutlineWidth", 0f);
    }
}
