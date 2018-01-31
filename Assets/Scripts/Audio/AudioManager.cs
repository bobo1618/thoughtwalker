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
		[SerializeField] AudioMixerGroup bgmGroup, sfxGroup, videoGroup;

		AudioSource bgmSource, sfxSource, videoSource;
		Tweener pitchTweener = null, distortTweener = null;
		[SerializeField] float pitchChangeAmount = 5, distortAmount = 1f;
		[SerializeField] AnimationCurve distortCurve;
		float effectMultiplier = 0;

		public static AudioSource VideoSource {
			get { return Instance.videoSource; }
		}

		void Awake() {
			Instance = this;

			bgmSource = gameObject.AddComponent<AudioSource>();
			bgmSource.playOnAwake = false;
			bgmSource.loop = true;
			bgmSource.outputAudioMixerGroup = bgmGroup;

			sfxSource = gameObject.AddComponent<AudioSource>();
			sfxSource.playOnAwake = false;
			sfxSource.loop = false;
			sfxSource.outputAudioMixerGroup = sfxGroup;
			
			videoSource = gameObject.AddComponent<AudioSource>();
			videoSource.playOnAwake = true;
			videoSource.loop = true;
			videoSource.outputAudioMixerGroup = videoGroup;
		}

		void Start() {
			if (startingBGM) SetBGM(startingBGM);
			DoPitchChange();
			DoDistort();
		}

		void DoPitchChange() {
			pitchTweener = mixer.DOSetFloat("Pitch", Random.Range(1 - (pitchChangeAmount * effectMultiplier / 100f), 1 + (pitchChangeAmount / 100f)), Random.Range(1f, 2f)).SetEase(Ease.InOutSine).OnComplete(() => DoPitchChange());
		}

		void DoDistort() {
			distortTweener = mixer.DOSetFloat("Distort", distortCurve.Evaluate(Random.Range(0, distortAmount)), Random.Range(0.5f, 1f)).SetEase(Ease.Linear).OnComplete(() => DoDistort());
		}

		public static void PlaySound(AudioClip sound, float volumeScale = 1) {
			if (!Instance) return;
			Instance.sfxSource.PlayOneShot(sound, volumeScale);
		}

		public static void SetBGM(AudioClip newBGM) {
			if (!Instance || Instance.bgmSource.clip == newBGM) return;
			Instance.bgmSource.DOFade(0, 1f).SetEase(Ease.Linear).OnComplete(() => {
				Instance.bgmSource.Stop();
				Instance.bgmSource.clip = newBGM;
				Instance.bgmSource.Play();
				Instance.bgmSource.DOFade(1, 1f).SetEase(Ease.Linear).SetDelay(0.5f);
			});
		}

		public static void SetEffectMultiplier(float mult) {
			if (!Instance) return;
			Instance.effectMultiplier = Mathf.Max(0, mult);
		}


		AudioClip resetBGM;
		float resetMult;
		bool resetReady;

		public static void PrepareToReset() {
			if (!Instance) return;
			Instance.resetBGM = Instance.bgmSource.clip;
			Instance.resetMult = Instance.effectMultiplier;
			Instance.resetReady = true;
		}

		public static void Reset() {
			if (!Instance || !Instance.resetReady) return;
			Instance.bgmSource.clip = Instance.resetBGM;
			Instance.effectMultiplier = Instance.resetMult;
			Instance.resetReady = false;
		}
	}
}