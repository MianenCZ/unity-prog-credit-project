using Cinemachine;
using DG.Tweening;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
	public enum MovemenState
    {
		Idle,
		Walk,
		Sprint,
		Combat,
		Crouch,
		None,
    }

	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class ThirdPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 2.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 5.335f;
		[Tooltip("Crouch speed of the character in m/s")]
		public float CrouchSpeed = 2.5f;
		[Tooltip("Sprint crouch speed of the character in m/s")]
		public float SprintCrouchSpeed = 3.5f;

		[Tooltip("Roll speed of the character in m/s")]
		public float RollSpeed = 1.8f;
		[Tooltip("Sprint roll speed of the character in m/s")]
		public float SprintRollSpeed = 5.0f;

		[Tooltip("How fast the character turns to face movement direction")]
		[Range(0.0f, 0.3f)]
		public float RotationSmoothTime = 0.12f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;
		//TODO: readonly
		[Tooltip("Cuurent speed of character")]
		public float CurrentSpeed = 0f;
		public MovemenState MovemenState = MovemenState.Idle;
		public MovemenState CloseUpState = MovemenState.Idle;


		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.50f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.28f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 70.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -30.0f;
		[Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
		public float CameraAngleOverride = 0.0f;
		[Tooltip("For locking the camera position on all axis")]
		public bool LockCameraPosition = false;


		[Tooltip("For locking the player position and rotation")]
		public bool LockPlayerPosition = false;

		// cinemachine
		private float _cinemachineTargetYaw;
		private float _cinemachineTargetPitch;

		// player
		private float _speed; 
		private float _animationBlend;
		private float _targetRotation = 0.0f;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

		// animation IDs
		private int _animIDSpeed;
		private int _animIDGrounded;
		private int _animIDJump;
		private int _animIDFreeFall;
		private int _animIDMotionSpeed;
		private int _animIDAttack;
		private int _animIDCrouch;
		private int _animIDRoll;
		private int _animIDBlock;

		private Animator _animator;
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

		private bool _hasAnimator;

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			_hasAnimator = TryGetComponent(out _animator);
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();

			AssignAnimationIDs();

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;

			InventoryCanvas.gameObject.SetActive(false);
		}

		private void Update()
		{
			_hasAnimator = TryGetComponent(out _animator);
			
			JumpAndGravity();
			GroundedCheck();
			if (!LockPlayerPosition)
			{
				Move();
			}
			if (Grounded)
			{
				CheckControls();
				CheckCombat();
			}
		}

		private void LateUpdate()
		{
			if (!LockPlayerPosition)
			{
				CameraRotation();
			}
		}


		private void AssignAnimationIDs()
		{
			_animIDSpeed = Animator.StringToHash("Speed");
			_animIDGrounded = Animator.StringToHash("Grounded");
			_animIDJump = Animator.StringToHash("Jump");
			_animIDFreeFall = Animator.StringToHash("FreeFall");
			_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
			_animIDAttack = Animator.StringToHash("Attack");
			_animIDCrouch = Animator.StringToHash("Crouch");
			_animIDRoll = Animator.StringToHash("Roll");
			_animIDBlock = Animator.StringToHash("Block");
		}

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

			// update animator if using character
			if (_hasAnimator)
			{
				_animator.SetBool(_animIDGrounded, Grounded);
			}
		}

		private void CameraRotation()
		{
			// if there is an input and camera position is not fixed
			if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
			{
				_cinemachineTargetYaw += _input.look.x * Time.deltaTime;
				_cinemachineTargetPitch += _input.look.y * Time.deltaTime;
			}

			// clamp our rotations so our values are limited 360 degrees
			_cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
			_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

			// Cinemachine will follow this target
			CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
		}

		private void Move()
		{
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
			if(_input.roll)
				targetSpeed = _input.sprint ? SprintRollSpeed : RollSpeed;
			if (_input.crouch)
				targetSpeed = _input.sprint ? SprintCrouchSpeed : CrouchSpeed;

			//Debug.Log($"Roll: {_input.roll}");

			_animator.SetBool(_animIDRoll, _input.roll);
			_animator.SetBool(_animIDCrouch, _input.crouch);

			if (CombatStage > 0)
			{
				targetSpeed = 0;
			}
			else
			{
				MovemenState = _input.sprint ? MovemenState.Sprint : MovemenState.Walk;
			}

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero)
			{
				targetSpeed = 0.0f;
				if (MovemenState != MovemenState.Combat)
					MovemenState = MovemenState.Idle;
			}

			if(IsCloseUp)
            {
				ChangeCloseUp();
			}


			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}
			_animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				_targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
				float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

				// rotate to face input direction relative to camera position
				transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
			}


			Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

			// move the player
			_controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

			CurrentSpeed = _speed;
			// update animator if using character
			if (_hasAnimator)
			{
				_animator.SetFloat(_animIDSpeed, _animationBlend);
				_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
			}


		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// update animator if using character
				if (_hasAnimator)
				{
					_animator.SetBool(_animIDJump, false);
					_animator.SetBool(_animIDFreeFall, false);
				}

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

					// update animator if using character
					if (_hasAnimator)
					{
						_animator.SetBool(_animIDJump, true);
					}
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}
				else
				{
					// update animator if using character
					if (_hasAnimator)
					{
						_animator.SetBool(_animIDFreeFall, true);
					}
				}

				// if we are not grounded, do not jump
				_input.jump = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		[Header("InventoryCloseUp")]
		public bool InventoryOpened = false;
		public float InventoryInCameraDistance = 5f;
		public float InventoryOutCameraDistance = 20f;
		public Vector3 InventoryInShoulderOffset;
		public Vector3 InventoryOutShoulderOffset;
		public Vector3 InventoryInventoryRotation;
		[SerializeField] CinemachineVirtualCamera playerFollowCamera;
		[SerializeField] Canvas InventoryCanvas;

		public Vector3 _onInventoryOpenRotation;
		private Tween _runningInventoryTween;


		[Header("Combat system")]
		public bool InCombat = false;
		public int CombatStage = 0;
		public bool CombatStageIncrementWindow = true;

		private void CheckControls()
        {
			if(_input.invetory)
			{
				Debug.LogWarning("CheckControls : _input.invetory");

				if(_runningInventoryTween != null)
                {
					return;
				}

				CinemachineComponentBase componentBase = playerFollowCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
				var follow = (componentBase as Cinemachine3rdPersonFollow);

				if (!(componentBase is Cinemachine3rdPersonFollow))
				{
					Debug.LogError("playerFollowCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) is not Cinemachine3rdPersonFollow");
				}

				if (!InventoryOpened)
                {
					InventoryOpened = true;

					_onInventoryOpenRotation = this.CinemachineCameraTarget.transform.localEulerAngles;
					
					this.LockCameraPosition = true;
					this.LockPlayerPosition = true;
					_runningInventoryTween = DOTween.Sequence()
                        .Join(DOTween.To(() => follow.ShoulderOffset, x => follow.ShoulderOffset = x, InventoryInShoulderOffset, 0.5f))
                        .Join(DOTween.To(() => follow.CameraDistance, x => follow.CameraDistance = x, InventoryInCameraDistance, 0.5f))
                        .Join(this.CinemachineCameraTarget.transform.DOLocalRotate(InventoryInventoryRotation, 0.5f))
						.OnComplete(() =>
						{
							_runningInventoryTween = null;
                            InventoryCanvas.gameObject.SetActive(true);
                            Cursor.visible = true;
                            Cursor.lockState = CursorLockMode.None;
                        });
				}
				else
				{
					InventoryOpened = false;
					InventoryCanvas.gameObject.SetActive(false);
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;

                    _runningInventoryTween = DOTween.Sequence()
                        .Join(DOTween.To(() => follow.ShoulderOffset, x => follow.ShoulderOffset = x, InventoryOutShoulderOffset, 0.5f))
                        .Join(DOTween.To(() => follow.CameraDistance, x => follow.CameraDistance = x, InventoryOutCameraDistance, 0.5f))
                        .Join(this.CinemachineCameraTarget.transform.DOLocalRotate(_onInventoryOpenRotation, 0.5f))
                        .OnComplete(() =>
						{
							_onInventoryOpenRotation = default;
							_runningInventoryTween = null;
							this.LockCameraPosition = false;
							this.LockPlayerPosition = false;
						});

				}
			}
        }

		private void CheckCombat()
        {
			Debug.Log($"_input.attack: {_input.attack}");
			if (!InventoryOpened && CombatStageIncrementWindow && _input.attack)
			{
				CombatStageIncrementWindow = false;
				CombatStage++;
			}

			if (CombatStage > 0)
				MovemenState = MovemenState.Combat;

			_animator.SetInteger(_animIDAttack, CombatStage);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;
			
			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}

		//*
		[Header("Cammera change")]
		public bool IsCloseUp = false;

        public float CloseViewInCameraSide = 1.0f;
        public float CloseViewInCameraDistance = 3.0f;


		public float CloseViewInSprintCameraSide = 0.8f;
		public float CloseViewInSprintCameraDistance = 5f;

		public float CloseViewOutCameraSide = 0.5f;
        public float CloseViewOutCameraDistance = 20.0f;

		private void ChangeCloseViewValues(float cameraSide, float cameraDistance)
		{
			CinemachineComponentBase componentBase = playerFollowCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
			if (componentBase is Cinemachine3rdPersonFollow)
			{
				Debug.Log($"ChangeCloseViewValues({cameraSide},{cameraDistance})");
				var follow = (componentBase as Cinemachine3rdPersonFollow);
				DOTween.To(() => follow.CameraDistance, x => follow.CameraDistance = x, cameraDistance, 1f);
				DOTween.To(() => follow.CameraSide, x => follow.CameraSide = x, cameraSide, 1f);
			}
		}

        private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.GetComponent<TriggerCloseUpScript>() != null)
			{
				IsCloseUp = true;
				ChangeCloseUp();
			}
		}

		private void ChangeCloseUp()
        {
			if (MovemenState == CloseUpState)
				return;

			if(!IsCloseUp)
			{
				ChangeCloseViewValues(CloseViewOutCameraSide, CloseViewOutCameraDistance);
				return;
			}


			switch (MovemenState)
            {
				case MovemenState.None:
					ChangeCloseViewValues(CloseViewOutCameraSide, CloseViewOutCameraDistance);
					break;
				case MovemenState.Idle:
                case MovemenState.Walk:
					ChangeCloseViewValues(CloseViewInCameraSide, CloseViewInCameraDistance);
					break;
                case MovemenState.Sprint:
                case MovemenState.Combat:
					ChangeCloseViewValues(CloseViewInSprintCameraSide, CloseViewInSprintCameraDistance);
					break;
			}
			CloseUpState = MovemenState;

		}

        private void OnTriggerExit(Collider other)
		{
			if (other.gameObject.GetComponent<TriggerCloseUpScript>() != null)
			{
				IsCloseUp = false;
				CloseUpState = MovemenState.None;
				ChangeCloseUp();
			}
		}
		

		//*/
		#region [ Right hand attacks ]
		public void RightHandAttack01Start()
		{
			//Do nothing
		}
		public void RightHandAttack01Hit()
		{
			CombatStageIncrementWindow = true;
		}
		public void RightHandAttack01End()
		{
			if(CombatStageIncrementWindow)
				CombatStage = 0;
		}
		public void RightHandAttack02Start()
		{
			//Do nothing
		}
		public void RightHandAttack02Hit()
		{
			CombatStageIncrementWindow = true;
		}
		public void RightHandAttack02End()
		{
			if (CombatStageIncrementWindow)
				CombatStage = 0;
		}
		public void RightHandAttack03Start()
		{
			//Do nothing
		}
		public void RightHandAttack03Hit()
		{
			CombatStageIncrementWindow = true;
		}
		public void RightHandAttack03End()
		{
			if (CombatStageIncrementWindow)
				CombatStage = 0;
		}
		public void RightHandAttack04Start()
		{
			//Do nothing
		}
		public void RightHandAttack04Hit()
		{
			CombatStageIncrementWindow = true;
		}
		public void RightHandAttack04End()
		{
			if (CombatStageIncrementWindow)
				CombatStage = 0;
			else
				CombatStage = 1;
		}
		#endregion


		#region [ Long attacks ]
		public void LongAttack01Start()
		{
			//Do nothing
		}
		public void LongAttack01Hit()
		{
			CombatStageIncrementWindow = true;
		}
		public void LongAttack01End()
		{
			if (CombatStageIncrementWindow)
				CombatStage = 0;
		}
		public void LongAttack02Start()
		{
			//Do nothing
		}
		public void LongAttack02Hit()
		{
			CombatStageIncrementWindow = true;
		}
		public void LongAttack02End()
		{
			if (CombatStageIncrementWindow)
				CombatStage = 0;
		}
		public void LongAttack03Start()
		{
			//Do nothing
		}
		public void LongAttack03Hit()
		{
			CombatStageIncrementWindow = true;
		}
		public void LongAttack03End()
		{
			if (CombatStageIncrementWindow)
				CombatStage = 0;
		}
		public void LongAttack04Start()
		{
			//Do nothing
		}
		public void LongAttack04Hit()
		{
			CombatStageIncrementWindow = true;
		}
		public void LongAttack04End()
		{
			if (CombatStageIncrementWindow)
				CombatStage = 0;
			else
				CombatStage = 1;
		}
		#endregion
	}
}