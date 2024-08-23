using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum States
{
    CuttingWoodShape,
    
    PaintingWoods

}
public class GameManager : MonoBehaviour
{
    public GameObject loadingScreen;  // The Panel or Canvas that contains the loading UI
    public Slider progressBar;
    public GameObject iconParent;
    GameObject[] icons;
    // Start is called before the first frame update
    Wood wood;
//  public  GameObject woodObj;
    Vector3 objectRotation;
    public MoveCutter moveCutter;
    private void Start()
    {
        StartLoadingProcess();
        wood =GameObject.FindAnyObjectByType<Wood>();
        setup();
    }
    void setup()
    {
        
          // Initialize the icons array with the number of children in iconParent
            icons = new GameObject[iconParent.transform.childCount];
      
            // Populate the icons array with the children of iconParent
            for (int i = 0; i <icons.Length; i++)
            {
                icons[i] = iconParent.transform.GetChild(i).gameObject;
            }
        int randomiconIndex=Random.Range(0,icons.Length);
        for (int i = 0; i < icons.Length; i++)
        {
            if (randomiconIndex == i)
            {
                icons[i].SetActive(true);
            }
        
        }
    }
    public void StartTheGame()
    {
       wood.CreateWood();
    }
    public void StopTheGame()
    {

    }
    public void Restart()
    {
        moveCutter.restart();
        wood.CreateWood();
     
    }
    public void Next()
    {
        
    }


    public void StartLoadingProcess()
    {
        StartCoroutine(LoadContent());
        AudioManager.Instance.PlayMusic();
    }

    IEnumerator LoadContent()
    {
        // Show the loading screen
        loadingScreen.SetActive(true);

        // Total loading time (in seconds)
        float totalLoadingTime = 6f;
        float elapsedTime = 0f;

        // Simulate loading over time
        while (elapsedTime < totalLoadingTime)
        {
            elapsedTime += Time.deltaTime; // Accumulate elapsed time
            progressBar.value = Mathf.Clamp01(elapsedTime / totalLoadingTime); // Update progress bar

            yield return null; // Wait for the next frame
        }

        // Ensure the progress bar is fully filled
        progressBar.value = 1f;

        // Optional: Add a brief delay before hiding the loading screen
        yield return new WaitForSeconds(0.5f);

        // Hide the loading screen after loading is complete
        loadingScreen.SetActive(false);

        // Now you can activate your game content or proceed with your game logic
        // Example: Activate game objects, start animations, etc.
    }

}
