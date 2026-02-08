using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    GameManager gameManager
    {
        get { return (GameManager)target; }
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        IntuitionGraph graph = gameManager.GetIntuitionGraph();
        if (GUILayout.Button("GetNeutralIntuition"))
        {
            if (gameManager.hasIntuitionGraph)
            {
                Debug.Log(string.Join(',', graph.GetNeutralIntuition(gameManager.deadCharacterName)));
            }
        }
        if (GUILayout.Button("GetIntuition"))
        {
            if (gameManager.hasIntuitionGraph)
            {
                Debug.Log(string.Join(',', gameManager.GetIntuitionCulprits()));
            }
        }
        if (GUILayout.Button("Print Intuition Matrix"))
        {
            if (gameManager.hasIntuitionGraph)
            {
                graph.PrintGraph();
            }
        }
    }
}
