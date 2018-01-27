using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ.Journal {

	[System.Serializable]
	public class JournalEntryUnlock {
		public JournalEntry entry;
		public int stage;
		public float delay;
	}

	[RequireComponent(typeof(Animator))]
	public class Journal : MonoBehaviour {
		[SerializeField] Text listText;

		public static Journal Instance { get; private set; }

		public delegate void EntryUnlockedEvent(JournalEntry entry);
		public EntryUnlockedEvent OnEntryUnlocked;

		List<JournalPage> pages;
		Animator animator;
		int curPageIndex = int.MinValue;
		bool isVisible = false;
		System.Action onNextVisible = null;

		void Awake() {
			Instance = this;
			animator = GetComponent<Animator>();
			pages = new List<JournalPage>(GetComponentsInChildren<JournalPage>(true));
		}

		void Start() {
			foreach (JournalPage page in pages) page.gameObject.SetActive(false);
		}

		public void SetNotifState(bool isOn) {
			animator.SetBool("Notif", isOn);
		}

		public void ToggleVisibility() {
			SetVisibility(!isVisible);
		}

		public void SetVisibility(bool isOn) {
			isVisible = isOn;
			animator.SetBool("Visible", isOn);
			SetNotifState(false);
			if (onNextVisible != null) StartCoroutine(PostVisibilityCR());
		}

		IEnumerator PostVisibilityCR() {
			yield return null;
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length * (1f - animator.GetCurrentAnimatorStateInfo(0).normalizedTime));
			onNextVisible.Invoke();
			onNextVisible = null;
		}

		public void AddEntry(JournalEntryUnlock unlock) {
			for (int p = 0; p < pages.Count; p++) {
				int curStage = -1;
				JournalPage page = pages[p];
				if (!page.HasEntry(unlock.entry, ref curStage) || curStage == unlock.stage) continue;
				SetPage(p);
				SetNotifState(true);
				onNextVisible = () => page.SetEntryStage(unlock);
				if (OnEntryUnlocked != null) OnEntryUnlocked(unlock.entry);
				break;
			}
		}

		public void TurnPage(bool nextPage) {
			SetPage(curPageIndex + (nextPage ? 1 : -1));
		}

		void SetPage(int index) {
			if (index == curPageIndex || index < 0 || index >= pages.Count) return;
			if (curPageIndex >= 0 && curPageIndex < pages.Count) pages[curPageIndex].gameObject.SetActive(false);
			curPageIndex = index;
			pages[curPageIndex].gameObject.SetActive(true);
			listText.text = (curPageIndex + 1) + "/" + pages.Count;
		}

		void Update() {
			if (Input.GetKeyUp(KeyCode.Space)) ToggleVisibility();
			if (isVisible) {
				if (Input.GetKeyUp(KeyCode.RightArrow)) TurnPage(true);
				else if (Input.GetKeyUp(KeyCode.LeftArrow)) TurnPage(false);
			}
		}

		// FOR TESTING ONLY
		[SerializeField]
		JournalEntryUnlock[] unlockList;
		public void AddEntryIndex(int index) {
			AddEntry(unlockList[index]);
		}
	}

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