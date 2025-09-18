using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NUUI
{
    public class AView
    {
        public string ID;
        public ViewState currentState;

        public VisualElement Root { get; set; }
        protected virtual void OnAnimInStart() { }
        protected virtual void OnAnimInEnd() { }
        protected virtual void OnAnimOutStart() { }
        protected virtual void OnAnimOutEnd() { }
        protected virtual void OnInteractionEnabled() { }
        protected virtual void OnInteractionDisabled() { }
        
        public virtual void Initialize()
        {
            SetState(ViewState.Inactive);
            
            Root.RegisterCallback<TransitionEndEvent>(evt =>
            {
                Debug.Log("some transition finished!");

                if (evt.stylePropertyNames.Contains("transform"))
                {
                    switch (currentState )
                    {
                        case ViewState.AnimatingIn:
                            AnimInDone();
                            break;
                        case ViewState.AnimatingOut:
                            AnimOutDone();
                            break;
                    }
                    Debug.Log("Transform transition finished!");
                }
            });
        }
        
        public void Open()
        {
            SetState(ViewState.AnimatingIn);
        }

        public void Close()
        {
            SetState(ViewState.AnimatingOut);
        }

        
        public void EnableInteraction()
        {
            SetState(ViewState.Active);
        }

        public void DisableInteraction()
        {
            SetState(ViewState.InteractionDisabled);
        }
        
        private void AnimInDone()
        {
            SetState(ViewState.Active);
            OnAnimInEnd();
        }

        private void AnimOutDone()
        {
            SetState(ViewState.Inactive);
            OnAnimOutEnd();
        }
        
        private void SetState(ViewState state)
        {
            if ((state == ViewState.AnimatingIn && currentState == ViewState.Active) ||
                (state == ViewState.AnimatingOut && currentState == ViewState.Inactive) ||
                currentState == state) return;
            currentState = state;
            Debug.Log(ID + " : Set state - " + currentState);
            
            switch (state)
            {
                case ViewState.Inactive:
                    Root.AddToClassList("Disabled");
                    break;

                case ViewState.AnimatingIn:
                    Root.AddToClassList("AnimOut");
                    Root.AddToClassList("no-transition");
                    Root.schedule.Execute(() =>
                    {
                        Root.RemoveFromClassList("no-transition");
                        Root.RemoveFromClassList("AnimOut");
                        Root.AddToClassList("AnimIn");
                    }).StartingIn(50);
                    Root.AddToClassList("Disabled");
                    OnAnimInStart();
                    break;

                case ViewState.Active:
                    //Root.RemoveFromClassList("Disabled");
                    OnInteractionEnabled();
                    break;

                case ViewState.InteractionDisabled:
                    Root.AddToClassList("Disabled");
                    OnInteractionDisabled();
                    break;
                
                case ViewState.AnimatingOut:
                    Root.RemoveFromClassList("no-transition");
                    Root.RemoveFromClassList("AnimIn");
                    Root.AddToClassList("AnimOut");
                    Root.AddToClassList("Disabled");
                    OnInteractionDisabled();
                    OnAnimOutStart();
                    break;
            }
            /*if (stateSfx.ContainsKey(state))
            {
                SoundManager.Instance.PlaySound(stateSfx[state]);
            }*/
        }
    }
    
    [CreateAssetMenu(menuName = "UI/Screen Definition")]
    public class ScreenSO : ScriptableObject
    {
        [Header("View")]
        public string id;
        public VisualTreeAsset uxml;
        public string animStyleName;

        [Header("Controller")]
        public string controllerClassName; 
        // e.g. "InventoryScreen" – must implement IScreen
    }
    
    [Serializable]
    public class ScreenDefinition
    {
        public string ID;
        public string uxmlPath;        // e.g. "UI/MainMenu.uxml"
        public string ControllerClass; 
        // creates the code-behind controller that implements IScreen
    }
    
    
    public enum ViewState { Init, Inactive, AnimatingIn, Active, InteractionDisabled, AnimatingOut }
    
}