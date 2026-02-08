using Doublsb.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject dialogManagerObject;
    public DialogManager dialogManager;

    public List<GameObject> characters;
    public List<Sprite> characterThumbnails;
    public Dictionary<string, CharacterData> characterDataDict;
    //Dead Character
    public string deadCharacterName;
    public Sprite deadCharacterThumbnail;

    public SuspectBoard suspectBoard;


    // Game Procedural Generation Values
    [SerializeField]
    List<Alibi> alibis;
    [SerializeField]
    Alibi crimeScene;
    [SerializeField]
    int idMurderer;
    Dictionary<Alibi, List<CharacterData>> alibiCharacterDataDict;

    //Intuition Graph
    IntuitionGraph intuitionGraph;
    [HideInInspector]
    public bool hasIntuitionGraph = false;

    public bool isOnDialogue;
    public bool isOnBoard;
    public GameObject grayScreen;
    
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        dialogManager = dialogManagerObject.GetComponent<DialogManager>();
        characterDataDict = new Dictionary<string, CharacterData>();
        alibiCharacterDataDict = new Dictionary<Alibi, List<CharacterData>>();
        grayScreen.SetActive(false);

        GenerateAlibis();
        GenerateCharacterData();
        CreateUI();
    }

    void GenerateAlibis()
    {
        int maxAlibis = GameGenerationRules.maxAlibis;
        alibis = EnumUtils.TakeRandomFromEnumValues<Alibi>(maxAlibis);
        crimeScene = EnumUtils.GetRandomEnumFromList<Alibi>(alibis);
        alibis.Remove(crimeScene);
        foreach (Alibi alibi in alibis)
        {
            alibiCharacterDataDict[alibi] = new List<CharacterData>();
        }
    }

    public void SetOnDialog(bool value)
    {
        this.isOnDialogue = value;
        grayScreen.SetActive(value);
    }

    public void SetOnBoard(bool value)
    {
        this.isOnBoard = value;
    }

    /*
    Role GetRole()
    {
        Role role = EnumUtils.GetRandomEnumValue<Role>();
        return role;
    }
    */
    EmotionalState GetEmotionalState(Role role)
    {
        EmotionalState emotionalState = EnumUtils.GetRandomEnumFromList(GameGenerationRules.RoleEmotionGrid[role]);
        return emotionalState;
    }

    Alibi GetAlibi()
    {
        Alibi alibi = EnumUtils.GetRandomEnumFromList<Alibi>(alibis);
        return alibi;
    }

    List<Role> GenerateRoleList()
    {
        List<Role> roles = new List<Role>()
        { Role.Assassino, Role.Acusador, Role.Aleatorio, Role.Aleatorio };
        roles = roles.Randomize().ToList();
        idMurderer = roles.IndexOf(Role.Assassino);
        return roles;

    }

    void GenerateCharacterData()
    {
        List<Role> roleList = GenerateRoleList();
        for (int i = 0; i < characters.Count; i++)
        {
            Role role = roleList[i];
            EmotionalState emotionalState = GetEmotionalState(role);
            Alibi alibi = GetAlibi();
            CharacterData characterData = new CharacterData(characters[i].name, role, emotionalState, alibi);
            characterDataDict.Add(characters[i].name, characterData);
            alibiCharacterDataDict[alibi].Add(characterData);
        }
        foreach (CharacterData characterData in characterDataDict.Values)
        {
            if (characterData.role != Role.Assassino)
            { 
                characterData.sameAlibiCharacters = alibiCharacterDataDict[characterData.alibi]
                    .Where(c => c.characterName != characterData.characterName && c.role != Role.Assassino)
                    .Select(c => c.characterName)
                    .ToList();
            }
        }
        //Now we set the Assassino sameAlibiCharacters. First, it must be a lie and not hold
        //with other characters truth.
        CharacterData murdererData = characterDataDict[characters[idMurderer].name];
        System.Random random = new System.Random();
        List<CharacterData> possibleRoomies = alibiCharacterDataDict[murdererData.alibi].Where(c=>c.characterName != murdererData.characterName).ToList();
        //If there are no possible roomies, we add a random amount of roomies
        if (possibleRoomies.Count == 0)
        {
            List<CharacterData> otherCharacters = characterDataDict.Values
                .Where(c => c.characterName != murdererData.characterName)
                .ToList();
            murdererData.sameAlibiCharacters = otherCharacters.Take(UnityEngine.Random.Range(1, otherCharacters.Count - 1))
                .Select(x => x.characterName).ToList();
        }
        else if (possibleRoomies.Count == characterDataDict.Count - 1)
        {
            murdererData.sameAlibiCharacters = possibleRoomies
                .Take(UnityEngine.Random.Range(0, possibleRoomies.Count - 1))
                .Select(x => x.characterName).ToList();
        } else
        {
            //We call random between 0 and 2, if its zero we add an impossibleRoomie to the list
            //Otherwise we remove a possibleRoomie from the list
            int rand = random.Next(0, 2);
            if (rand==0)
            {
                List<CharacterData> impossibleRoomies = characterDataDict.Values
                    .Where(c => c.alibi != murdererData.alibi && c.characterName != murdererData.characterName)
                    .ToList();
                List<CharacterData> toAddImpossibleRoomies = impossibleRoomies
                    .Take(UnityEngine.Random.Range(1, impossibleRoomies.Count - 1))
                    .ToList();
                murdererData.sameAlibiCharacters = possibleRoomies.Select(x=>x.characterName).ToList();
                murdererData.sameAlibiCharacters.AddRange(toAddImpossibleRoomies.Select(x=>x.characterName).ToList());
            }
            else
            {
                int numToRemove = UnityEngine.Random.Range(1, possibleRoomies.Count);
                murdererData.sameAlibiCharacters = possibleRoomies
                    .Randomize().Take(possibleRoomies.Count - numToRemove).Select(x=>x.characterName).ToList();
            }
        }
        intuitionGraph = new IntuitionGraph(characterDataDict.Keys.Append(deadCharacterName).ToList());
        foreach (CharacterData characterData in characterDataDict.Values)
        {
            characterData.GenerateAlibiMessage();
            characterData.GenerateOpinions();
            Debug.Log(characterData.ToString());
        }
        hasIntuitionGraph = true;
    }

    public void RegisterOpinion(string source, string target, Opinion opinion)
    {
        this.intuitionGraph.AddWeight(source, target, GameGenerationRules.opinionValueDict[opinion]);
    }

    internal void RegisterRevealedOpinions(string source)
    {
        this.intuitionGraph.AddEdgesToAllTargets(source);
    }

    public void UpdateTrustCulprit(string characterName, int value)
    {
        intuitionGraph.SetMultiplier(characterName, value);
    }

    public List<string> GetIntuitionCulprits()
    {
        return this.intuitionGraph.GetIntuition(deadCharacterName);
    }

    public void StartDialogueWithCharacter(string characterName)
    {
        CharacterData characterData = characterDataDict[characterName];
        List<DialogData> dialogData = characterData.GenerateDialogData();
        dialogManager.Show(dialogData);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<CharacterData> GetComplementarListCharacters(List<CharacterData> characterDatas)
    {
        List<CharacterData> complement = characterDataDict.Values.Where(x=>!characterDatas.Contains(x)).ToList();
        return complement;
    }

    public List<string> GetComplementarListCharacters(List<string> characterNames)
    {
        List<string> complement = characterDataDict.Keys.Where(x => !characterNames.Contains(x)).ToList();
        return complement;
    }

    public List<CharacterData> GetCharactersWithAlibi(Alibi alibi)
    {
        return alibiCharacterDataDict[alibi];
    }

    public CharacterData GetMurdererData()
    {
        return characterDataDict[characters[idMurderer].name];
    }

    void CreateUI()
    {
        //Create Dead Character Thumbnail
        suspectBoard.AddDeadCharacterThumbnail(new CharacterData(deadCharacterName, Role.Morto, EmotionalState.Curioso, crimeScene), deadCharacterThumbnail);
        //Create Characters Thumbnail
        for (int i = 0; i < characters.Count; i++)
        {
            Sprite characterThumbnail = characterThumbnails[i];
            CharacterData characterData = characterDataDict[characters[i].name];
            suspectBoard.AddCharacterThumbnail(characterData, characterThumbnail);
        }
    }

    public void UpdateUI(CharacterData characterData)
    {
        suspectBoard.UpdateCharacterThumbnail(characterData);
    }

    public void Blame(string blamedCharacter)
    {
        if (blamedCharacter == GetMurdererData().characterName)
        {
            suspectBoard.WinRoutine();
        }
        else
        {
            suspectBoard.LoseRoutine();
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    internal IntuitionGraph GetIntuitionGraph()
    {
        return intuitionGraph;
    }
}
