using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MusicRoom.UI
{
    public class SplashScreen : MonoBehaviour
    {   
        [Header("UI References")]
        [SerializeField] private Image logoImage;
        [SerializeField] private Image taglineText;
        [SerializeField] private TMP_Text logoText;
        
        [Header("Animation Settings")]
        [SerializeField] private float fadeInDuration = 1f;
        [SerializeField] private float displayDuration = 2f;
        [SerializeField] private float fadeOutDuration = 1f;

        [Header("Config")]
        [SerializeField] private string mainMenuSceneName = "MainMenuScene";

        private void Start()
        {
            // Start with everything invisible
            logoImage.color = new Color(1, 1, 1, 0);
            taglineText.color = new Color(1, 1, 1, 0);
            if (logoText != null)
                logoText.color = new Color(1, 1, 1, 0);

            // Start the splash screen sequence
            StartCoroutine(PlaySplashSequence());
        }

        private IEnumerator PlaySplashSequence()
        {
            yield return StartCoroutine(FadeImage(logoImage, 0, 1, fadeInDuration));
            
            if (logoText != null)
                yield return StartCoroutine(FadeText(logoText, 0, 1, fadeInDuration));
            
            yield return StartCoroutine(FadeImage(taglineText, 0, 1, fadeInDuration));
            
            yield return new WaitForSeconds(displayDuration);
            
            StartCoroutine(FadeImage(logoImage, 1, 0, fadeOutDuration));
            if (logoText != null)
                StartCoroutine(FadeText(logoText, 1, 0, fadeOutDuration));
            yield return StartCoroutine(FadeImage(taglineText, 1, 0, fadeOutDuration));

            Debug.Log($"Attempting to load scene: {mainMenuSceneName}");
            try 
            { 
                if (UIManager.Instance == null)
                {
                    Debug.LogError("UIManager.Instance is null! Make sure UIManager exists in the scene.");
                    SceneManager.LoadScene(mainMenuSceneName);
                }
                else
                {
                    UIManager.Instance.LoadScene(mainMenuSceneName);
                }
            } 
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load scene: {mainMenuSceneName}. Error: {e.Message}");
                SceneManager.LoadScene(mainMenuSceneName);
            }
        }

        private IEnumerator FadeImage(Image image, float startAlpha, float endAlpha, float duration)
        {
            float elapsedTime = 0;
            Color color = image.color;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                image.color = new Color(color.r, color.g, color.b, newAlpha);
                yield return null;
            }
        }

        private IEnumerator FadeText(TMP_Text text, float startAlpha, float endAlpha, float duration)
        {
            float elapsedTime = 0;
            Color color = text.color;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                text.color = new Color(color.r, color.g, color.b, newAlpha);
                yield return null;
            }
        }
    }
}
