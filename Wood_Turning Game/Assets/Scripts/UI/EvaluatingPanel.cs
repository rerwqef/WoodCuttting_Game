using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class EvaluatingPanel : MonoBehaviour
{
    public Image accuracyBarImage;

    public Text accuracyText;

    public Text rewardMoneyText;
    public CanvasGroup[] emojies;
    public GameObject TapToContinue;
 public   int m;
    public void StartEvaluating(float accuracy)
    {
        StartCoroutine(ScheduleAccuracyBarImageFillAmout(accuracy));
    }

    IEnumerator ScheduleAccuracyBarImageFillAmout(float accuracy)
    {        
        float currentAccuracy = 0f;
        float increaseSpeed = 0.3f;
       /* if (accuracy!=0)
        {*/

            while (currentAccuracy <= accuracy)
            {
                currentAccuracy = Mathf.Min(currentAccuracy + increaseSpeed * Time.deltaTime, accuracy + 0.00001f);

                accuracyBarImage.fillAmount = currentAccuracy;
                accuracyText.text = "Accuracy: " + Mathf.RoundToInt(currentAccuracy * 100f).ToString() + "%";

                rewardMoneyText.text = "+ $" + Mathf.RoundToInt(currentAccuracy * 500f).ToString();


                yield return null;
            }
        if (currentAccuracy > 0.3f)
        {
            TapToContinue.SetActive(true);
        }
        if (currentAccuracy > 0.89f)
        {
            m = 3;
        }
     else   if (currentAccuracy >0.49f)
        {
            m = 2;
        }
       else if (currentAccuracy >0.21f)
        {
            m = 1;
        }
        StartCoroutine(FadeInEmoji(m));
       
        //}
       /* else
        {

           
     
          *//*  accuracyBarImage.fillAmount = 0;
            accuracyText.text = "Accuracy: " + 0;

            rewardMoneyText.text = "+ $" + 4;*//*
        }*/
       GameSettings.Instance.SetCollectedCoin(Mathf.RoundToInt(currentAccuracy * 500f));
    }
    IEnumerator FadeInEmoji(int n, float duration = 0.5f)
    {
        Debug.Log("Calling FadeInEmoji");

        for (int i = 0; i < n; i++)
        {
         
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                emojies[i].alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            emojies[i].alpha = 1f; // Ensure alpha is set to 1 at the end
        }
    }
}
