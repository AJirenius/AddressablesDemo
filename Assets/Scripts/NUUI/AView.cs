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
            SetState(ViewState.Hidden);
            
            Root.RegisterCallback<TransitionEndEvent>(evt =>
            {
                Debug.Log("some transition finished!");

                if (evt.stylePropertyNames.Contains("translate"))
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
            SetState(ViewState.Inactive);
        }
        
        private void AnimInDone()
        {
            SetState(ViewState.Active);
            OnAnimInEnd();
        }

        private void AnimOutDone()
        {
            SetState(ViewState.Hidden);
            OnAnimOutEnd();
        }
        
        void SetPickingModeRecursive(VisualElement element, PickingMode mode)
        {
            element.pickingMode = mode;
            foreach (var child in element.Children())
                SetPickingModeRecursive(child, mode);
        }
        
        private void SetState(ViewState state)
        {
            if ((state == ViewState.AnimatingIn && currentState == ViewState.Active) ||
                (state == ViewState.AnimatingOut && currentState == ViewState.Hidden) ||
                currentState == state) return;
            currentState = state;
            Debug.Log(ID + " : Set state - " + currentState);
            
            switch (state)
            {
                case ViewState.Hidden:
                    Root.AddToClassList("hidden");
                    Root.AddToClassList("no-transition");
                    Root.AddToClassList("animOut");
                    SetPickingModeRecursive(Root, PickingMode.Ignore);
                    break;
                
                case ViewState.Inactive:
                    Root.AddToClassList("disabled");
                    SetPickingModeRecursive(Root, PickingMode.Ignore);
                    break;

                case ViewState.AnimatingIn:
                    Root.schedule.Execute(() =>
                    {
                        Root.RemoveFromClassList("hidden");
                        Root.RemoveFromClassList("no-transition");
                        Root.RemoveFromClassList("animOut");
                        Root.AddToClassList("animIn");
                        SetPickingModeRecursive(Root, PickingMode.Ignore);
                        OnAnimInStart();
                    }).StartingIn(50);
                    
                    break;

                case ViewState.Active:
                    SetPickingModeRecursive(Root, PickingMode.Position);    
                    OnInteractionEnabled();
                    break;
                
                case ViewState.AnimatingOut:
                    Root.RemoveFromClassList("animIn");
                    Root.AddToClassList("animOut");
                    SetPickingModeRecursive(Root, PickingMode.Ignore);
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
 
    public enum ViewState { Init, 
        Hidden, // totally hidden from player. Normally at start or after animating out.
        Inactive, // Visible but not interactable. eg. under another screen.
        AnimatingIn, 
        Active, // when animIn finished and all is interactable.
        AnimatingOut }
    
}