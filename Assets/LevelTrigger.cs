using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class LevelTrigger : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private bool canUse = true;
  
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        
        // Hide the object at first
        meshRenderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canUse) return;
        canUse = false;
        LevelManager.Instance.LoadNextLevel();
    }

    public void Show()
    {
        meshRenderer.enabled = true;
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
    }
    
    
}
