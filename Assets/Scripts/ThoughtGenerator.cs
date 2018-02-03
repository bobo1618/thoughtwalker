using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGJ.JournalStuff;
using DG.Tweening;

namespace GGJ.Thoughts {
    public enum EmotionalState
    {
        STAGE1,
        STAGE2,
        STAGE3
    }

    public class ThoughtData {
        public string scramble;
        public Sprite thoughtSprite;
        public float appearDelay;
        public float fadeDelay;
        public string message;
        public bool autoFade;
        public float speed;
    }

    public class ThoughtGenerator : MonoBehaviour {
        public List<JournalEntryUnlock> thoughtUnlocks;
        public Transform thoughtPosition;
        public GameObject thoughtPrefab;
        public Sprite thoughtSprite;
        [Multiline]
        public string thoughtText;
        public float appearDelay;
        public float fadeDelay;
        public Transform parentTransform;
        public EmotionalState thoughtState;
        public bool autoFade;
        [Multiline]
        public string scramble;
		public float appearSpeed;
		public float lockPlayerTime = 0;

        public bool oneTimeShow = false;
		bool shownOnce = false;

        System.Action onFading;

        private GameObject thoughtGenerated;

        public void SetCallback(System.Action callback = null) {
            if(callback != null) {
                onFading = callback;
            }
        }

		public void GenerateThought() {
			if (oneTimeShow && shownOnce) return;

			ThoughtData thoughtData = new ThoughtData() {
				appearDelay = appearDelay,
				thoughtSprite = thoughtSprite,
				autoFade = autoFade,
				message = thoughtText,
				fadeDelay = fadeDelay,
				scramble = scramble,
				speed = appearSpeed
			};

			if (!thoughtGenerated) thoughtGenerated = Instantiate(thoughtPrefab, parentTransform ? parentTransform : thoughtPosition);

			ThoughtScript thought = thoughtGenerated.GetComponent<ThoughtScript>();
			if (thought) {
				thought.PlayThought(thoughtData).OnComplete(() => {
					shownOnce = true;
					if (oneTimeShow) thought.destroyOnFade = true;
					if (thoughtUnlocks.Count > 0) {
						foreach (JournalEntryUnlock selectedEntry in thoughtUnlocks) {
							Journal.Instance.AddEntry(selectedEntry);
						}
						thoughtUnlocks.Clear();
					}
					if (autoFade) Invoke("FadeThought", fadeDelay);
				});
			}
			else {
				shownOnce = true;
			}

			Player.Instance.LockMovement(lockPlayerTime);
			lockPlayerTime = 0;
		}

        public void FadeThought() {
            if(onFading != null) {
                onFading.Invoke();
            }
            if(thoughtGenerated != null && thoughtGenerated.GetComponent<SpriteRenderer>() != null) {
                thoughtGenerated.GetComponent<ThoughtScript>().FadeThought();
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if(other.gameObject.tag == "Player") {
                Invoke("GenerateThought", appearDelay);
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if(other.gameObject.tag == "Player") {
                CancelInvoke("GenerateThought");
                if (thoughtGenerated && thoughtGenerated.activeSelf) {
                    Invoke("FadeThought", fadeDelay);
                }
            }
        }
    }
}