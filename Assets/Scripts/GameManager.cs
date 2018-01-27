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
		}

		public void NextStage() {
			curStageIndex++;
			if (curStageIndex >= stages.Count) {
				// CONGLATURATION!!! A WINNER IS YOU
			}
			else {
				GameStage curStage = stages[curStageIndex];
			}
		}

		void OnEntryUnlock(JournalEntry entry) {
			
		}
	}

	[System.Serializable]
	public class GameStage {
		public EmotionalState state;
		public List<JournalEntry> entriesToUnlock;
	}
}