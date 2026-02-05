using System.Collections;
using System.Collections.Generic;
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
        }
        else
        {
            newCharacterThumbnail.transform.parent = RightPage.transform;
        }
        newCharacterThumbnail.transform.SetAsLastSibling();
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
        endingText.text = "ACERTOU, MISERAVI!";
        Debug.Log("YOU WON!");
    }

    public void LoseRoutine()
    {
        EndingRoutine();
        endingText.text = "VOCÊ ERROU!!";
        Debug.Log("YOU DIED!");
    }
}
