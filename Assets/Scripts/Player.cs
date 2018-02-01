using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGJ.Management;
using GGJ.JournalStuff;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour {
	[SerializeField] float moveSpeed = 2f;
	[SerializeField] SpriteRenderer[] spritesToFlip;
	[SerializeField] Animator animator;

	public static Player Instance { get; private set; }

	new Rigidbody2D rigidbody;
	bool flipX = false;
	float movementLockTime = 0;

	void Awake () {
		Instance = this;
		rigidbody = GetComponent<Rigidbody2D>();
	}
	
	void Update () {
		if (animator) animator.SetFloat("Speed", Mathf.Abs(rigidbody.velocity.x));
		if (movementLockTime > 0) {
			movementLockTime -= Time.deltaTime;
			return;
		}
		if ((Journal.Instance && Journal.Instance.IsVisible) || (GameManager.Instance && GameManager.Instance.IsVideoPlaying)) return;
		Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		rigidbody.velocity = Vector2.right * input.x * moveSpeed;
		if (input.x != 0 && (input.x > 0) == flipX) {
			flipX = !flipX;
			foreach (SpriteRenderer sprite in spritesToFlip) sprite.flipX = flipX;
		}
	}

	public void Fade(bool fadeOut) {
		if (animator) animator.SetBool("Fade", fadeOut);
		GetComponent<Collider2D>().enabled = !fadeOut;
		rigidbody.simulated = !fadeOut;
	}

	public void LockMovement(float forSeconds) {
		movementLockTime = Mathf.Max(forSeconds, movementLockTime);
	}
}