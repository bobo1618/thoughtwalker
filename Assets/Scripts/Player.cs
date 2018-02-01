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

	new Rigidbody2D rigidbody;
	bool flipX = false;

	void Awake () {
		rigidbody = GetComponent<Rigidbody2D>();
	}
	
	void Update () {
		if (animator) animator.SetFloat("Speed", Mathf.Abs(rigidbody.velocity.x));
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
}