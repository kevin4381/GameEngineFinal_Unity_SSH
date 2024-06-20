using System.Collections;
using UnityEngine;
using TMPro;

public class BlinkingText : MonoBehaviour
{
    public TextMeshProUGUI textToBlink; 
    public float blinkInterval = 0.5f; 

    void Start()
    {
        if (textToBlink == null)
        {
            textToBlink = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (textToBlink != null)
        {
            StartCoroutine(BlinkText());
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component is not assigned and there is no TextMeshProUGUI component attached to this GameObject or its children.");
        }
    }

    IEnumerator BlinkText()
    {
        while (true)
        {
            // 텍스트 투명도 변경
            textToBlink.color = new Color(textToBlink.color.r, textToBlink.color.g, textToBlink.color.b, 0);
            yield return new WaitForSeconds(blinkInterval);
            textToBlink.color = new Color(textToBlink.color.r, textToBlink.color.g, textToBlink.color.b, 1);
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
