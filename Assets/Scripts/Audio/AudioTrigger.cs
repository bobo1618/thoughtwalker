using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ.Audio {
	public class AudioTrigger : MonoBehaviour {
		[SerializeField] AudioClip bgmClip;
		[SerializeField] bool affectEffects = false;
		[SerializeField][Range(0,1)] float effectMultiplier = 0;
		[SerializeField] bool resetOnExit = false;

		void OnTriggerEnter2D(Collider2D coll) {
			if (!coll.CompareTag("Player")) return;
			if (resetOnExit) AudioManager.PrepareToReset();
			if (bgmClip) AudioManager.SetBGM(bgmClip);
			if (affectEffects) AudioManager.SetEffectMultiplier(effectMultiplier);
		}

		void OnTriggerExit2D(Collider2D coll) {
			if (!resetOnExit || !coll.CompareTag("Player")) return;
			AudioManager.Reset();
		}
	}
}