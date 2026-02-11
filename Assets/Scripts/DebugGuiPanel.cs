using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGuiPanel : MonoBehaviour
{
    public Font myFont;
    string printedGraph;

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
    void OnGUI()
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.normal.background = MakeTex(Screen.width / 2-200, 600, new Color(0, 0, 0, 0.5f));
        GUILayout.BeginArea(new Rect(Screen.width/2+200, 0, Screen.width / 2, 600));
        GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = 20;
        GUI.skin.font = myFont;
        GUILayout.BeginVertical(guiStyle);
        foreach (CharacterData character in GameManager.Instance.characterDataDict.Values)
        {
            GUILayout.Label($"{character.characterName} - {character.role}");
        }
        if (GUILayout.Button("UpdateGraph"))
        {
            printedGraph = GameManager.Instance.GetIntuitionGraph().GetGraphPrint();
        }
        GUILayout.Label(printedGraph);
        GUILayout.EndVertical();
        GUILayout.EndArea();
#endif
    }
}
