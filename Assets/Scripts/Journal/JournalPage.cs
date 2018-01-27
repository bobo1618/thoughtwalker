using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GGJ.Journal {
	public class JournalPage : MonoBehaviour {
		[SerializeField] EntryMapper[] entryMap;

		Dictionary<JournalEntry, EntryMapper> entryLookup = new Dictionary<JournalEntry, EntryMapper>();

		void Awake() {
			foreach (EntryMapper map in entryMap) entryLookup[map.entry] = map;
		}

		void Start() {
			// Reset all the entries
			foreach (EntryMapper map in entryMap) {
				map.Initialize();
				map.SetStage(-1, 0);
			}
		}

		public bool HasEntry(JournalEntry entry, ref int curStage) {
			bool retval = entryLookup.ContainsKey(entry);
			if (retval) curStage = entryLookup[entry].curStage;
			return retval;
		}

		public void SetEntryStage(EntryUnlock unlock) {
			if (!entryLookup.ContainsKey(unlock.entry) || unlock.stage >= unlock.entry.images.Count) return;
			StartCoroutine(SetEntryStateCR(unlock));
		}

		IEnumerator SetEntryStateCR(EntryUnlock unlock) {
			if (unlock.delay > 0) yield return new WaitForSeconds(unlock.delay);
			entryLookup[unlock.entry].SetStage(unlock.stage, unlock.fadeTime);
		}
	}

	[System.Serializable]
	public class EntryMapper {
		public JournalEntry entry;
		[SerializeField] Image image;
		public int curStage { get; private set; }
		Image fadeImage;

		public void Initialize() {
			fadeImage = Object.Instantiate(image, image.transform.position, image.transform.rotation, image.transform.parent);
			curStage = int.MinValue;
		}

		public void SetStage(int stage, float fadeTime) {
			if (stage == curStage || stage >= entry.images.Count) return;
			Sprite newSprite = stage < 0 ? null : entry.images[stage];
			image.sprite = newSprite;
			if (newSprite == null) image.color = new Color(1, 1, 1, 0);
			if (fadeTime > 0) {
				fadeImage.sprite = image.sprite;
				if (fadeImage.sprite == null)
					fadeImage.color = new Color(1, 1, 1, 0);
				else {
					fadeImage.color = Color.white;
					fadeImage.DOFade(0, fadeTime).SetEase(Ease.InQuad);
				}
				image.color = new Color(1, 1, 1, 0);
				if (image.sprite != null) image.DOFade(1, fadeTime).SetEase(Ease.OutQuad);
			}
		}
	}
}