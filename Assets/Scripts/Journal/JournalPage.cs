using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GGJ.Journal {
	public class JournalPage : MonoBehaviour {
		[SerializeField] JournalEntryMapper[] entryMap;

		Dictionary<JournalEntry, JournalEntryMapper> entryLookup = new Dictionary<JournalEntry, JournalEntryMapper>();

		void Awake() {
			foreach (JournalEntryMapper map in entryMap) entryLookup[map.entry] = map;
		}

		void Start() {
			// Reset all the entries
			foreach (JournalEntryMapper map in entryMap) {
				map.Initialize();
				map.SetStage(-1);
			}
		}

		public bool HasEntry(JournalEntry entry, ref int curStage) {
			bool retval = entryLookup.ContainsKey(entry);
			if (retval) curStage = entryLookup[entry].curStage;
			return retval;
		}

		public void SetEntryStage(JournalEntryUnlock unlock) {
			if (!entryLookup.ContainsKey(unlock.entry) || unlock.stage >= unlock.entry.stages.Count) return;
			StartCoroutine(SetEntryStateCR(unlock));
		}

		IEnumerator SetEntryStateCR(JournalEntryUnlock unlock) {
			if (unlock.entry.stages[unlock.stage].delay > 0) yield return new WaitForSeconds(unlock.entry.stages[unlock.stage].delay);
			entryLookup[unlock.entry].SetStage(unlock.stage);
		}
	}

	[System.Serializable]
	public class JournalEntryMapper {
		public JournalEntry entry;
		[SerializeField] Image image;
		public int curStage { get; private set; }
		Image fadeImage;

		public void Initialize() {
			fadeImage = Object.Instantiate(image, image.transform.position, image.transform.rotation, image.transform.parent);
			fadeImage.color = new Color(1, 1, 1, 0);
			curStage = int.MinValue;
		}

		public void SetStage(int stageIndex) {
			if (stageIndex == curStage || stageIndex >= entry.stages.Count) return;
			JournalEntry.EntryStage stage = stageIndex < 0 ? null : entry.stages[stageIndex];
			float fadeTime = stage == null ? 0 : Mathf.Max(0, stage.fadeTime);

			fadeImage.sprite = image.sprite;
			if (fadeImage.sprite == null) fadeImage.color = new Color(1, 1, 1, 0);
			image.sprite = stage == null ? null : stage.image;
			if (image.sprite == null) image.color = new Color(1, 1, 1, 0);

			if (fadeTime > 0) {
				if (fadeImage.sprite != null) {
					fadeImage.color = Color.white;
					fadeImage.DOFade(0, fadeTime).SetEase(Ease.InQuad);
				}
				if (image.sprite != null) {
					image.color = new Color(1, 1, 1, 0);
					image.DOFade(1, fadeTime).SetEase(Ease.OutQuad);
				}
			}
		}
	}
}