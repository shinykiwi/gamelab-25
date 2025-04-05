using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private int numberOfAnimations;

    private void ChooseAnimation()
    {
        int num = Random.Range(1, numberOfAnimations);
        
        
    }
}
