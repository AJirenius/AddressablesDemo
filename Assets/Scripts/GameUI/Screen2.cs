using NUUI;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameUI
{
    public class Screen2:AScreen
    {
        public override void Initialize()
        {
            // Find the button by its name (set in UI Builder or UXML)
            Button myButton = Root.Q<Button>("Button1");

            // Register a click callback
            myButton.clicked += OnButtonClick;
            Debug.Log("Screen2 was enabled!");
            base.Initialize();
        }

        private void OnButtonClick()
        {
            Debug.Log("Button2 was clicked!");
            ScreenManager.Instance.AddToQueue("Screen1");
        }

        protected override void OnAnimInStart()
        {
            Debug.Log("OnAnimInStart2");
            base.OnAnimInStart();   
        }
    }
}