using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string nextScene;
    [SerializeField]
    GameObject transitionLoadScenePrefab;
    public void StartGame()
    {
        if (TransitionLoadScene.instance == null)
        {
            GameObject transitionLoadSceneObj = Instantiate(transitionLoadScenePrefab);
            transitionLoadSceneObj.transform.parent = null;
            transitionLoadSceneObj.GetComponent<TransitionLoadScene>().LoadScene(nextScene);
        }
        else
        {
            TransitionLoadScene.instance.LoadScene(nextScene);
        }
    }
}
