using UnityEngine;
[RequireComponent (typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{

    [SerializeField]
    AudioSource musicSource;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        musicSource.loop = true;
        StartMusic();
    }


    //Call this when you go in the pause menu i guess
    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void StartMusic()
    {
        musicSource.Play();
    }
}
