using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterThumbnail : MonoBehaviour
{
    public CharacterData characterData;
    public Image characterImage;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI alibiText;
    bool hasSetAlibiText;
    public TextMeshProUGUI opinionText;
    bool hasSetOpinionText;

    public void SetCharacterData(CharacterData characterData, Sprite characterThumbnail)
    {
        this.characterData = characterData;
        this.characterImage.sprite = characterThumbnail;
        this.characterNameText.text = characterData.characterName;
        hasSetAlibiText = false;
        hasSetOpinionText = false;
        if (characterData.role != Role.Morto)
        {
            this.alibiText.text = "???";
            this.opinionText.text = "???\n???\n???";
        }
        else
        {
            this.alibiText.text = $"{LeanLocalization.GetTranslationText("Alibi/DeadText")} {GameGenerationRules.GetAlibiString(characterData.alibi)}.";
        }
    }

    public void UpdateCharacterData(CharacterData characterData)
    {
        if (characterData.revealedAlibi & !hasSetAlibiText)
        {
            this.alibiText.text = characterData.alibiMessage;
        }
        if (characterData.revealedOpinions & !hasSetOpinionText)
        {
            this.opinionText.text = string.Join("\n",characterData.opinionMessages);
        }
    }

    public void Blame()
    {
        GameManager.Instance.Blame(characterData.characterName);
    }
}
