using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using TMPro;

public class UIFader : MonoBehaviour
{
    [SerializeField] public Image image;
    [SerializeField] public TMP_Text tmpUIText;
    [SerializeField] public float fadeDuration = 0.2f;
    [SerializeField] public float showDuration = 3f;

 
    void OnEnable()
    {
        StartCoroutine(FadeRoutine(1));
    }

   

    public void SetAlpha(float alpha){
        Color color = tmpUIText.color;
        color.a = alpha;
        tmpUIText.color = color;

         color = image.color;
        color.a = alpha;
        image.color = color;
    }

   private IEnumerator FadeRoutine(float targetAlpha)
    {
            
    float time = 0f;
    float startAlpha;
        try{
            startAlpha = tmpUIText.color.a; 
        }
        catch{
            yield break;
        }
        

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);

            try{
               if (tmpUIText != null)
                {
                    SetAlpha( newAlpha);
                } 
            }
            catch{
                yield break;
            }

            yield return null;
        }

        try{
            if (tmpUIText != null)
                {
                    SetAlpha( targetAlpha);
                } 
        }
        catch{
            yield break;
        }

        
    }
}