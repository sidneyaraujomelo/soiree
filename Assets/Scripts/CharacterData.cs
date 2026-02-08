using Doublsb.Dialog;
using Lean.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameGenerationRules;
using Random = UnityEngine.Random;

public class CharacterData : MonoBehaviour
{
    public string characterName;
    public Role role;
    public EmotionalState emotionalState;
    public Alibi alibi;
    public List<string> startMessages;
    public List<string> sameAlibiCharacters;
    public string alibiMessage;
    public List<string> opinionMessages;
    public bool revealedAlibi;
    public bool revealedOpinions;
    string genderString;

    public CharacterData(string characterName, Role role, EmotionalState emotionalState, Alibi alibi)
    {
        this.characterName = characterName;
        this.genderString = (characterName == "Regnum" || characterName == "Solaris") ? "M" : "F";
        this.role = role;
        this.emotionalState = emotionalState;
        this.alibi = alibi;

        revealedAlibi = false;
        revealedOpinions = false;

        this.alibiMessage = "";
    }

    public void GenerateAlibiMessage()
    {
        switch (role)
        {
            case Role.Assassino:
                alibiMessage = GetMurdererAlibi();
                break;
            case Role.Aleatorio:
                alibiMessage = GetAleatorioAlibi();
                break;
            case Role.Acusador:
                alibiMessage = GetAcusadorAlibi();
                break;
        }
    }

    public void GenerateOpinions()
    {
        switch (role)
        {
            case Role.Assassino:
                opinionMessages = GetAssassinoOpinions();
                break;
            case Role.Aleatorio:
                opinionMessages = GetAleatorioOpinions();
                break;
            case Role.Acusador:
                opinionMessages = GetAcusadorOpinions();
                break;
        }
    }

    public override string ToString()
    {
        return string.Format($"{characterName}: {role} {alibi} {emotionalState} {string.Join(',',sameAlibiCharacters)}");
    }

    public override bool Equals(object other)
    {
        CharacterData otherCharacter = other as CharacterData;
        return this.characterName == otherCharacter.characterName && this.role == otherCharacter.role;
    }

    public List<DialogData> GenerateDialogData()
    {
        List<DialogData> dialogTexts = new List<DialogData>();
        /*foreach (var message in this.startMessages)
        {
            dialogTexts.Add(new DialogData(message, this.characterName));
        }*/
        DialogData dialogData = new DialogData($"/color:red/{this.characterName} {LeanLocalization.GetTranslationText("Emotion/BaseText")} {GameGenerationRules.GetEmotionalStateString(this.emotionalState, characterName)}.", this.characterName);
        dialogData.SelectList.Add("Alibi", LeanLocalization.GetTranslationText("Alibi/Question"));
        dialogData.SelectList.Add("Opiniao", LeanLocalization.GetTranslationText("Opinion/Question"));
        dialogData.Callback = () => CheckOption();
        dialogTexts.Add(dialogData);
        return dialogTexts;
    }

    void RevealAlibi()
    {
        Debug.Log("Revealed Alibi");
        this.revealedAlibi = true;
        GameManager.Instance.UpdateUI(this);
    }

    void RevealOpinions()
    {
        this.revealedOpinions = true;
        GameManager.Instance.RegisterRevealedOpinions(this.characterName);
        GameManager.Instance.UpdateUI(this);
    }

    void CheckOption()
    {
        if (GameManager.Instance.dialogManager.Result == "Alibi")
        {
            var dialogTexts = new List<DialogData>();
            dialogTexts.Add(new DialogData(this.alibiMessage, this.characterName));
            GameManager.Instance.dialogManager.Show(dialogTexts);
            RevealAlibi();
        }
        else if (GameManager.Instance.dialogManager.Result == "Opiniao")
        {
            var dialogTexts = new List<DialogData>();
            for (int i = 0; i < this.opinionMessages.Count; i++)
            {
                dialogTexts.Add(new DialogData(opinionMessages[i], this.characterName));
            }
            RevealOpinions();
            GameManager.Instance.dialogManager.Show(dialogTexts);
        }
    }

    string GetGeneralAlibi()
    {
        return $"{LeanLocalization.GetTranslationText("Alibi/BaseText")} {GameGenerationRules.GetAlibiString(this.alibi)}.";
    }

    string GetAlibiCharacters(List<string> characterNames, bool positive = true, bool nominal = true)
    {
        if (positive)
        {
            if (characterNames.Count == 0)
            {
                return LeanLocalization.GetTranslationText($"Alibi/Answer/Alone{this.genderString}");
            }
            else if (characterNames.Count == 1)
            {
                if (nominal)
                { 
                    return $"{LeanLocalization.GetTranslationText("Alibi/Answer/Base")} {characterNames[0]}.";
                }
                else
                {
                    return $"{LeanLocalization.GetTranslationText("Alibi/Answer/ViUmaPessoa")}.";
                }
            }
            else
            {
                string message = "";
                if (nominal)
                {
                    message = LeanLocalization.GetTranslationText("Alibi/Answer/Base");
                    for (int i = 0; i < characterNames.Count; i++)
                    {
                        if (i != characterNames.Count - 1)
                        {
                            message += $"{characterNames[i]}, ";
                        }
                        else
                        {
                            message += $"{LeanLocalization.GetTranslationText("Generic/E")} {characterNames[i]}.";
                        }
                    }
                    return message;
                }
                else
                {
                    return $"{LeanLocalization.GetTranslationText("Alibi/Answer/Base")} {characterNames.Count} {LeanLocalization.GetTranslationText("Alibi/Answer/Pessoas")}.";
                }
                
            }
        }
        else
        {
            if (characterNames.Count == 0)
            {
                return $"{LeanLocalization.GetTranslationText("Alibi/Answer/ViNinguem")}";
            }
            else if (characterNames.Count == 1)
            {
                return $"{LeanLocalization.GetTranslationText("Alibi/Answer/BaseNegative")} {characterNames[0]}.";
            }
            else
            {
                string message = $"{LeanLocalization.GetTranslationText("Alibi/Answer/BaseNegative")} ";
                for (int i = 0; i < characterNames.Count; i++)
                {
                    if (i != characterNames.Count - 1)
                    {
                        message += $"{characterNames[i]}, ";
                    }
                    else
                    {
                        message += $"{LeanLocalization.GetTranslationText("Generic/E")} {characterNames[i]}.";
                    }
                }
                return message;
            }
        }
    }

    string GetMurdererAlibi()
    {
        string message = GetGeneralAlibi();
        //Ao gerar o alibi do assassino, consideramos uma frase positiva ou negativa de forma aleatória.
        //Na frase positiva, o personagem viu os personagens listados em sameAlibiCharacters. Na frase negativa,
        //o personagem não viu algum personagem que de fato estava na mesma sala que ele diz estar.
        bool positive = Random.Range(0, 2) == 0;
        if (positive)
        {
            message += $" {GetAlibiCharacters(sameAlibiCharacters)}";
        }
        else
        {
            List<CharacterData> realAlibiCharacters = GameManager.Instance.GetCharactersWithAlibi(this.alibi)
                .Where(x=>x.characterName!=this.characterName).ToList();
            List<CharacterData> lieAlibiCharacters = realAlibiCharacters.Randomize().Take(Random.Range(1, realAlibiCharacters.Count - 1)).ToList();
            message += $" {GetAlibiCharacters(lieAlibiCharacters.Select(x=>x.characterName).ToList(), false)}";
        }
        return message;
    }

    string GetAcusadorAlibi()
    {
        string message = GetGeneralAlibi();
        //O acusador pode dar duas informações: uma informação (pode ser parcial) sobre os personagens na sala dele e
        //uma informação específica sobre o assassino.
        //Primeiro, determino aleatoriamente se o acusador vai informar sobre os personagens na sala dele.
        bool willInformAboutRoom = Random.Range(0,2) == 0;
        // Em seguida, determino aleatoriamente se a informação será completa ou não.
        bool willInformAboutRoomCompleteInformation = Random.Range(0, 2) == 0;
        if (willInformAboutRoom)
        { 
            // Em seguida, determino se a informação será positiva ou negativa.
            bool positiveInformation = Random.Range(0, 2) == 0;
            // Finalmente, determino se a informação é nominal ou não nominal.
            bool nominalInformation = Random.Range(0, 2) == 0;
            // Agora construo a mensagem a partir dessas decisões.
            List<string> characterNamesToMention = willInformAboutRoomCompleteInformation ?
                sameAlibiCharacters :
                sameAlibiCharacters.Randomize().Take(Random.Range(1, sameAlibiCharacters.Count)).ToList();
            message += $" {GetAlibiCharacters(characterNamesToMention, positiveInformation, nominalInformation)}";
        }
        //Agora adiciono a informação relacionada ao assassino.
        CharacterData murdererData = GameManager.Instance.GetMurdererData();
        //Primeiro, checo se o acusador estava na mesma sala que o assassino. Ou seja, o assassino está mentindo
        //e o acusador tentará desmintir. Verifico também se a informação dada pelo acusador é incompleta, se for,
        //o acusador aponta diretamente que o assassino não estava na sala.
        if (murdererData.alibi == this.alibi)
        {
            if (!willInformAboutRoomCompleteInformation)
            {
                message += $" {LeanLocalization.GetTranslationText("Alibi/Answer/BaseNegative")} {murdererData.characterName}";
            }
        }
        return message;
    }

    string GetAleatorioAlibi()
    {
        string message = GetGeneralAlibi();
        //O aleatorio dá uma informação possivelmente parcial sobre a sala em que está.
        // Em seguida, determino aleatoriamente se a informação será completa ou não.
        bool willInformAboutRoomCompleteInformation = Random.Range(0, 2) == 0;
        // Em seguida, determino se a informação será positiva ou negativa.
        bool positiveInformation = Random.Range(0, 2) == 0;
        // Finalmente, determino se a informação é nominal ou não nominal.
        bool nominalInformation = Random.Range(0, 2) == 0;
        // Agora construo a mensagem a partir dessas decisões.
        List<string> characterNamesToMention = new List<string>();
        if (willInformAboutRoomCompleteInformation)
        {
            characterNamesToMention = positiveInformation ? sameAlibiCharacters :
                GameManager.Instance.GetComplementarListCharacters(sameAlibiCharacters);
        }
        else
        {
            characterNamesToMention = positiveInformation ?
                sameAlibiCharacters.Randomize().Take(Random.Range(1, sameAlibiCharacters.Count)).ToList() :
                GameManager.Instance.GetComplementarListCharacters(
                    sameAlibiCharacters.Randomize().Take(Random.Range(1, sameAlibiCharacters.Count)).ToList());
        }
        if (characterNamesToMention.Count > 0) { 
            characterNamesToMention = characterNamesToMention.Where(x => x != this.characterName).ToList();
        }
        message += $" {GetAlibiCharacters(characterNamesToMention, positiveInformation, nominalInformation)}";
        return message;
    }

    public string GetMortoOpinion(RoleOpinionRelationship roleOpinionRelationship)
    {
        List<Opinion> drawableOpinions = roleOpinionRelationship.drawableOpinion[Role.Morto];
        Opinion selectedOpinion = drawableOpinions[Random.Range(0, drawableOpinions.Count)];
        GameManager.Instance.RegisterOpinion(this.characterName, GameManager.Instance.deadCharacterName, selectedOpinion);
        return GetOpinionDeadString(selectedOpinion, GameManager.Instance.deadCharacterName);
    }

    public List<string> GetGeneralOpinion()
    {
        List<string> opinions = new List<string>();
        RoleOpinionRelationship roleOpinionRelationship = GameGenerationRules.roleOpinionRelationships
            .Where(x => x.role == this.role).First();
        foreach (CharacterData toOpinate in GameManager.Instance.characterDataDict.Values)
        {
            if (roleOpinionRelationship.drawableOpinion.ContainsKey(toOpinate.role) && !this.Equals(toOpinate))
            {
                List<Opinion> drawableOpinions = roleOpinionRelationship.drawableOpinion[toOpinate.role];
                Opinion selectedOpinion = drawableOpinions[Random.Range(0, drawableOpinions.Count)];
                opinions.Add(GetOpinionString(selectedOpinion, toOpinate.characterName));
                GameManager.Instance.RegisterOpinion(this.characterName, toOpinate.characterName, selectedOpinion);
            }
        }
        opinions.Add(GetMortoOpinion(roleOpinionRelationship));
        return opinions;
    }

    public List<string> GetAssassinoOpinions()
    {
        return GetGeneralOpinion();
    }
    public List<string> GetAleatorioOpinions()
    {
        return GetGeneralOpinion();
    }
    public List<string> GetAcusadorOpinions()
    {
        List<string> opinions = new List<string>();
        RoleOpinionRelationship roleOpinionRelationship = GameGenerationRules.roleOpinionRelationships
            .Where(x => x.role == this.role).First();
        bool firstAleatorio = true;
        foreach (CharacterData toOpinate in GameManager.Instance.characterDataDict.Values)
        {
            if (roleOpinionRelationship.drawableOpinion.ContainsKey(toOpinate.role))
            {
                if (toOpinate.role == Role.Aleatorio && firstAleatorio)
                {
                    Opinion selectedOpinion = Opinion.Negativa;
                    opinions.Add(GetOpinionString(selectedOpinion, toOpinate.characterName));
                    GameManager.Instance.RegisterOpinion(this.characterName, toOpinate.characterName, selectedOpinion);
                    firstAleatorio = false;
                }
                else
                {
                    List<Opinion> drawableOpinions = roleOpinionRelationship.drawableOpinion[toOpinate.role];
                    Opinion selectedOpinion = drawableOpinions[Random.Range(0, drawableOpinions.Count)];
                    GameManager.Instance.RegisterOpinion(this.characterName, toOpinate.characterName, selectedOpinion);
                    opinions.Add(GetOpinionString(selectedOpinion, toOpinate.characterName));
                }
            }
        }
        opinions.Add(GetMortoOpinion(roleOpinionRelationship));
        return opinions;
    }

}
