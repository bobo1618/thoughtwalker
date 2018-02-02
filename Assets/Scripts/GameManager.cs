using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGJ.Thoughts;
using GGJ.JournalStuff;
using GGJ.Audio;
using UnityEngine.Video;

namespace GGJ.Management {
	public class GameManager : MonoBehaviour {
		[SerializeField] List<GameStage> stages;
		[SerializeField] VideoPlayer videoPlayer;
        [SerializeField] VideoClip introVideo;
		[SerializeField] Animator faderAnim;

		public static GameManager Instance { get; private set; }
		public EmotionalState CurState {
			get { return stages[curStageIndex].state; }
		}
		public bool IsVideoPlaying {
			get { return isVideoPlaying; }
		}

		public delegate void VideoEndEvent();
		public VideoEndEvent OnVideoEnd;
		
		Dictionary<JournalEntry, bool> toBeUnlocked = new Dictionary<JournalEntry, bool>();
		int curStageIndex = -1;
		bool isVideoPlaying = false, allEntriesUnlocked = false;

		void Awake() {
			Instance = this;
		}

		void Start() {
			Cursor.visible = false;
			if (videoPlayer && AudioManager.VideoSource) videoPlayer.SetTargetAudioSource(0, AudioManager.VideoSource);
			if (introVideo) PlayVideo(introVideo);
			Journal.Instance.OnEntryUnlocked += OnEntryUnlocked;
			StartCoroutine(NextStage());
		}

		public void PlayVideo(VideoClip clip) {
			if (!videoPlayer || isVideoPlaying || !clip) return;
			videoPlayer.clip = clip;
			StartCoroutine(PlayVideoCR());
		}

		IEnumerator PlayVideoCR() {
			isVideoPlaying = true;
			if (faderAnim) faderAnim.SetBool("Fade", true);
			videoPlayer.Play();
			float keyDownTime = 0;
			while (videoPlayer.isPlaying && videoPlayer.time <= videoPlayer.clip.length) {
				if (Input.anyKey) {	// Skip the video if any key is held down for more than 1 second
					keyDownTime += Time.deltaTime;
					if (keyDownTime > 1f) break;
				}
				else keyDownTime = 0;
				yield return null;
			}
			if (isVideoPlaying) StopVideo();
		}

		void StopVideo() {
			if (!videoPlayer) return;
			videoPlayer.Stop();
			isVideoPlaying = false;
			if (faderAnim) faderAnim.SetBool("Fade", false);
			if (OnVideoEnd != null) OnVideoEnd();
		}

		IEnumerator NextStage() {
			if (curStageIndex >= 0) {
				yield return new WaitForSeconds(stages[curStageIndex].stageEndDelay);
				if (videoPlayer && stages[curStageIndex].stageEndVideo) {
					PlayVideo(stages[curStageIndex].stageEndVideo);
					while (isVideoPlaying) yield return null;
				}
				foreach (GameObject actGO in stages[curStageIndex].activateOnEnd) actGO.SetActive(true);
				foreach (GameObject actGO in stages[curStageIndex].deactivateOnEnd) actGO.SetActive(false);
			}
			curStageIndex++;
			if (curStageIndex >= stages.Count) {
				Debug.Log("CONGLATURATION!!! YOUR WINNER! And you were eaten by a grue.");
			}
			else {
				GameStage curStage = stages[curStageIndex];
				toBeUnlocked.Clear();
				foreach (JournalEntry entry in curStage.entriesToUnlock) toBeUnlocked[entry] = false;
				allEntriesUnlocked = false;
				Journal.Instance.SetLock(false);
				//Journal.Instance.SetVisibility(false);
			}
		}

		void OnEntryUnlocked(JournalEntry entry) {
			if (entry == stages[curStageIndex].stageEndingEntry) {
				StartCoroutine(NextStage());
				return;
			}

			if (allEntriesUnlocked) return;

			if (!toBeUnlocked.ContainsKey(entry)) return;
			toBeUnlocked[entry] = true;
			allEntriesUnlocked = true;
			foreach (bool isUnlocked in toBeUnlocked.Values) allEntriesUnlocked &= isUnlocked;
			if (allEntriesUnlocked) {   // Stage end officially begins here
				Journal.Instance.SetLock(true);
				foreach (JournalEntryUnlock unlock in stages[curStageIndex].unlockAfterAllUnlocked) Journal.Instance.AddEntry(unlock);
			}
		}
	}

	[System.Serializable]
	public class GameStage {
		public EmotionalState state;
		public List<JournalEntry> entriesToUnlock;
		public List<JournalEntryUnlock> unlockAfterAllUnlocked;
		public JournalEntry stageEndingEntry;
		public List<GameObject> activateOnEnd, deactivateOnEnd;
		public VideoClip stageEndVideo;
		public float stageEndDelay = 5f;
	}
}