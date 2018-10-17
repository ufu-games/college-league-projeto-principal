﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	[Space(5)]
	[Header("Movement Handling")]
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	
	[Header("Jump Handling")]
	public float goingUpGravity = -25f;
	public float goingDownGravity = -50f;
	public float floatingGravity = -10f;
	private float m_gravity;
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;
	public float jumpPressedRememberTime = 0.15f;
	public float groundedRememberTime = 0.15f;
	public float cutJumpHeight = 0.35f;
	private float m_jumpPressedRemember;
	private float m_groundedRemember;
	
	[Space(5)]
	[Header("Wall Jump Handling")]
	public float onWallGravity = -5f;
	public Vector2 wallJumpVelocity = new Vector2(-5f, 5f);
	private bool m_isOnWall;
	
	[Space(5)]
	[Header("Audio Handling")]
	public AudioClip hurtClip;


	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	private Prime31.CharacterController2D m_controller;
	private Animator m_animator;
	private RaycastHit2D m_lastControllerColliderHit;
	private Vector3 m_velocity;
	
	// Dialogue Handling
	// se tiver dialogo rolando na tela, bloquear o input do player...
	private bool m_isShowingDialogue;


	// OnCollision, OnTrigger, etc... - Have some kind of API (i.e. Interact, ...)
	// easily extendable


	void Awake()
	{
		m_animator = GetComponent<Animator>();
		m_controller = GetComponent<Prime31.CharacterController2D>();

		// listen to some events for illustration purposes
		m_controller.onControllerCollidedEvent += onControllerCollider;
		m_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		m_controller.onTriggerExitEvent += onTriggerExitEvent;
		
		m_gravity = goingUpGravity;
	}


	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;

		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		Debug.Log( "flags: " + m_controller.collisionState + ", hit.normal: " + hit.normal );
	}

	void onTriggerEnterEvent(Collider2D col) {
		Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );

		// Interfaces
		IDangerous dangerousInteraction = col.gameObject.GetComponent<IDangerous>();
		IInteractable interaction = col.gameObject.GetComponent<IInteractable>();
		IShowDialogue showDialogue = col.gameObject.GetComponent<IShowDialogue>();

		if(dangerousInteraction != null) {
			if(hurtClip) { SoundManager.instance.PlaySfx(hurtClip); }
			dangerousInteraction.InteractWithPlayer(this.GetComponent<Collider2D>());
		}

		if(interaction != null) {
			interaction.Interact();
		}

		if(showDialogue != null) {
			showDialogue.ShowDialogue();
			m_isShowingDialogue = true;
		}

		if(col.gameObject.layer == LayerMask.NameToLayer("JumpingPlatform")) {
			m_velocity.y = Mathf.Sqrt( 5f * jumpHeight * -m_gravity );
			m_animator.Play( "Jump" );
		}
	}


	void onTriggerExitEvent( Collider2D col )
	{
		Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}

	#endregion

	void Update()
	{
		if(m_controller.isGrounded) {
			m_groundedRemember = groundedRememberTime;
			m_gravity = goingUpGravity;
			m_velocity.y = 0;
		}

		if(m_isShowingDialogue) {
			m_animator.Play("Idle");
			m_velocity = Vector2.zero;
			if(!DialogueManager.instance.isShowingDialogue) m_isShowingDialogue = false;
			return;
		}

		Move();
		AnimationLogic();
		Jump();
		Float();
		WallJump();

		var smoothedMovementFactor = m_controller.isGrounded ? groundDamping : inAirDamping;
		// mudar aqui, usar lerp no futuro

		m_velocity.x = Mathf.Lerp( m_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		// velocity verlet for y velocity
		m_velocity.y += m_gravity * Time.deltaTime;
		
		// ignora as "one way platforms" por um frame (para cair delas)
		if( m_controller.isGrounded && Input.GetKey( KeyCode.DownArrow ) )
		{
			m_velocity.y *= 3f;
			m_controller.ignoreOneWayPlatformsThisFrame = true;
		}

		// applying velocity verlet on delta position for y axis
		// standard euler on x axis
		Vector2 deltaPosition = new Vector2(m_velocity.x * Time.deltaTime, (m_velocity.y * Time.deltaTime) + (.5f * m_gravity * (Time.deltaTime * Time.deltaTime)));
		
		m_controller.move( deltaPosition );
		m_velocity = m_controller.velocity;
	}

	private void AnimationLogic() {
		if(Mathf.Abs(m_velocity.y) > Mathf.Epsilon) {
			m_animator.Play("Jump");
		} else if(Mathf.Abs(normalizedHorizontalSpeed) > 0 && m_controller.isGrounded) {
			m_animator.Play("Running");
		} else {
			m_animator.Play("Idle");
		}
	}

	private void Move() {
		if( Input.GetKey( KeyCode.RightArrow ) ) {
			normalizedHorizontalSpeed = 1;
			if( transform.localScale.x < 0f ) {
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
			}
		}
		else if( Input.GetKey( KeyCode.LeftArrow ) ) {
			normalizedHorizontalSpeed = -1;
			if( transform.localScale.x > 0f ) {
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
			}
		} else {
			normalizedHorizontalSpeed = 0;
		}
	}

	private void Jump() {
		m_groundedRemember -= Time.deltaTime;
		m_jumpPressedRemember -= Time.deltaTime;

		if(Input.GetKeyDown(KeyCode.UpArrow)) {
			m_jumpPressedRemember = jumpPressedRememberTime;
		}

		if(Input.GetKeyUp(KeyCode.UpArrow)) {
			if(m_velocity.y > 0) {
				m_velocity.y = m_velocity.y * cutJumpHeight;
			}
		}

		if(m_velocity.y < 0 && !m_isOnWall) {
			m_gravity = goingDownGravity;
		}

		// REGULAR JUMP
		// || ((m_controller.collisionState.right || m_controller.collisionState.left) && !m_controller.isGrounded))
		if( ( (m_groundedRemember > 0) && (m_jumpPressedRemember > 0) ) ) {
			m_jumpPressedRemember = 0;
			m_groundedRemember = 0;

			m_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -m_gravity );
			m_animator.Play( "Jump" );
		}
	}

	private void Float() {
		if(m_velocity.y >= 0) return;

		if(Input.GetKey(KeyCode.UpArrow)) {
			m_gravity = floatingGravity;
		} else {
			m_gravity = goingDownGravity;
		}
	}

	private void WallJump() {
		if(m_controller.isGrounded) return;

		// Stick to Wall
		if(((m_controller.collisionState.right && Input.GetKey(KeyCode.RightArrow)) ||
			(m_controller.collisionState.left && Input.GetKey(KeyCode.LeftArrow)))) {
				m_isOnWall = true;
				if(m_velocity.y < 0) m_gravity = onWallGravity;
			} else {
				m_isOnWall = false;
			}
		
		// Wall Jump
		if((m_controller.collisionState.right || m_controller.collisionState.left) && Input.GetKeyDown(KeyCode.UpArrow)) {
			m_gravity = goingUpGravity;
			m_velocity.x = wallJumpVelocity.x * Mathf.Sign(transform.localScale.x);
			m_velocity.y = Mathf.Sqrt(2f * wallJumpVelocity.y * -m_gravity);
		}
	}
}

