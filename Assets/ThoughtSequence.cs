using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ.Thoughts {
    public class ThoughtSequence : MonoBehaviour {
        public List<ThoughtGenerator> thoughtGenerators;
        
        GameObject thoughtGenerated;
        int index = 0;
        System.Action func;

        public void Update() {
            if(Input.GetKeyDown("n")) {
                if(thoughtGenerators.Count > 0) {
                    thoughtGenerators[0].FadeThought();
                }
            }
        }

        public void BeginThoughtGeneration() {
            NextThought();
        }

        void NextThought() {
            thoughtGenerators[0].GenerateThought(() => {
                thoughtGenerators.RemoveAt(0);
                Debug.Log(thoughtGenerators.Count);
                if(thoughtGenerators.Count > 0)
                    NextThought();
                else {
                    // thing at end
                }
            });
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if(other.gameObject.tag == "Player") {
                BeginThoughtGeneration();
            }
        }
    }
}
