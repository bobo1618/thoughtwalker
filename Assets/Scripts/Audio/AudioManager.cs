using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GGJ.Audio {
	public class AudioManager : MonoBehaviour {
		static AudioManager Instance {
			get {
				if (!_Instance) _Instance = new GameObject().AddComponent<AudioManager>();
				return _Instance;
			}
		}
		static AudioManager _Instance = null;

		AudioSource musicSource;
		AudioSource soundSource;

		void Awake() {
			_Instance = this;
			musicSource = gameObject.AddComponent<AudioSource>();
			musicSource.playOnAwake = false;
			musicSource.loop = true;
			soundSource = gameObject.AddComponent<AudioSource>();
			soundSource.playOnAwake = false;
			soundSource.loop = false;
		}

		public static void PlaySound(AudioClip sound, float volumeScale = 1) {
			Instance.soundSource.PlayOneShot(sound, volumeScale);
		}

		public static void PlayBGM(AudioClip newBGM) {
			Instance.musicSource.DOFade(0, 0.5f).OnComplete(() => {
				Instance.musicSource.Stop();
				Instance.musicSource.clip = newBGM;
				Instance.musicSource.Play();
				Instance.musicSource.DOFade(1, 0.5f);
			});
		}
	}
}