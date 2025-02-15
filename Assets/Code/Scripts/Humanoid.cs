using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class Humanoid : MonoBehaviour
{
    // Not sure if this will be needed but it's just an example of where we can put common vars like this
    private float health = 100f;
    
    // Internal variables
    private MeshRenderer meshRenderer;
    private Collider collider;

    private void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        collider = gameObject.GetComponent<Collider>();
    }


    // For colliding with void
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Void"))
        {
            Death();
        }
    }

    private void Death()
    {
        health = 0f;

        // Damage visual sequence
        Material material = meshRenderer.material;
        Color originalColor = material.color;
        float duration = 0.2f;
        
        Sequence sequence = DOTween.Sequence();
        
        sequence.Append(material.DOColor(Color.red, duration)); 
        sequence.Append(material.DOColor(originalColor, duration));
        sequence.AppendCallback(ToggleVisibility);

        sequence.Play();

        StartCoroutine(DelaySpawn(5));
    }

    private void Respawn()
    {
        health = 100f;
        
        Material material = meshRenderer.material;
        Color originalColor = material.color;
        originalColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); 
        
        material.color = originalColor;

        material.DOFade(1f, 1f);
    }

    /// <summary>
    /// Spawns self at the specified position given by the transform.
    /// </summary>
    /// <param name="t"></param>
    private void Spawn(Transform t)
    {
        Spawn(t.position);
    }

    /// <summary>
    /// Spawns self at the specified position.
    /// </summary>
    /// <param name="position"></param>
    private void Spawn(Vector3 position)
    {
        gameObject.transform.position = position;
        ToggleVisibility();
        Respawn();
    }

    /// <summary>
    /// Spawns self at the spawn point given by the LevelManager.
    /// </summary>
    private void Spawn()
    {
        // Try to find the level manager since no transform was given
        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        
        // If there is a level manager in the scene, use the designated spawn point
        if (levelManager)
        {
            Spawn(levelManager.GetSpawnPoint());
        }
        else
        {
            Spawn(Vector3.zero);
        }
    }

    private IEnumerator DelaySpawn(float delay)
    {
        Debug.Log("Delay spawn!");
        yield return new WaitForSeconds(delay);
        Spawn();
    }

    /// <summary>
    /// Either shows or hides self and children.
    /// </summary>
    private void ToggleVisibility()
    {
        bool visibility = !meshRenderer.enabled;
        meshRenderer.enabled = visibility;
        MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var r in renderers)
        {
            r.enabled = visibility;
        }
    }
}
