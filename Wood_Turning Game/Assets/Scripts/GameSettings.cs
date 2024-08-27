using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{

    // Static instance of GameSettings
    private static GameSettings _instance;
    public bool needVibration;
    private bool IsMusicOn;
    private bool isgamesoundOn;
    public int collcetdPoints;
    public GameObject settingpannel; // The panel to be shown or hidden
    public GameObject TryaginPannel;
    private CanvasGroup canvasGroup; // CanvasGroup component of the panel

    public Text collectedpointsText;

    public Image VibrationBtn;
    public Image MusicBtn;
    public Image SoundBtn;

    public AudioSource musicAudio;
    public AudioSource backgroundSource;
    // Public property to access the singleton instance
    public AudioClip bTncLck;
    public bool canplay;
    public static GameSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameSettings>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("GameSettings");
                    _instance = singletonObject.AddComponent<GameSettings>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        // Get the CanvasGroup component from the settings panel
        canvasGroup = settingpannel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = settingpannel.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f; // Ensure the panel starts as invisible
        settingpannel.SetActive(false); // Start with the panel hidden
    }
    public void Start()
    {
    
        IsMusicOn = GetAndSetValue("Music", true);
        needVibration = GetAndSetValue("Vibration", true);
        isgamesoundOn = GetAndSetValue("Gamesound", true);
        Coin();
        SettingsBtnSet();
    }
  void SettingsBtnSet()
    {
        
        UpdateImageColor(MusicBtn, IsMusicOn); UpdateImageColor(VibrationBtn, needVibration); UpdateImageColor(SoundBtn, isgamesoundOn);
        if (IsMusicOn) musicAudio.mute = false; else musicAudio.mute = true;
        if (isgamesoundOn) backgroundSource.mute = false;else backgroundSource.mute = true;
    }
    void Coin()
    {
        if (!PlayerPrefs.HasKey("CollectedPoints"))
        {
            PlayerPrefs.SetInt("CollectedPoints", collcetdPoints);
        }
        else collcetdPoints= PlayerPrefs.GetInt("CollectedPoints");
        collectedpointsText.text=collcetdPoints.ToString();
    }
   public void SetCollectedCoin(int point)
    {
        collcetdPoints = collcetdPoints + point;
        PlayerPrefs.SetInt("CollectedPoints", collcetdPoints);
    collectedpointsText.text = collcetdPoints.ToString();
    }
  
    public void TSettingPannelOn()
    {
        StartCoroutine(FadeIn(settingpannel));
    }

    public void TSettingPannelOff()
    {
        StartCoroutine(FadeOut(settingpannel));
    }
    public void TryAginpanneloff()
    {
        StartCoroutine(FadeOut(TryaginPannel));
    }
    public void TryaginON()
    {

        StartCoroutine(FadeIn(TryaginPannel));
    }
    private IEnumerator FadeIn(GameObject pannel, float duration = 0.5f)
    {
        pannel.SetActive(true); // Make sure the panel is active
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f; // Ensure alpha is set to 1 at the end
    }


    private IEnumerator FadeOut(GameObject pannel, float duration = 0.5f)
    {
        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f; // Ensure alpha is set to 0 at the end
        pannel.SetActive(false); // Deactivate the panel when fully invisible
    }


    private bool GetAndSetValue(string name, bool defaultValue)
    {
        if (!PlayerPrefs.HasKey(name))
        {
            PlayerPrefs.SetInt(name, defaultValue ? 1 : 0);
        }

        return PlayerPrefs.GetInt(name) == 1;
    }

    public void Vibration()
    {
        needVibration = !needVibration;
        UpdateImageColor(VibrationBtn, needVibration);
        PlayerPrefs.SetInt("Vibration", needVibration ? 1 : 0);
    }

    public void Gamesound()
    {
        isgamesoundOn = !isgamesoundOn;
        if (isgamesoundOn) backgroundSource.mute = false; else backgroundSource.mute = true;
        UpdateImageColor(SoundBtn, isgamesoundOn);
        PlayerPrefs.SetInt("Gamesound", isgamesoundOn ? 1 : 0);
    }

    public void Music()
    {
        IsMusicOn = !IsMusicOn;
        if (IsMusicOn) musicAudio.mute = false; else musicAudio.mute=true;
        UpdateImageColor(MusicBtn, IsMusicOn);
        PlayerPrefs.SetInt("Music", IsMusicOn ? 1 : 0);
    }

    private void UpdateImageColor(Image img, bool state)
    {
        Color newColor = state ? Color.cyan : Color.blue;

        // Set the alpha value based on the state
        newColor.a = state ? 1f : 0.5f;

        // Apply the new color to the image
        img.color = newColor;
    }
    public void PlayUISound()
    {
       backgroundSource.PlayOneShot(bTncLck);
    }
    
}
