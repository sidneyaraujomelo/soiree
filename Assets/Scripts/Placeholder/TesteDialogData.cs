using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class TesteDialogData : MonoBehaviour
{
    public DialogManager dialogManager;

    public List<string> messages;
    public List<DialogData> data;

    private void Start()
    {
    }

    public void PlayDialogue()
    {
        Debug.Log("chamou");
        var dialogTexts = new List<DialogData>();

        foreach (var message in messages)
        {
            dialogTexts.Add(new DialogData(message, "Corotinho"));
        }

        dialogManager.Show(dialogTexts);
    }
}
