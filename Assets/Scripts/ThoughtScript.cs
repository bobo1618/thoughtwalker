using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace GGJ.Thoughts {
    public class ThoughtScript : MonoBehaviour {
        bool autoFade;
        string thoughtText;

        // Use this for initialization
        public void PlayThought(float timeAppear, Sprite thoughtSprite, bool autoFade, string entry) {
            SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
            Color spriteColor = sRenderer.color;
            spriteColor.a = 0f;
            sRenderer.color = spriteColor;
            sRenderer.sprite = thoughtSprite;
            thoughtText = entry;
            GetComponentInChildren<TextMeshPro>().text = "";

            sRenderer.DOFade(1.0f, timeAppear).OnComplete(() => ShowText()).SetAutoKill();
        }

        public void ShowText() {
            TextMeshPro textMesh = GetComponentInChildren<TextMeshPro>();

            int s = 0;
            float speed = 10;
            textMesh.DOFade(1.0f, 1.0f);
            DOTween.To(() => s, strLen => {
                textMesh.text = thoughtText.Substring(0, strLen);
            }, thoughtText.Length, thoughtText.Length / speed).OnComplete(() => FadeThought()).SetAutoKill();
        }

        public void FadeThought() {
            SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
            GetComponentInChildren<TextMeshPro>().DOFade(0.0f, 0.5f).SetDelay(4f);
            Tweener tweenFade = sRenderer.DOFade(0.0f, 0.5f).SetDelay(5f).OnComplete(() => KillThought()).SetAutoKill();
        }

        void KillThought() {
            GameObject.Destroy(gameObject);
        }
    }
}