using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCore.UI
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
                Debug.LogWarning("Screen " + gameObject.name + " already have panel " + panel.gameObject.name);
                return;
            }
            panels.Add(panel);
        }

        public void RemovePanel(APanel panel)
        {
            if (!panels.Contains(panel))
            {
                Debug.LogWarning("Screen " + gameObject.name + " doesn't have panel " + panel.gameObject.name);
                return;
            }
            panels.Remove(panel);
        }
    }
}