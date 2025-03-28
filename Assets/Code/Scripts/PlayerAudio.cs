using Unity.VisualScripting;
using UnityEngine;

namespace Code.Scripts
{
    public class PlayerAudio : AudioManager
    {
        [SerializeField] private AudioClip[] walking;

        public void PlayFootsteps()
        {
            Play(walking);
        }

        protected override void Start()
        {
            base.Start();
            
            PlayFootsteps();
        }
    }
}
