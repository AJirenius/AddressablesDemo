using UnityEngine;
using UnityEngine.UIElements;

namespace NUUI
{
    [CreateAssetMenu(menuName = "UI/Screen Definition")]
    public class ScreenSO : ScriptableObject
    {
        [Header("View")]
        public string id;
        public VisualTreeAsset uxml;
        public string animStyleName;
        public bool isPopup = false;

        [Header("Controller")]
        public string controllerClassName; 
    }
}