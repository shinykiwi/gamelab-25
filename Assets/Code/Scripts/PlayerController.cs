using DG.Tweening;
using System.Collections;
using Code.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    float moveSpeed = 1.0f;

    [Header("VFX")] [SerializeField] private ParticleSystem walkingParticles;

    [Header("Collisions")]
    [SerializeField]
    float distanceTravelledHitByProjectile = 1.0f;

    [SerializeField]
    float durationTimeHitByProjectile = 0.25f;

    [Header("References")]

    [SerializeField]
    Animator player_animator;

    [SerializeField]
    Transform character_transform;

    [SerializeField]
    LayerMask fenceMask;

    PlayerInput playerInput;
    Rigidbody rb;
    Collider coll;

    InputAction moveAction;
    InputAction lookAction;

    private PlayerAudio playerAudio;

    float rotation_speed = 1000f;

    bool isInvincibleToProjectiles = false;
    bool isBeingHit = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        coll = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        moveAction = playerInput.actions.FindAction("Move");
        lookAction = playerInput.actions.FindAction("Look");
        playerAudio = GetComponentInChildren<PlayerAudio>();
    }

    void FixedUpdate()
    {
        if(!isBeingHit)
        {
            Vector2 moveInputValue = moveAction.ReadValue<Vector2>();
            Vector3 move = new Vector3(moveInputValue.x, 0, moveInputValue.y);
            Move(move);
        }
    }

    public void GetHitByProjectile(Vector3 direction)
    {
        if(isInvincibleToProjectiles)
            return;
        
        StartCoroutine(DoProjectileHit(direction, distanceTravelledHitByProjectile, durationTimeHitByProjectile));
    }

    public void ToggleProjectileInvincibility()
    {
        isInvincibleToProjectiles = !isInvincibleToProjectiles;
    }

    void Move(Vector3 move)
    {
        float movement_amount = Mathf.Abs(move.x) + Mathf.Abs(move.z);
        player_animator.SetFloat("movement_amount", movement_amount);
        player_animator.SetBool("is_aiming", false);

        //Move the player
        rb.linearVelocity = new Vector3(0.0f, rb.linearVelocity.y, 0.0f) + moveSpeed * move;

        if (move.magnitude > 0)
        {
            if (!walkingParticles.isPlaying)
            {
                walkingParticles.Play();
            }
            playerAudio.PlayFootsteps();
        }
        else
        {
            walkingParticles.Stop();
            playerAudio.Stop();
        }

        //Adjust the character rotation (child)
        Vector3 camera_rotation = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
        if(move.magnitude > 0)
        {
            Quaternion target_rotation = Quaternion.LookRotation(Quaternion.LookRotation(camera_rotation) * move);
            character_transform.rotation =
               Quaternion.RotateTowards(character_transform.rotation, target_rotation, rotation_speed * Time.deltaTime);
        }
    }

    private IEnumerator DoProjectileHit(Vector3 direction, float distance, float duration)
    {
        // TODO add animation
        playerInput.DeactivateInput();
        isBeingHit = true;
        rb.useGravity = false;
        coll.excludeLayers += fenceMask;

        rb.linearVelocity = distance / duration * direction;
        yield return new WaitForSeconds(duration);

        rb.linearVelocity = Vector3.zero;
        rb.useGravity = true;
        coll.excludeLayers -= fenceMask;
        isBeingHit = false;
        playerInput.ActivateInput();
    }
}
