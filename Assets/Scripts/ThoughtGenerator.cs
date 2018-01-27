using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGJ.Journal;

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

        private GameObject thoughtGenerated;
        private bool shownOnce;

        void GenerateThought() {
            if(thoughtGenerated != null) {
                return;
            }

            if(oneTimeShow && shownOnce) {
                return;
            }

            if(thoughtUnlocks.Count > 0) {
                foreach(JournalEntryUnlock selectedEntry in thoughtUnlocks) {
                    Journal.Journal.Instance.AddEntry(selectedEntry);
                    thoughtUnlocks.Remove(selectedEntry);
                }
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

            thoughtGenerated.GetComponent<ThoughtScript>().PlayThought(thoughtData);
            shownOnce = true;
        }

        public void FadeThought() {
            if(thoughtGenerated != null && thoughtGenerated.GetComponent<SpriteRenderer>() != null) {
                thoughtGenerated.GetComponent<ThoughtScript>().FadeThought();
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if(other.gameObject.tag == "Player") {
                Invoke("GenerateThought", 0f); // For now
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if(other.gameObject.tag == "Player") {
                Invoke("FadeThought", 0f);
            }
        }
    }
}