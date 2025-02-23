using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerAnimation : MonoBehaviour
{
   [SerializeField]
   private Animator animator;
   private Vector3 v3_movement;
   private Vector3 v3_aim;
   private CharacterController characterController;
   private float speed_current = 0f;
   private float speed_while_running = 4f;
   private float speed_while_aiming = 4f;
   
   private float rotation_speed = 1000f;

   private void Start()
   {
      characterController = GetComponent<CharacterController>();
   }

   private void Update()
   {
      var controller = Gamepad.current;
      if (controller == null) return;

      v3_movement = new Vector3(controller.leftStick.x.value, 0f, controller.leftStick.y.value);
      v3_aim = new Vector3(controller.rightStick.x.value, 0f, controller.rightStick.y.value);

      float movement_amount = Mathf.Abs(v3_movement.x) + Mathf.Abs(v3_movement.z);
      float aim_amount = Mathf.Abs(v3_aim.x) + Mathf.Abs(v3_aim.z);

      Vector3 camera_rotation = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);

      if (aim_amount > 0)
      {
         Quaternion target_rotation = Quaternion.LookRotation(Quaternion.LookRotation(camera_rotation) * v3_aim);
         transform.rotation =
            Quaternion.RotateTowards(transform.rotation, target_rotation, rotation_speed * Time.deltaTime);

         Vector3 walk_aim_direction = Quaternion.LookRotation(new Vector3(-v3_aim.x, 0f, v3_aim.z)) * v3_movement;
         
         animator.SetFloat("aim_x", walk_aim_direction.x, 0.15f, Time.deltaTime);
         animator.SetFloat("aim_y", walk_aim_direction.z, 0.15f, Time.deltaTime);
      }
      else if (movement_amount > 0)
      {
         Quaternion target_rotation = Quaternion.LookRotation(Quaternion.LookRotation(camera_rotation) * v3_movement);
         transform.rotation =
            Quaternion.RotateTowards(transform.rotation, target_rotation, rotation_speed * Time.deltaTime);
         
      }
      
      animator.SetFloat("movement_amount", movement_amount);
      animator.SetBool("is_aiming", aim_amount > 0 ? true : false);

      if (!animator.applyRootMotion)
      {
         speed_current = aim_amount > 0 ? speed_while_aiming : speed_while_running;
         Vector3 move_direction = Quaternion.LookRotation(camera_rotation) * v3_movement;
         characterController.Move(move_direction * speed_current * Time.deltaTime);
      }
   }
}
