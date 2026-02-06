using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.X86;
using Random = UnityEngine.Random;

public enum EmotionalState
{
    Resoluto,
    Impaciente,
    Enfurecido,
    Perturbado,
    Cauteloso,
    Curioso,
    Amedrontado,
    Atonito,
    Aliviado
}

public enum Alibi
{
    SalaoPrincipal,
    SalaDeDanca,
    SalaDeMusica,
    Jardim,
    Banheiro,
    Cozinha,
    HallDeEntrada,
    QuartoDeHospede,
    QuartoPrincipal,
    Biblioteca,
    Adega
}

public enum Role
{
    Assassino,
    Acusador,
    Aleatorio,
    Morto
}

public enum Opinion
{
    Positiva,
    Neutra,
    Negativa
}

public static class GameGenerationRules
{
    public static Dictionary<Role, List<EmotionalState>> RoleEmotionGrid = new Dictionary<Role, List<EmotionalState>>()
    {
        {
            Role.Assassino,
            new List<EmotionalState>() 
            {
                EmotionalState.Resoluto,
                EmotionalState.Impaciente,
                EmotionalState.Enfurecido,
                EmotionalState.Perturbado,
                EmotionalState.Cauteloso,
                EmotionalState.Curioso
            } 
        },
        { 
            Role.Acusador,
            new List<EmotionalState>() 
            {
                EmotionalState.Resoluto,
                EmotionalState.Impaciente,
                EmotionalState.Enfurecido,
                EmotionalState.Amedrontado,
                EmotionalState.Atonito,
                EmotionalState.Aliviado
            } 
        },
        {
            Role.Aleatorio,
            new List<EmotionalState>() 
            {
                EmotionalState.Perturbado,
                EmotionalState.Cauteloso,
                EmotionalState.Curioso,
                EmotionalState.Amedrontado,
                EmotionalState.Atonito,
                EmotionalState.Aliviado
            } 
        }
    };

    public static int maxAlibis = 3;
    public static string GetAlibiString(Alibi alibi)
    {
        return LeanLocalization.GetTranslationText("Alibi/" + alibi.ToString());
    }

    public static string GetEmotionalStateToStringM(EmotionalState emotionalState)
    {
        return LeanLocalization.GetTranslationText("Emotion/" + emotionalState.ToString() + "M");
    }
    /*
    static Dictionary<EmotionalState, string> EmotionalStateToStringM = new Dictionary<EmotionalState, string>()
    {
        { EmotionalState.Resoluto,  },
        { EmotionalState.Impaciente, "impaciente" },
        { EmotionalState.Enfurecido, "enfurecido" },
        { EmotionalState.Perturbado, "perturbado" },
        { EmotionalState.Cauteloso, "cauteloso" },
        { EmotionalState.Curioso, "curioso" },
        { EmotionalState.Amedrontado, "amedrontado" },
        { EmotionalState.Atonito, "atônito" },
        { EmotionalState.Aliviado, "aliviado"}
    };*/

    public static string GetEmotionalStateToStringF(EmotionalState emotionalState)
    {
        return LeanLocalization.GetTranslationText("Emotion/" + emotionalState.ToString() + "F");
    }
    /*
    static Dictionary<EmotionalState, string> EmotionalStateToStringF = new Dictionary<EmotionalState, string>()
    {
        { EmotionalState.Resoluto, "resoluta" },
        { EmotionalState.Impaciente, "impaciente" },
        { EmotionalState.Enfurecido, "enfurecida" },
        { EmotionalState.Perturbado, "perturbada" },
        { EmotionalState.Cauteloso, "cautelosa" },
        { EmotionalState.Curioso, "curiosa" },
        { EmotionalState.Amedrontado, "amedrontada" },
        { EmotionalState.Atonito, "atônita" },
        { EmotionalState.Aliviado, "aliviada"}
    };
    */
    public static string GetEmotionalStateString(EmotionalState emotionalState, string characterName)
    {
        if (characterName == "Regnum" || characterName == "Solaris")
        {
            return GetEmotionalStateToStringM(emotionalState);
        }
        else
        {
            return GetEmotionalStateToStringF(emotionalState);
        }
    }

    public class RoleOpinionRelationship
    {
        public Role role;
        public Dictionary<Role, List<Opinion>> drawableOpinion;
    }

    public static List<RoleOpinionRelationship> roleOpinionRelationships = new List<RoleOpinionRelationship>()
    {
        new RoleOpinionRelationship() {
            role = Role.Assassino,
            drawableOpinion = new Dictionary<Role, List<Opinion>>()
            {
                { Role.Acusador, new List<Opinion>() { Opinion.Negativa } },
                { Role.Aleatorio, new List<Opinion>() { Opinion.Positiva, Opinion.Neutra, Opinion.Negativa } },
                { Role.Morto, new List<Opinion>() { Opinion.Positiva,  Opinion.Neutra} }
            }
        },
        new RoleOpinionRelationship() {
            role = Role.Acusador,
            drawableOpinion = new Dictionary<Role, List<Opinion>>()
            {
                { Role.Assassino, new List<Opinion>() { Opinion.Negativa } },
                { Role.Aleatorio, new List<Opinion>() { Opinion.Positiva, Opinion.Neutra, Opinion.Negativa } },
                { Role.Morto, new List<Opinion>() { Opinion.Positiva} }
            }
        },
        new RoleOpinionRelationship() {
            role = Role.Aleatorio,
            drawableOpinion = new Dictionary<Role, List<Opinion>>()
            {
                { Role.Acusador, new List<Opinion>() { Opinion.Positiva, Opinion.Neutra, Opinion.Negativa } },
                { Role.Assassino, new List<Opinion>() { Opinion.Positiva, Opinion.Neutra, Opinion.Negativa } },
                { Role.Aleatorio, new List<Opinion>() { Opinion.Positiva, Opinion.Neutra, Opinion.Negativa } },
                { Role.Morto, new List<Opinion>() { Opinion.Positiva, Opinion.Neutra, Opinion.Negativa } },
            }
        }
    };

    static List<string> positiveOpinionsString = new List<string>()
    {
        "{CHARACTER} é gentil com todos.",
        "Eu confio completamente em {CHARACTER}.",
        "Jamais ouvi falarem mal de {CHARACTER}.",
        "{CHARACTER} jamais faria uma coisa dessas.",
        "Conheço {CHARACTER} desde a infância, não é uma pessoa ruim."
    };

    static List<string> neutralOpinionsString = new List<string>()
    {
        "Não conheço {CHARACTER} pessoalmente.",
        "Não tenho como opinar sobre {CHARACTER}.",
        "{CHARACTER} tem uma vida bastante privada.",
        "{CHARACTER} sempre foi bastante independente.",
        "Eu e {CHARACTER} não fazemos parte do mesmo círculo de amizades."
    };


    static List<string> negativeOpinionsString = new List<string>()
    {
        "{CHARACTER} é uma péssima pessoa.",
        "Não confio nem um pouco em {CHARACTER}.",
        "Só escuto falarem mal de {CHARACTER}.",
        "Não me surpreenderia se {CHARACTER} for culpado.",
        "Não gosto de {CHARACTER} desde a infância."
    };

    static List<string> positiveOpinionsDeadString = new List<string>()
    {
        "{CHARACTER} era gentil com todos.",
        "Eu não acredito que {CHARACTER} se foi.",
        "Quem faria uma coisa dessas com {CHARACTER}.",
        "Conhecia {CHARACTER} desde a infância, não era sua hora."
    };

    static List<string> neutralOpinionsDeadString = new List<string>()
    {
        "Não conheci {CHARACTER} pessoalmente.",
        "Não tenho como opinar sobre {CHARACTER}.",
        "{CHARACTER} tinha uma vida bastante privada.",
        "Eu e {CHARACTER} não fazíamos parte do mesmo círculo de amizades."
    };


    static List<string> negativeOpinionsDeadString = new List<string>()
    {
        "{CHARACTER} era uma péssima pessoa.",
        "Não confiava nem um pouco em {CHARACTER}.",
        "Falavam tão mal de {CHARACTER} que uma hora isso ia acontecer.",
        "Não gosto de {CHARACTER} desde a infância."
    };

    static Dictionary<Opinion, List<string>> opinionStringDict = new Dictionary<Opinion, List<string>>()
    {
        { Opinion.Positiva, positiveOpinionsString },
        { Opinion.Neutra, neutralOpinionsString },
        { Opinion.Negativa, negativeOpinionsString }
    };

    static Dictionary<Opinion, List<string>> opinionDeadStringDict = new Dictionary<Opinion, List<string>>()
    {
        { Opinion.Positiva, positiveOpinionsDeadString },
        { Opinion.Neutra, neutralOpinionsDeadString },
        { Opinion.Negativa, negativeOpinionsDeadString }
    };

    public static string GetOpinionString(Opinion opinionType, string characterName)
    {
        string randomMessage = opinionStringDict[opinionType][Random.Range(0, opinionStringDict[opinionType].Count)];
        randomMessage = randomMessage.Replace("{CHARACTER}", characterName);
        return randomMessage;
    }

    public static string GetOpinionDeadString(Opinion opinionType, string characterName)
    {
        string randomMessage = opinionDeadStringDict[opinionType][Random.Range(0, opinionDeadStringDict[opinionType].Count)];
        randomMessage = randomMessage.Replace("{CHARACTER}", characterName);
        return randomMessage;
    }
}
