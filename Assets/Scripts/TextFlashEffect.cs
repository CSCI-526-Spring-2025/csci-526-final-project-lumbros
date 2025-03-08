using UnityEngine;
using TMPro;
using System.Collections;

public class TextFlashEffect : MonoBehaviour
{
    // 引用文本组件
    private TMP_Text textComponent;

    // 跟踪显示的值
    private string previousText;

    // 闪烁效果设置
    public float flashDuration = 0.8f;
    public float flashSpeed = 4.0f;
    public Color flashColor = new Color(1f, 0.9f, 0f);  // 金色
    private Color originalColor;

    // 控制闪烁效果的变量
    private Coroutine flashCoroutine;

    void Start()
    {
        // 获取文本组件
        textComponent = GetComponent<TMP_Text>();
        if (textComponent == null)
        {
            Debug.LogError("TMPTextFlashEffect requires a TextMeshPro Text component!");
            enabled = false;
            return;
        }

        // 保存初始值
        originalColor = textComponent.color;
        previousText = textComponent.text;
    }

    void Update()
    {
        // 检测文本内容变化
        if (textComponent.text != previousText)
        {
            Flash();
            previousText = textComponent.text;
        }
    }

    // 闪烁效果
    public void Flash()
    {
        // 如果已有协程在运行，先停止
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        // 启动新的闪烁协程
        flashCoroutine = StartCoroutine(FlashText());
    }

    IEnumerator FlashText()
    {
        float elapsedTime = 0;

        while (elapsedTime < flashDuration)
        {
            // 在原始颜色和闪烁颜色之间平滑过渡
            float t = Mathf.PingPong(elapsedTime * flashSpeed, 1.0f);
            textComponent.color = Color.Lerp(originalColor, flashColor, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 恢复原始颜色
        textComponent.color = originalColor;
        flashCoroutine = null;
    }
}