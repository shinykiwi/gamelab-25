using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class LevelTrigger : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private bool canUse = false;
  
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        // Hide the object at first
        Hide();
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
        canUse = true;
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
        canUse = false;
    }
    
    
}
