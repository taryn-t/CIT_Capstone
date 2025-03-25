
using System.Collections;
using TMPro;
using UnityEngine;

public class PopupMessage : MonoBehaviour
{
    [SerializeField] public float fadeDuration = 0.2f;
    [SerializeField] public float showDuration = 3f;
    
    [SerializeField] TMP_Text tmpUIText;

    public string _message = "Default Message";
      public string Message
    {
        get { return _message; }
        set { _message = value; }
    }

    public void ShowMessage(){
        StartCoroutine(ShowMessageRoutine());
    }
    public IEnumerator ShowMessageRoutine(){

        

        yield return StartCoroutine(FadeRoutine(1f));
        
        yield return new WaitForSeconds(fadeDuration + showDuration);

        yield return StartCoroutine(FadeRoutine(0f));
        yield return new WaitForSeconds(fadeDuration );
        Destroy(gameObject);
    }

       public void FadeIn()
    {
        StartCoroutine(FadeRoutine(1f));
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void FadeOut()
    {
        StartCoroutine(FadeRoutine(0f));
    }
    
    public void SetMessage(string message){
        Message = message;
        tmpUIText.text = Message;
    }

    public void SetAlpha(float alpha){
        Color color = tmpUIText.color;
        color.a = alpha;
        tmpUIText.color = color;
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