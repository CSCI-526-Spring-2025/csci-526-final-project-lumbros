using UnityEngine;
using TMPro;
using System.Collections;

public class TextFlashEffect : MonoBehaviour
{
    // �����ı����
    private TMP_Text textComponent;

    // ������ʾ��ֵ
    private string previousText;

    // ��˸Ч������
    public float flashDuration = 0.8f;
    public float flashSpeed = 4.0f;
    public Color flashColor = new Color(1f, 0.9f, 0f);  // ��ɫ
    private Color originalColor;

    // ������˸Ч���ı���
    private Coroutine flashCoroutine;

    void Start()
    {
        // ��ȡ�ı����
        textComponent = GetComponent<TMP_Text>();
        if (textComponent == null)
        {
            Debug.LogError("TMPTextFlashEffect requires a TextMeshPro Text component!");
            enabled = false;
            return;
        }

        // �����ʼֵ
        originalColor = textComponent.color;
        previousText = textComponent.text;
    }

    void Update()
    {
        // ����ı����ݱ仯
        if (textComponent.text != previousText)
        {
            Flash();
            previousText = textComponent.text;
        }
    }

    // ��˸Ч��
    public void Flash()
    {
        // �������Э�������У���ֹͣ
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        // �����µ���˸Э��
        flashCoroutine = StartCoroutine(FlashText());
    }

    IEnumerator FlashText()
    {
        float elapsedTime = 0;

        while (elapsedTime < flashDuration)
        {
            // ��ԭʼ��ɫ����˸��ɫ֮��ƽ������
            float t = Mathf.PingPong(elapsedTime * flashSpeed, 1.0f);
            textComponent.color = Color.Lerp(originalColor, flashColor, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // �ָ�ԭʼ��ɫ
        textComponent.color = originalColor;
        flashCoroutine = null;
    }
}