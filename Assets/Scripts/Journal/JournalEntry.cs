using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ.JournalStuff {
	[CreateAssetMenu(menuName = "Scriptable Objects/Journal entry")]
	public class JournalEntry : ScriptableObject {
		[System.Serializable]
		public class EntryStage {
			public Sprite image;
			public float fadeTime = 1, shakeStrength = 0;
		}
		public List<EntryStage> stages;
	}
}