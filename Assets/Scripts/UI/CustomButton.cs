using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MusicRoom.UI
{
    [RequireComponent(typeof(Button))]
    public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Hover Settings")]
        [SerializeField] private float hoverScale = 1.05f;
        [SerializeField] private float brightnessIncrease = 0.2f;
        [SerializeField] private float animationSpeed = 10f;

        private Vector3 _originalScale;
        private Vector3 _targetScale;
        private Image _buttonImage;
        private Color _originalColor;
        private Color _targetColor;

        private void Start()
        {
            _originalScale = transform.localScale;
            _targetScale = _originalScale;
            
            _buttonImage = GetComponent<Image>();
            if (_buttonImage != null)
            {
                _originalColor = _buttonImage.color;
                _targetColor = _originalColor;
            }

            // Ensure the button's transition is set to none
            var button = GetComponent<Button>();
            if (button != null)
            {
                button.transition = Selectable.Transition.None;
            }
        }

        private void Update()
        {
            // Smooth scale animation
            transform.localScale = Vector3.Lerp(
                transform.localScale, 
                _targetScale, 
                Time.deltaTime * animationSpeed
            );

            // Smooth color animation
            if (_buttonImage != null)
            {
                _buttonImage.color = Color.Lerp(
                    _buttonImage.color,
                    _targetColor,
                    Time.deltaTime * animationSpeed
                );
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Debug.Log($"Hover Enter: {gameObject.name}"); // Debug log
            
            // Scale up
            _targetScale = _originalScale * hoverScale;

            // Brighten
            if (_buttonImage != null)
            {
                float h, s, v;
                Color.RGBToHSV(_originalColor, out h, out s, out v);
                v = Mathf.Clamp01(v + brightnessIncrease);
                _targetColor = Color.HSVToRGB(h, s, v);
                _targetColor.a = _originalColor.a; // Preserve original alpha
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Debug.Log($"Hover Exit: {gameObject.name}"); // Debug log
            
            // Return to original scale and color
            _targetScale = _originalScale;
            _targetColor = _originalColor;
        }
    }
}
