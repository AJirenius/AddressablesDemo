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
        
        public Dictionary<string, AScreen> screensById;
        public Dictionary<string, APanel> panelsById;
        
        private readonly Dictionary<string, ScreenSO> _defMap = new();
        
        public List<AScreen> queue;
        public List<APanel> openPanels;

        
        private VisualElement _root;

        void Awake()
        {
            _root = uiDocument.rootVisualElement;
            
            panelsById = new Dictionary<string, APanel>();
            screensById = new Dictionary<string, AScreen>();
            queue = new List<AScreen>();
            openPanels = new List<APanel>();

            foreach (var def in screenDefinitions)
            {
                var type = Type.GetType(def.controllerClassName);
                if (type == null) { Debug.LogError($"Controller not found: {def.controllerClassName}"); return; }

                var scre = def.uxml.Instantiate();
                
                var screenRoot = scre.Q<VisualElement>(className: "screen");
                //var visualElement = def.uxml.Instantiate();
                _root.Add(screenRoot); // First we test to have all in hierarchy but invisible.
                
                screenRoot.styleSheets.Add(scre.styleSheets[0]);
                var screen = (AScreen)Activator.CreateInstance(type);
                screen.Root = screenRoot;
                screen.Initialize();
                screensById[def.id] = screen;
                _defMap[def.id] = def;
            }
        }
        
        public void StartNewQueue(string id)
        {
            StartNewQueue(screensById[id]);
        }

        public void StartNewQueue(AScreen newScreen)
        {
            CloseQueue(false);
            AddToQueue(newScreen);
        }
        
        public void AddToQueue(string id)
        {
            AddToQueue(screensById[id]);
        }

        // ALL additions of screens will in the end be executed in this method
        public void AddToQueue(AScreen newScreen)
        {
            if (queue.Count > 0)
            {
                if (newScreen.isPopup == false)
                {
                    queue[queue.Count - 1].Close();
                }
                else
                {
                    queue[queue.Count - 1].DisableInteraction();
                }
            }
            queue.Add(newScreen);
            newScreen.Open();
            //OpenPanels(newScreen);

            //if (backdrop) backdrop.Open(newScreen);
            //ErrorTracking.Instance.breadcrumbs.AddBreadCrumb(newScreen.name, BreadcrumbType.Navigation, "Open");
        }
        
        public void CloseQueue(bool closePanels = true)
        {
            while (queue.Count > 0)
            {
                queue[queue.Count - 1].Close();
                queue.RemoveAt(queue.Count - 1);
            }
            
            /*if (closePanels)
            {
                foreach (APanel panel in openPanels)
                {
                    panel.Close();
                }
                openPanels.Clear();
            }*/
            
            //if (backdrop) backdrop.Close();
        }
        
        public void Back()
        {
            if (queue.Count > 0)
            {
                bool backFromPopup = queue[queue.Count - 1].isPopup;       
                queue[queue.Count - 1].Close();
                //if (backdrop) backdrop.Close();
                queue.RemoveAt(queue.Count - 1);

                if (queue.Count > 0)
                {
                    AScreen backScreen = queue[queue.Count - 1];
                    if (backFromPopup)
                    {
                        backScreen.EnableInteraction();
                    }
                    else
                    {
                        backScreen.Open();
                    }
                    //ErrorTracking.Instance.breadcrumbs.AddBreadCrumb(backScreen.name, BreadcrumbType.Navigation, "Backed To");
                    //OpenPanels(backScreen);
                    //if (backdrop) backdrop.Open(backScreen);
                }
            }
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
           // _stack.Push(screen);

            screen.Open();
           
        }

    }   
}