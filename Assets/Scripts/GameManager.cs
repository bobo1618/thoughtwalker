using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGJ.Thoughts;
using GGJ.Journal;
using GGJ.Audio;
using UnityEngine.Video;

namespace GGJ.Management {
	public class GameManager : MonoBehaviour {
		[SerializeField] List<GameStage> stages;
		[SerializeField] float stageEndDelay = 5f;
		[SerializeField] VideoPlayer videoPlayer;

		public static GameManager Instance { get; private set; }
		public EmotionalState CurState {
			get { return stages[curStageIndex].state; }
		}

		public Dictionary<JournalEntry, bool> toBeUnlocked = new Dictionary<JournalEntry, bool>();

		int curStageIndex = -1;

		void Start() {
			Journal.Journal.Instance.OnEntryUnlocked += OnEntryUnlocked;
			StartCoroutine(NextStage(0));
		}

		IEnumerator NextStage(float delay) {
			yield return new WaitForSeconds(stageEndDelay);
			if (curStageIndex >= 0) {
				if (videoPlayer && stages[curStageIndex].stageEndVideo) {
					videoPlayer.clip = stages[curStageIndex].stageEndVideo;
					videoPlayer.Play();
				}
				foreach (GameObject actGO in stages[curStageIndex].activateOnEnd) actGO.SetActive(true);
				foreach (GameObject actGO in stages[curStageIndex].deactivateOnEnd) actGO.SetActive(false);
			}
			curStageIndex++;
			Debug.Log("Next stage");
			if (curStageIndex >= stages.Count) {
				Debug.Log("CONGLATURATION!!! YOUR WINNER! And you were eaten by a grue.");
			}
			else {
				GameStage curStage = stages[curStageIndex];
				toBeUnlocked.Clear();
				foreach (JournalEntry entry in curStage.entriesToUnlock) toBeUnlocked[entry] = false;
			}
		}

		bool allEntriesUnlocked = false;

		void OnEntryUnlocked(JournalEntry entry) {
			if (entry == stages[curStageIndex].stageEndingEntry) {
				StartCoroutine(NextStage(stageEndDelay));
				return;
			}

			if (allEntriesUnlocked) return;

			if (!toBeUnlocked.ContainsKey(entry)) return;
			toBeUnlocked[entry] = true;
			allEntriesUnlocked = true;
			foreach (bool isUnlocked in toBeUnlocked.Values) allEntriesUnlocked &= isUnlocked;
			if (allEntriesUnlocked) {
				foreach (JournalEntryUnlock unlock in stages[curStageIndex].unlockAfterAllUnlocked) Journal.Journal.Instance.AddEntry(unlock);
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
	}
}