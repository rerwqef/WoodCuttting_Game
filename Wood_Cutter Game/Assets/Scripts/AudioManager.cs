using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AudioManager : MonoBehaviour
{
    // Singleton instance
    public static AudioManager Instance { get; private set; }

    public AudioSource GameMusic;
    public bool isMuted = false;
    public GameObject musicon, musicoff;
   
 
    public bool needVibration = true;
    
    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // If an instance of AudioManager already exists and it's not this, destroy this object
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // Set the instance to this object and mark it as DontDestroyOnLoad
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Your initialization code here
       // SetPlayerprefabs();
    }
/*    void SetPlayerprefabs()
    {
        if (PlayerPrefs.GetInt("Mute")==null )
        {
            
        }
    }
    void getplayerPrefabs()
    {

    }*/
 public void PlayMusic() 
        {
        GameMusic.Play();
        }

    public void Sound()
    {
        isMuted = !isMuted;
        if (isMuted)
        {
            GameMusic.mute = true;
            musicoff.SetActive(true);
            musicon.SetActive(false);

        }
        else
        {
            GameMusic.mute = false;
            musicoff.SetActive(false);
            musicon.SetActive(true);

        }

    }
    public void Vibration(Image img)
    {
        needVibration = !needVibration;
        if (needVibration) img.color = Color.white; else img.color = Color.gray;

    }

}
