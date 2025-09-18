using System.Collections;
using System.Collections.Generic;
//using JCore.Tracking;
using UnityEngine;

namespace JCore.UI
{
    public class ScreenManager : SingletonMonoBehaviour<ScreenManager>
    {
        public AScreen[] screens = new AScreen[0];
        public APanel[] panels = new APanel[0];
        public Dictionary<string, AScreen> screensById;
        public Dictionary<string, APanel> panelsById;
        public List<AScreen> queue;
        public List<APanel> openPanels;
        
        //[Tooltip("UI Gameobject with Backdrop script attached. Will be shown behind views that uses backdrop in inspector.")]
        //public Backdrop backdrop;
        
        [Tooltip("Default state sfx for all views that has 'Use Default Sfx' set in inspector. Can be overridden by views own settings.")]
        public StateSfx[] defaultSfx;
        void Awake()
        {
            panelsById = new Dictionary<string, APanel>();
            screensById = new Dictionary<string, AScreen>();
            queue = new List<AScreen>();
            openPanels = new List<APanel>();
            InitializeViews();
        }

        public void StartNewQueue(string id, AViewParams param = null)
        {
            StartNewQueue(screensById[id], param);
        }

        public void StartNewQueue(AScreen newScreen, AViewParams param = null)
        {
            CloseQueue(false);
            AddToQueue(newScreen, param);
        }

        public void AddToQueue(string id, AViewParams param = null)
        {
            AddToQueue(screensById[id], param);
        }

        // ALL additions of screens will in the end be executed in this method
        public void AddToQueue(AScreen newScreen, AViewParams param = null)
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
            if (param != null) newScreen.SetParams(param);
            newScreen.Open();
            OpenPanels(newScreen);

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
            
            if (closePanels)
            {
                foreach (APanel panel in openPanels)
                {
                    panel.Close();
                }
                openPanels.Clear();
            }
            
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
                    OpenPanels(backScreen);
                    //if (backdrop) backdrop.Open(backScreen);
                }
            }
        }

        private void OpenPanels(AScreen screen)
        {
            for (int i = openPanels.Count - 1; i >= 0; i--)
            {
                APanel panel = openPanels[i];
                if (!screen.panels.Contains(panel))
                {
                    openPanels.Remove(panel);
                    panel.Close();
                }
            }
            foreach (APanel panel in screen.panels)
            {
                if (!openPanels.Contains(panel))
                {
                    openPanels.Add(panel);
                    panel.Open();
                }
            }
        }

        private void InitializeViews()
        {
            // Panels before screens as screens might add panels on init
            panels = GetComponentsInChildren<APanel>(true);
            foreach (APanel panel in panels)
            {
                panelsById.Add(panel.name, panel);
                if (panel.useDefaultSfx)
                {
                    foreach (StateSfx sfx in defaultSfx)
                    {
                        if (panel.sfx.Contains(sfx) == false) panel.sfx.Add(sfx);
                    }
                }
                panel.Initialize();
            }

            screens = GetComponentsInChildren<AScreen>(true);
            foreach (AScreen screen in screens)
            {
                screensById.Add(screen.name, screen);
                if (screen.useDefaultSfx)
                {
                    foreach (StateSfx sfx in defaultSfx)
                    {
                        if (screen.sfx.Contains(sfx) == false) screen.sfx.Add(sfx);
                    }
                }
                screen.Initialize();
            }
            
            //if (backdrop) backdrop.Initialize();
        }
    }
}
