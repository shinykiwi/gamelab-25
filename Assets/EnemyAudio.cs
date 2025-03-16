using System;
using Code.Scripts;
using UnityEngine;

public class EnemyAudio : AudioManager
{
    [SerializeField] private AudioClip shieldHitSound;
    
    public void PlayShieldHit()
    {
        volume = 0.5f;
        Play(shieldHitSound);    
    }
}
