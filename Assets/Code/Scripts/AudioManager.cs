using UnityEngine;

namespace Code.Scripts
{
    public class AudioManager : MonoBehaviour
    {
        private AudioSource audioSource;

        protected virtual void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        protected void Play(AudioClip clip, bool looped = false)
        {
            audioSource.loop = looped;
            
            if (!audioSource.isPlaying)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

        protected void Play(AudioClip[] clips, bool looped = false)
        {
            audioSource.loop = looped;
            
            if (!audioSource.isPlaying)
            {
                int randomIndex = UnityEngine.Random.Range(0, clips.Length);
                audioSource.clip = clips[randomIndex];
                audioSource.Play();
            }
        }

        protected void PlayInterrupt(AudioClip clip, bool looped = false)
        {
            audioSource.loop = looped;
            audioSource.clip = clip;
            audioSource.Play();
        }

        protected void PlayInterrupt(AudioClip[] clips, bool looped = false)
        {
            audioSource.loop = looped;
            int randomIndex = UnityEngine.Random.Range(0, clips.Length);
            audioSource.clip = clips[randomIndex];
            audioSource.Play();
        }

        public void Stop()
        {
            audioSource.Stop();
        }
    }
}
