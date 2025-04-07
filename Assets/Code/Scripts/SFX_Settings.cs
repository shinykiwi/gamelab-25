using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SFX_Settings", menuName = "Scriptable Objects/SFX_Settings")]
public class SFX_Settings : ScriptableObject
{

    public AudioClip BouncyWall;
    public AudioClip ProjectileInPortal;
    public AudioClip ProjectileHitPlayer;
    public AudioClip ProjectileHitEnemy;

    //public AudioClip ProjectileOutPortal;//Maybe will not use


    public AudioClip EnemyDie;
    public AudioClip LevelComplete;

    public AudioMixerGroup group;


    public static void PlayAudioClip(AudioClip clip, Vector3 position, AudioMixerGroup group, float volume = 1.0f)
    {
        if (clip == null) return;

        GameObject gameObject = new GameObject("One shot audio");
        gameObject.transform.position = position;
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        audioSource.outputAudioMixerGroup = group;
        audioSource.clip = clip;
        audioSource.spatialBlend = 1f;
        audioSource.volume = volume;
        audioSource.Play();
        Object.Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));

    }


}
