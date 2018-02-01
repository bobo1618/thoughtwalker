using GGJ.Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ.JournalStuff {

	[System.Serializable]
	public class JournalEntryUnlock {
		public JournalEntry entry;
		public int stage = 0;
		public float delay = 0;
	}

	[RequireComponent(typeof(Animator))]
	public class Journal : MonoBehaviour {
		[SerializeField] Text listText;

		public static Journal Instance { get; private set; }

		public delegate void EntryUnlockedEvent(JournalEntry entry);
		public EntryUnlockedEvent OnEntryUnlocked;

		public delegate void JournalVisibleEvent(bool isVisible);
		public JournalVisibleEvent OnVisible;

		public bool IsVisible {
			get { return _isVisible; }
			set {
				if (_isVisible == value) return;
				_isVisible = value;
				if (OnVisible != null) OnVisible(_isVisible);
			}
		}
		bool _isVisible = false;

		List<JournalPage> pages;
		Animator animator;
		int curPageIndex = int.MinValue;
		bool isLocked = false;
		List<System.Action> onNextVisible = new List<System.Action>();

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
			SetVisibility(!IsVisible);
		}

		public void SetVisibility(bool isOn) {
			IsVisible = isOn;
			animator.SetBool("Visible", isOn);
			SetNotifState(false);
			if (onNextVisible.Count > 0) StartCoroutine(PostVisibilityCR());
		}

		IEnumerator PostVisibilityCR() {
			yield return null;
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length * (1f - animator.GetCurrentAnimatorStateInfo(0).normalizedTime));
			foreach (var action in onNextVisible) action.Invoke();
			onNextVisible.Clear();
		}

		public void AddEntry(JournalEntryUnlock unlock) {
			for (int p = 0; p < pages.Count; p++) {
				int curStage = -1;
				JournalPage page = pages[p];
				if (!page.HasEntry(unlock.entry, ref curStage) || curStage == unlock.stage) continue;
				SetPage(p);

				System.Action whenVisible = () => {
					page.SetEntryStage(unlock);
					if (OnEntryUnlocked != null) OnEntryUnlocked(unlock.entry);
				};
				if (!IsVisible) {
					onNextVisible.Add(whenVisible);
					SetNotifState(true);
				}
				else whenVisible.Invoke();

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
			if (listText) listText.text = (curPageIndex + 1) + "/" + pages.Count;
		}

		void Update() {
			if (!isLocked && (!GameManager.Instance || !GameManager.Instance.IsVideoPlaying) && Input.GetKeyDown(KeyCode.Space)) ToggleVisibility();
			//if (isVisible) {
			//	if (Input.GetKeyUp(KeyCode.RightArrow)) TurnPage(true);
			//	else if (Input.GetKeyUp(KeyCode.LeftArrow)) TurnPage(false);
			//}
		}

		public void SetLock(bool turnOn) {
			isLocked = turnOn;
		}
	}
}