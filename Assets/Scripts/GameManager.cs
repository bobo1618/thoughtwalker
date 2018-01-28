using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGJ.Thoughts;
using GGJ.Journal;
using GGJ.Audio;

namespace GGJ.Management {
	public class GameManager : MonoBehaviour {
		[SerializeField] List<GameStage> stages;

		public static GameManager Instance { get; private set; }
		public EmotionalState CurState {
			get { return stages[curStageIndex].state; }
		}

		public Dictionary<JournalEntry, bool> toBeUnlocked = new Dictionary<JournalEntry, bool>();

		int curStageIndex = -1;
		bool firstEverUnlock = true;

		void Start() {
			Journal.Journal.Instance.OnEntryUnlocked += OnEntryUnlocked;
			NextStage();
		}

		public void NextStage() {
			curStageIndex++;
			if (curStageIndex >= stages.Count) {
				Debug.Log("CONGLATURATION!!! YOUR WINNER! And you were eaten by a grue.");
			}
			else {
				GameStage curStage = stages[curStageIndex];
				toBeUnlocked.Clear();
				foreach (JournalEntry entry in curStage.entriesToUnlock) toBeUnlocked[entry] = false;
			}
		}

		void OnEntryUnlocked(JournalEntry entry) {
			if (!toBeUnlocked.ContainsKey(entry)) return;
			toBeUnlocked[entry] = true;
			bool stageComplete = true;
			foreach (bool isUnlocked in toBeUnlocked.Keys) stageComplete &= isUnlocked;
			if (stageComplete) {
				foreach (GameObject actGO in stages[curStageIndex].activateOnEnd) actGO.SetActive(true);
				foreach (GameObject actGO in stages[curStageIndex].deactivateOnEnd) actGO.SetActive(false);
			}
		}
	}

	[System.Serializable]
	public class GameStage {
		public EmotionalState state;
		public List<JournalEntry> entriesToUnlock;
		public List<GameObject> activateOnEnd, deactivateOnEnd;
		public Transform playerStartPoint;
	}
}