using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JCore;
using UnityEngine;
using UnityEngine.UIElements;

namespace NUUI
{
    public class ScreenManager : SingletonMonoBehaviour<ScreenManager>
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private List<ScreenSO> screenDefinitions;
        
        private readonly Dictionary<string, ScreenSO> _defMap = new();
        private readonly Stack<AView> _stack = new(); // for overlays
        private readonly Queue<string> _queue = new();  // queued screen IDs
        private VisualElement _root;

        void Awake()
        {
            _root = uiDocument.rootVisualElement;
            foreach (var def in screenDefinitions)
                _defMap[def.id] = def;
            
            ShowScreen("Screen1");
        }
        
        
        

        public void ShowScreen(string id)
        {
            var def = _defMap[id];

            // Build view tree on the fly
            var ve = def.uxml.Instantiate();
            _root.Add(ve);

            // Create controller and bind it
            var type = Type.GetType(def.controllerClassName);
            if (type == null) { Debug.LogError($"Controller not found: {def.controllerClassName}"); return; }

            var screen = (AView)Activator.CreateInstance(type);
            screen.Root = ve;
            screen.Initialize();
            _stack.Push(screen);

            screen.Open();
           
        }

    }   
}