using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TrustMarker : MonoBehaviour
{
    public Color neutralColor = Color.blue;
    public Color positiveColor = Color.green;
    public Color negativeColor = Color.red;

    public Dictionary<Color, int> colorMultipliers = new Dictionary<Color, int>();
    public List<Color> colorList => colorMultipliers.Keys.ToList();

    public int currentColor = 0;    

    public Image markerImage;

    // Start is called before the first frame update
    void Start()
    {
        colorMultipliers.Add(neutralColor,1);
        colorMultipliers.Add(positiveColor, 2);
        colorMultipliers.Add(negativeColor, -1);
    }

    void SetMarkerColor(Color color)
    {
        if (markerImage != null)
        {
            markerImage.color = color;
        }
    }
    
    public void NextColor()
    {
        currentColor = (currentColor + 1) % colorList.Count;
        SetMarkerColor(colorList[currentColor]);
    }

    public int GetCurrentMultipler()
    {
        return colorMultipliers[colorList[currentColor]];
    }
}
