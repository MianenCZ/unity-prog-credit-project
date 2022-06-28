using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool invetory;
		public bool attack;
		public bool block;
		public bool roll;
		public bool crouch;

		[Header("Movement Settings")]
		public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnOpenInventory(InputValue value)
		{
			InventoryInput(value.isPressed);
		}

		public void OnAttack(InputValue value)
		{
			Attack(value.isPressed);
		}

		public void OnRoll(InputValue value)
        {
			Roll(value.isPressed);
		}

		public void OnCrouch(InputValue value)
		{
			Crouch(value.isPressed);
		}

		public void OnBlock(InputValue value)
		{
			Block(value.isPressed);
		}

#else
	// old input sys if we do decide to have it (most likely wont)...
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void InventoryInput(bool newInventoryState)
		{
			invetory = newInventoryState;
		}

		public void Attack(bool newAttackState)
		{
			attack = newAttackState;
		}

		public void Roll(bool newRollState)
		{
			roll = newRollState;
		}

		public void Block(bool newBlockState)
		{
			block = newBlockState;
		}

		public void Crouch(bool newCrouchState)
		{
			crouch = newCrouchState;
		}

#if !UNITY_IOS || !UNITY_ANDROID

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

#endif

	}
	
}