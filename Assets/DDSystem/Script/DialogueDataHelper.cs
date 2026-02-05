using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Doublsb.Dialog;

public class DialogueDataHelper : MonoBehaviour
{
    public string message;
    public string characterName;
    public List<string> selectionTags;
    public List<string> dialogueJson;

    DialogueDataHelper Callback(DialogManager dialogManager)
    {
        if (selectionTags.Count != dialogueJson.Count)
        {
            Debug.LogError("Selection tags count does not match dialogue JSON count.");
            return null;
        }
        int index = selectionTags.IndexOf(dialogManager.Result);
        if (index >= 0 && index < dialogueJson.Count)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(dialogueJson[index]);
            var dialogTexts = JsonUtility.FromJson<DialogueDataHelper>(textAsset.text);
            return dialogTexts;
        }
        else
        {
            Debug.LogError("Selected tag not found in the list.");
            return null;
        }
    }
}
