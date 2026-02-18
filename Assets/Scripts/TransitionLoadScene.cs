using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace EasyTransition
{

    public class TransitionLoadScene : MonoBehaviour
    {
        public static TransitionLoadScene instance;
        public TransitionSettings transition;
        public float startDelay;

        Dictionary<string, UnityAction> onTransitionEndToScene;
        void Awake()
        {
            instance = this;
            DontDestroyOnLoad(this);
            onTransitionEndToScene = new Dictionary<string, UnityAction>();
            onTransitionEndToScene.Add("MainScene", () => 
            {
                InteractableManager.instance.PresentCharacters();
            } );
            onTransitionEndToScene.Add("IntroScene", () => { Debug.Log("finished intro transition"); });
            onTransitionEndToScene.Add("AssassinatoScene", () => { Debug.Log("finished assassinato transition"); });
        }

        private void Start()
        {
            //TransitionManager.Instance().onTransitionEnd += (() => Debug.Log("finished transition"));
        }

        public void LoadScene(string _sceneName)
        {
            TransitionManager.Instance().onTransitionEnd = null;
            TransitionManager.Instance().onTransitionEnd += onTransitionEndToScene.ContainsKey(_sceneName) ? onTransitionEndToScene[_sceneName] : (() => { Debug.Log("finished default transition"); });
            TransitionManager.Instance().Transition(_sceneName, transition, startDelay);
        }   
    }

}


