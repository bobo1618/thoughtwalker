using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ.Journal {

	[System.Serializable]
	public class JournalEntryUnlock {
		public JournalEntry entry;
		public int stage;
	}

	[RequireComponent(typeof(Animator))]
	public class Journal : MonoBehaviour {
		List<JournalPage> pages;
		Animator animator;
		int curPageIndex = 0;
		bool isVisible = false;

		const string PARAM_VISIBLE = "Visible";

		public static Journal Instance { get; private set; }

		void Awake() {
			Instance = this;
			animator = GetComponent<Animator>();
			pages = new List<JournalPage>(GetComponentsInChildren<JournalPage>(true));
		}

		void Start() {
			foreach (JournalPage page in pages) page.gameObject.SetActive(false);
		}

		public void ToggleVisibility() {
			SetVisibility(!isVisible);
		}

		public void SetVisibility(bool isOn, System.Action doAfter = null) {
			isVisible = isOn;
			animator.SetBool(PARAM_VISIBLE, isOn);
			if (doAfter != null) StartCoroutine(PostVisibilityCR(doAfter));
		}

		IEnumerator PostVisibilityCR(System.Action doAfter) {
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
			doAfter.Invoke();
		}

		public void AddEntry(JournalEntryUnlock unlock) {
			for (int p = 0; p < pages.Count; p++) {
				int curStage = -1;
				JournalPage page = pages[p];
				if (!page.HasEntry(unlock.entry, ref curStage) || curStage == unlock.stage) continue;
				SetPage(p);
				SetVisibility(true, () => { page.SetEntryStage(unlock); });
				break;
			}
		}

		public void TurnPage(bool nextPage) {
			bool hasPage = SetPage(curPageIndex + (nextPage ? 1 : -1));
		}

		bool SetPage(int index) {
			if (index == curPageIndex || index < 0 || index >= pages.Count) return false;
			pages[curPageIndex].gameObject.SetActive(false);
			curPageIndex = index;
			pages[curPageIndex].gameObject.SetActive(true);
			return true;
		}

		void Update() {
			if (Input.GetKeyUp(KeyCode.Space)) ToggleVisibility();
			if (isVisible) {
				if (Input.GetKeyUp(KeyCode.RightArrow)) TurnPage(true);
				else if (Input.GetKeyUp(KeyCode.LeftArrow)) TurnPage(false);
			}
		}
	}

	[CreateAssetMenu(menuName = "Scriptable Objects/Journal entry")]
	public class JournalEntry : ScriptableObject {
		[System.Serializable]
		public class EntryStage {
			public Sprite image;
			public float delay = 0, fadeTime = 1;
		}
		public List<EntryStage> stages;
	}
}