using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.ComponentModel;
using NaughtyAttributes;


public class UserInterfaceQTE : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float keyOffsetScale = 0.08f;

    [Header("UI Objects")]
    [SerializeField] Image resultImage;
    [SerializeField] Image pressContainer;

    [Header("UI Visuals")]
    [SerializeField] private Image leftKeyImage;
    [SerializeField] private Image rightKeyImage;
    [SerializeField] private List<SpriteData> spritesList = new();

    [System.Serializable]
    public struct SpriteData
    {
        public string name;
        public Sprite sprite;
        public Image container;
    }


    public void HideResultVisual()
    {
        resultImage.sprite = null;
        Color temp = resultImage.color;
        temp.a = 0f;
        resultImage.color = temp;
    }

    public void HideHitResult()
    {
        pressContainer.sprite = null;
        Color temp = pressContainer.color;
        temp.a = 0f;
        pressContainer.color = temp;
    }

    public SpriteData GetSpriteDataByName(string name)
    {
        int index = 0;
        for (int i = 0; i < spritesList.Count; i++)
        {
            if (spritesList[i].name == name) break;
            index++;
        }
        return spritesList[index];
    }

    public void PressFeedback(float value)
    {
        Color dim = new Color32(167, 167, 167, 255);
        if (value < -0.5f)
        {
            leftKeyImage.color = dim;
            RectTransform rect = leftKeyImage.GetComponent<RectTransform>();
            rect.localScale = rect.localScale * (1 - keyOffsetScale);
        }
        else
        {
            rightKeyImage.color = dim;
            RectTransform rect = rightKeyImage.GetComponent<RectTransform>();
            rect.localScale = rect.localScale * (1 - keyOffsetScale);
        }
    }

    public void ResetPressFeedback(float value)
    {
        if(value < -0.5f)
        {
            leftKeyImage.color = Color.white;
            RectTransform rect = leftKeyImage.GetComponent<RectTransform>();
            rect.localScale = rect.localScale / (1 - keyOffsetScale); 
        }
        else
        {
            rightKeyImage.color = Color.white;
            RectTransform rect = rightKeyImage.GetComponent<RectTransform>();
            rect.localScale = rect.localScale / (1 - keyOffsetScale);
        }
    }


    public void ShowFeedbackUI(string name)
    {
        SpriteData spriteData = GetSpriteDataByName(name);
        Color temp = spriteData.container.color;
        temp.a = 1f;
        spriteData.container.color = temp;
        spriteData.container.sprite = spriteData.sprite;
    }
}

