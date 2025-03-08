using UnityEngine;
using TMPro;
using System.Collections;
public class TextFlashEffect : MonoBehaviour
{
    // Reference to the text component
    private TMP_Text textComponent;

    // Track the displayed value
    private string previousText;

    // Flash effect settings
    public float flashDuration = 0.8f;
    public float flashSpeed = 4.0f;
    public Color flashColor = new Color(1f, 0.9f, 0f);  // Gold color
    private Color originalColor;

    // Variables to control the flash effect
    private Coroutine flashCoroutine;

    void Start()
    {
        // Get the text component
        textComponent = GetComponent<TMP_Text>();
        if (textComponent == null)
        {
            Debug.LogError("TMPTextFlashEffect requires a TextMeshPro Text component!");
            enabled = false;
            return;
        }

        // Save initial values
        originalColor = textComponent.color;
        previousText = textComponent.text;
    }

    void Update()
    {
        // Detect changes in text content
        if (textComponent.text != previousText)
        {
            Flash();
            previousText = textComponent.text;
        }
    }

    // Flash effect method
    public void Flash()
    {
        // Stop any currently running coroutine
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        // Start a new flash coroutine
        flashCoroutine = StartCoroutine(FlashText());
    }

    IEnumerator FlashText()
    {
        float elapsedTime = 0;
        while (elapsedTime < flashDuration)
        {
            // Smoothly transition between original color and flash color
            float t = Mathf.PingPong(elapsedTime * flashSpeed, 1.0f);
            textComponent.color = Color.Lerp(originalColor, flashColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restore the original color
        textComponent.color = originalColor;
        flashCoroutine = null;
    }
}