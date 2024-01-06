using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Singleton (jedna instancja dla ca³ej gry)
    private static SoundManager _instance;

    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Jeœli nie ma jeszcze instancji, stwórz jedn¹
                GameObject soundManagerObject = new GameObject("SoundManager");
                _instance = soundManagerObject.AddComponent<SoundManager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        // Upewnij siê, ¿e jest tylko jedna instancja
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Metoda do w³¹czania/wyciszania dŸwiêku
    public void ToggleMute(bool mute)
    {
        AudioListener.volume = mute ? 0 : 1;
    }
}
