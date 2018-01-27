using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GGJ.Thoughts {
    public class ThoughtScript : MonoBehaviour {
        bool autoFade;

        public void Update() {
            SpriteRenderer sRendereer = GetComponent<SpriteRenderer>();
            if(transform.localScale.x < 0) {
                sRendereer.flipX = true;
            } else {
                sRendereer.flipX = false;
            }
        }

        // Use this for initialization
        public void PlayThought(float timeAppear, Sprite thoughtSprite, bool autoFade) {
            SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
            Color spriteColor = sRenderer.color;
            spriteColor.a = 0f;
            sRenderer.color = spriteColor;
            sRenderer.sprite = thoughtSprite;

            if(autoFade) {
                sRenderer.DOFade(1.0f, timeAppear).OnComplete(() => FadeThought());
            } else {
                sRenderer.DOFade(1.0f, timeAppear);
            }
        }

        public void FadeThought() {
            SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
            Tweener tweenFade = sRenderer.DOFade(0.0f, 0.5f).OnComplete(() => KillThought());
        }

        void KillThought() {
            GameObject.Destroy(gameObject);
        }
    }
}