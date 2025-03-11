using System;
using Code.Scripts;
using UnityEngine;

public class EnemyAudio : AudioManager
{
    [SerializeField] private AudioClip shieldHitSound;
    
    public void PlayShieldHit()
    {
        // Disabled it for now for the build because the sound is too loud and I havent fixed it yet
        //Play(shieldHitSound);    
    }
}
