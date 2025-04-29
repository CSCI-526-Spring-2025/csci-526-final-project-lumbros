using UnityEngine;
using UnityEngine.UI;

public class GrayOutByMoney : MonoBehaviour
{
    public int requiredMoney = 30;
    private Image img;
    private Color originalColor;
    private bool isGray = false;

    void Start()
    {
        img = GetComponent<Image>();
        if (img != null)
        {
            originalColor = img.color;
        }
    }

    void Update()
    {
        if (MoneyManager.Instance == null || img == null)
            return;

        if (MoneyManager.Instance.mMoney < requiredMoney)
        {
            if (!isGray)
            {
                img.color = new Color(0.3f, 0.3f, 0.3f, 1f); 
                isGray = true;
            }
        }
        else
        {
            if (isGray)
            {
                img.color = originalColor;
                isGray = false;
            }
        }
    }
}
