using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

namespace GGJ.Audio {
	public class AudioManager : MonoBehaviour {
		static AudioManager Instance;

		[SerializeField] AudioClip startingBGM;
		[SerializeField] AudioMixer mixer;
		[SerializeField] AudioMixerGroup bgmGroup, sfxGroup;

		AudioSource musicSource;
		AudioSource soundSource;
		Tweener pitchTweener = null, distortTweener = null;
		[SerializeField] float pitchChangeAmount = 5, distortAmount = 1f;
		[SerializeField] AnimationCurve distortCurve;

		void Awake() {
			Instance = this;
			musicSource = gameObject.AddComponent<AudioSource>();
			musicSource.playOnAwake = false;
			musicSource.loop = true;
			musicSource.outputAudioMixerGroup = bgmGroup;
			soundSource = gameObject.AddComponent<AudioSource>();
			soundSource.playOnAwake = false;
			soundSource.loop = false;
			musicSource.outputAudioMixerGroup = sfxGroup;
		}

		void Start() {
			if (startingBGM) PlayBGM(startingBGM);
			DoPitchChange();
			DoDistort();
		}

		void DoPitchChange() {
			pitchTweener = mixer.DOSetFloat("Pitch", Random.Range(1 - (pitchChangeAmount / 100f), 1 + (pitchChangeAmount / 100f)), Random.Range(1f, 2f)).SetEase(Ease.InOutSine).OnComplete(() => DoPitchChange());
		}

		void DoDistort() {
			distortTweener = mixer.DOSetFloat("Distort", distortCurve.Evaluate(Random.Range(0, distortAmount)), Random.Range(0.5f, 1f)).SetEase(Ease.Linear).OnComplete(() => DoDistort());
		}

		public static void PlaySound(AudioClip sound, float volumeScale = 1) {
			if (!Instance) return;
			Instance.soundSource.PlayOneShot(sound, volumeScale);
		}

		public static void PlayBGM(AudioClip newBGM) {
			if (!Instance || Instance.musicSource.clip == newBGM) return;
			Instance.musicSource.DOFade(0, 1f).SetEase(Ease.Linear).OnComplete(() => {
				Instance.musicSource.Stop();
				Instance.musicSource.clip = newBGM;
				Instance.musicSource.Play();
				Instance.musicSource.DOFade(1, 1f).SetEase(Ease.Linear).SetDelay(0.5f);
			});
		}
	}
}