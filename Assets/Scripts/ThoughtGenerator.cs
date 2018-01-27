using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GGJ.Thoughts {
    public enum EmotionalState
    {
        STAGE1,
        STAGE2,
        STAGE3
    }

    public class ThoughtGenerator : MonoBehaviour {
        public Transform thoughtPosition;
        public GameObject thoughtPrefab;
        public Sprite thoughtSprite;
        public float appearDelay;
        public float fadeDelay;
        public Transform parentTransform;
        public EmotionalState thoughtState;
        public bool autoFade;
        public float thoughtDuration;

        private GameObject thoughtGenerated;

        void GenerateThought() {
            if(thoughtGenerated != null) {
                return;
            }

            if(parentTransform == null) {
                thoughtGenerated = Instantiate(thoughtPrefab, thoughtPosition);
            } else {
                thoughtGenerated = Instantiate(thoughtPrefab, parentTransform);
            }
            thoughtGenerated.GetComponent<ThoughtScript>().PlayThought(appearDelay, thoughtSprite, autoFade);
        }

        void FadeThought() {
            if(thoughtGenerated.activeInHierarchy) {
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