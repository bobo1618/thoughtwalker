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

    public class ThoughtGenerator : MonoBehaviour {
        public JournalEntryUnlock[] thoughtUnlocks;
        public Transform thoughtPosition;
        public GameObject thoughtPrefab;
        public Sprite thoughtSprite;
        public float appearDelay;
        public float fadeDelay;
        public Transform parentTransform;
        public EmotionalState thoughtState;
        public bool autoFade;
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

            if(parentTransform == null) {
                thoughtGenerated = Instantiate(thoughtPrefab, thoughtPosition);
            } else {
                thoughtGenerated = Instantiate(thoughtPrefab, parentTransform);
            }

            if(thoughtUnlocks.Length > 0) {
                foreach(JournalEntryUnlock selectedEntry in thoughtUnlocks) {
                    // ROHAN OVER HERE
                    //selectedEntry.
                }
            }

            thoughtGenerated.GetComponent<ThoughtScript>().PlayThought(appearDelay, thoughtSprite, autoFade, "example over here MUAHAHAHAHAHAHA");
            shownOnce = true;
        }

        void FadeThought() {
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
                Invoke("FadeThought", fadeDelay);
            }
        }
    }
}