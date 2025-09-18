using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NUUI
{   
    public abstract class AScreen : AView
    {
        public bool useDefaultSfx = true;
        public bool isPopup = false;
        public bool useBackdrop = false;
        public List<APanel> panels;
        
        public void AddPanel(APanel panel)
        {
            if (panels.Contains(panel))
            {
                Debug.LogWarning("Screen: " + ID + " already have panel: " + panel.ID);
                return;
            }
            panels.Add(panel);
        }

        public void RemovePanel(APanel panel)
        {
            if (!panels.Contains(panel))
            {
                Debug.LogWarning("Screen: " + ID + " doesn't have panel: " + panel.ID);
                return;
            }
            panels.Remove(panel);
        }
    }
}