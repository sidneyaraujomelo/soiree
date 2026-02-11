using Lean.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuspectBoard : MonoBehaviour
{
    public GameObject background;
    public GameObject openButton;
    public GameObject notebook;
    public GameObject LeftPage;
    public GameObject RightPage;
    public GameObject EndingScreen;
    public TextMeshProUGUI endingText;

    //Intuition Panel
    public GameObject intuitionButtonPanel;
    public TextMeshProUGUI intuitionCulpritsLabel;

    public GameObject characterThumbnailPrefab;
    public GameObject deadCharacterThumbnailPrefab;

    public Dictionary <string, CharacterThumbnail> characterThumbnails = new Dictionary<string, CharacterThumbnail>();

    private void Awake()
    {
        background.SetActive(false);
        notebook.SetActive(false);
        openButton.SetActive(true);
        EndingScreen.SetActive(false);
    }

    public void SetOpen(bool value)
    {
        background.SetActive(value);
        notebook.SetActive(value);
        openButton.SetActive(!value);
        GameManager.Instance.SetOnBoard(value);
    }

    public void AddDeadCharacterThumbnail(CharacterData character, Sprite characterThumbnail)
    {
        GameObject newCharacterThumbnail = Instantiate(deadCharacterThumbnailPrefab);
        CharacterThumbnail thumbnailComponent = newCharacterThumbnail.GetComponent<CharacterThumbnail>();
        thumbnailComponent.SetCharacterData(character, characterThumbnail);
        newCharacterThumbnail.transform.parent = LeftPage.transform;
        newCharacterThumbnail.transform.SetAsFirstSibling();
        characterThumbnails.Add(character.characterName, thumbnailComponent);
    }
    public void AddCharacterThumbnail(CharacterData character, Sprite characterThumbnail)
    {
        GameObject newCharacterThumbnail = Instantiate(characterThumbnailPrefab);
        CharacterThumbnail thumbnailComponent = newCharacterThumbnail.GetComponent<CharacterThumbnail>();
        thumbnailComponent.SetCharacterData(character, characterThumbnail);
        if (LeftPage.transform.childCount <= 3)
        {
            newCharacterThumbnail.transform.parent = LeftPage.transform;
            newCharacterThumbnail.transform.SetAsLastSibling();
        }
        else
        {
            newCharacterThumbnail.transform.parent = RightPage.transform;
            newCharacterThumbnail.transform.SetSiblingIndex(newCharacterThumbnail.transform.parent.childCount - 2);
        }
        characterThumbnails.Add(character.characterName, thumbnailComponent);
    }
    public void UpdateCharacterThumbnail(CharacterData character)
    {
        CharacterThumbnail thumbnailComponent = characterThumbnails[character.characterName];
        thumbnailComponent.UpdateCharacterData(character);
    }

    void EndingRoutine()
    {
        background.SetActive(true);
        notebook.SetActive(false);
        openButton.SetActive(false);
        EndingScreen.SetActive(true);

    }
    public void WinRoutine()
    {
        EndingRoutine();
        endingText.text = LeanLocalization.GetTranslationText("Main/Board/Acertou");
        Debug.Log("YOU WON!");
    }

    public void LoseRoutine()
    {
        EndingRoutine();
        endingText.text = LeanLocalization.GetTranslationText("Main/Board/Errou");
        Debug.Log("YOU DIED!");
    }

    public void ShowIntuitionResult(List<CharacterData> intuitionCulprits)
    {
        intuitionButtonPanel.gameObject.SetActive(false);
        intuitionCulpritsLabel.gameObject.SetActive(true);
        string base_string = intuitionCulprits.Any(x=>x.genderString == "M") ? 
            LeanLocalization.GetTranslationText("Main/Board/IntuitionResultM") 
            : LeanLocalization.GetTranslationText("Main/Board/IntuitionResultF");
        List<string> intuitionCulpritsNames = intuitionCulprits.Select(x => x.characterName).ToList();
        intuitionCulpritsLabel.text = base_string.Replace("{CHARACTER}", string.Join(LeanLocalization.GetTranslationText("Generic/Ou"), intuitionCulpritsNames));
    }
}
