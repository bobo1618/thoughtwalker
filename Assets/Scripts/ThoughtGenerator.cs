using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGJ.Journal;
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
    }

    public class ThoughtGenerator : MonoBehaviour {
        public List<JournalEntryUnlock> thoughtUnlocks;
        public Transform thoughtPosition;
        public GameObject thoughtPrefab;
        public Sprite thoughtSprite;
        public string thoughtText;
        public float appearDelay;
        public float fadeDelay;
        public Transform parentTransform;
        public EmotionalState thoughtState;
        public bool autoFade;
        public string scramble;

        public bool oneTimeShow = false;

        System.Action onFading;

        private GameObject thoughtGenerated;
        private bool shownOnce;

        public void GenerateThought(System.Action callback = null) {
            if(thoughtGenerated != null) {
                return;
            }

            if(oneTimeShow && shownOnce) {
                return;
            }

            if(callback != null) {
                onFading = callback;
            }

            if(parentTransform == null) {
                thoughtGenerated = Instantiate(thoughtPrefab, thoughtPosition);
            } else {
                thoughtGenerated = Instantiate(thoughtPrefab, parentTransform);
            }

            ThoughtData thoughtData = new ThoughtData();
            thoughtData.appearDelay = appearDelay;
            thoughtData.thoughtSprite = thoughtSprite;
            thoughtData.autoFade = autoFade;
            thoughtData.message = thoughtText;
            thoughtData.fadeDelay = fadeDelay;
            thoughtData.scramble = scramble;

            thoughtGenerated.GetComponent<ThoughtScript>().PlayThought(thoughtData).OnComplete(() => {
                if(autoFade) {
                    Invoke("FadeThought", fadeDelay);
                }
            });
        }

        public void FadeThought() {
            if(onFading != null) {
                onFading.Invoke();
            }
            if(thoughtUnlocks.Count > 0) {
                foreach(JournalEntryUnlock selectedEntry in thoughtUnlocks) {
                    Journal.Journal.Instance.AddEntry(selectedEntry);
                }
                thoughtUnlocks.Clear();
            }

            if(thoughtGenerated != null && thoughtGenerated.GetComponent<SpriteRenderer>() != null) {
                thoughtGenerated.GetComponent<ThoughtScript>().FadeThought();
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if(other.gameObject.tag == "Player") {
                GenerateThought();
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if(other.gameObject.tag == "Player") {
                CancelInvoke("FadeThought");
                if(thoughtGenerated != null) {
                    FadeThought();
                }
            }
        }
    }
}