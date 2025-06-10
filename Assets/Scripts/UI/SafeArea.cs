using UnityEngine;

namespace MusicRoom.UI
{
    public class SafeArea : MonoBehaviour
    {
        RectTransform Panel;
        Rect LastSafeArea = new Rect(0, 0, 0, 0);

        void Awake()
        {
            Panel = GetComponent<RectTransform>();
            RefreshSafeAreaSize();
        }

        void Update()
        {
            RefreshSafeAreaSize();
        }

        void RefreshSafeAreaSize()
        {
            Rect safeArea = Screen.safeArea;

            if (safeArea != LastSafeArea)
            {
                LastSafeArea = safeArea;

                Vector2 anchorMin = safeArea.position;
                Vector2 anchorMax = safeArea.position + safeArea.size;
                
                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;

                Panel.anchorMin = anchorMin;
                Panel.anchorMax = anchorMax;
            }
        }
    }
}
