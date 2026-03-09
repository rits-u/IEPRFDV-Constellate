using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.ComponentModel;


public class UserInterfaceQTE : MonoBehaviour
{
    //[Header("UI Visuals")]
    //[SerializeField] Sprite spriteHit;
    //[SerializeField] Sprite spriteMiss;

    [Header("UI Objects")]
    [SerializeField] Image resultImage;
    [SerializeField] Image pressContainer;

    [Header("UI Visuals")]
    [SerializeField] private List<SpritePair> spritesList = new();

    [System.Serializable]
    private struct SpritePair 
    {
        public string name;
        public Sprite sprite;
      //  public Image container;
    }


    public void HideResultVisual()
    {
        resultImage.sprite = null;
        Color temp = resultImage.color;
        temp.a = 0f;
        resultImage.color = temp;
    }

    //public void SetResultSprite()
    //{
    //    Color temp = resultImage.color;
    //    temp.a = 1f;
    //    resultImage.color = temp;
    //    resultImage.sprite = sprite;
    //}


    public void HideHitResult()
    {
        pressContainer.sprite = null;
        Color temp = pressContainer.color;
        temp.a = 0f;
        pressContainer.color = temp;
    }



    public void ShowHitResult(string name)
    {
        foreach (SpritePair pair in spritesList)
        {
            if(pair.name == name)
            {
                Color temp = pressContainer.color;
                temp.a = 1f;
                pressContainer.color = temp;
                pressContainer.sprite = pair.sprite;
            //    pressContainer.gameObject.SetActive(true);
            } 
        }

    }

    public void ShowHitVisual()
    {

    }
}

