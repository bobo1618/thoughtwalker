using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EntryUnlock {
	public JournalEntry entry;
	public int stage;
}


public class Journal : MonoBehaviour {
	[SerializeField] Image entryImage;
	[SerializeField] JournalEntry[] addAtStart;

	SortedList<float, Sprite> unlockedEntries = new SortedList<float, Sprite>();
	Animator animator;
	float curEntryPos = -1;
	bool isVisible = false;

	const string PARAM_VISIBLE = "Visible";

	void Awake() {
		animator = GetComponent<Animator>();
	}

	void Start() {
		foreach (JournalEntry entry in addAtStart) AddEntry(entry, 0);
	}

	public void ToggleVisibility() {
		SetVisibility(!isVisible);
	}

	public void SetVisibility(bool isOn) {
		isVisible = isOn;
		if (animator) animator.SetBool(PARAM_VISIBLE, isOn);
		else gameObject.SetActive(isOn);
	}

	public void AddEntry(JournalEntry newEntry, int stage) {
		if (stage >= 0 && stage < newEntry.images.Length) {
			unlockedEntries[newEntry.order] = newEntry.images[stage];
			ChangeEntry(newEntry.order);
		}
	}

	public void NextEntry(bool forward) {
		int newIndex = unlockedEntries.IndexOfKey(curEntryPos) + (forward ? 1 : -1);
		if (newIndex < 0 || newIndex >= unlockedEntries.Count) return;
		ChangeEntry(unlockedEntries.Keys[newIndex]);
	}

	void ChangeEntry(float entryPos) {
		if (!unlockedEntries.ContainsKey(entryPos)) return;
		curEntryPos = entryPos;
		entryImage.sprite = unlockedEntries[curEntryPos];
	}

	void Update() {
		if (Input.GetKeyUp(KeyCode.Space)) ToggleVisibility();
		if (isVisible) {
			if (Input.GetKeyUp(KeyCode.RightArrow)) NextEntry(true);
			else if (Input.GetKeyUp(KeyCode.LeftArrow)) NextEntry(false);
		}
	}
}

[CreateAssetMenu(menuName = "Scriptable Objects/Journal entry")]
public class JournalEntry : ScriptableObject {
	public float order;
	public Sprite[] images;
}
