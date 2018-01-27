using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace GGJ.Thoughts {
    public class ThoughtScript : MonoBehaviour {
        bool autoFade;
        string thoughtText;
        float disappearDelay;
        string scramble;

        // Use this for initialization
        public void PlayThought(ThoughtData thoughtData) {
            SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
            Color spriteColor = sRenderer.color;
            spriteColor.a = 0f;
            sRenderer.color = spriteColor;
            sRenderer.sprite = thoughtData.thoughtSprite;
            thoughtText = thoughtData.message;
            GetComponentInChildren<TextMeshPro>().text = "";
            disappearDelay = thoughtData.fadeDelay;
            autoFade = thoughtData.autoFade;
            scramble = thoughtData.scramble;

            sRenderer.DOFade(1.0f, 1.0f).SetDelay(thoughtData.appearDelay).OnComplete(() => ShowText()).SetAutoKill();
        }

        public void ShowText() {
            TextMeshPro textMesh = GetComponentInChildren<TextMeshPro>();

            int s = 0;
            float speed = 20;
            textMesh.DOFade(1.0f, 1.0f);
            if(autoFade) {
                DOTween.To(() => s, strLen => {
                    if(scramble != string.Empty) {
                        string newText = thoughtText.Substring(0, strLen) + scramble.Substring(strLen, thoughtText.Length - strLen);
                        Debug.Log(newText);
                        textMesh.text = newText;
                    } else {
                        textMesh.text = thoughtText.Substring(0, strLen);
                    }
                }, thoughtText.Length, thoughtText.Length / speed).SetEase(Ease.OutQuad).OnComplete(() => FadeThought()).SetAutoKill();
            } else {
                DOTween.To(() => s, strLen => {
                    if(scramble != string.Empty) {
                        string newText = thoughtText.Substring(0, strLen) + scramble.Substring(strLen, thoughtText.Length - strLen);
                        textMesh.text = newText;
                    } else {
                        textMesh.text = thoughtText.Substring(0, strLen);
                    }
                }, thoughtText.Length, thoughtText.Length / speed).SetEase(Ease.OutQuad).SetAutoKill();
            }
        }

        public void FadeThought() {
            transform.parent = null;
            SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
            GetComponentInChildren<TextMeshPro>().DOFade(0.0f, 0.5f).SetDelay(disappearDelay);
            Tweener tweenFade = sRenderer.DOFade(0.0f, 0.5f).SetDelay(disappearDelay).OnComplete(() => KillThought()).SetAutoKill();
        }

        void KillThought() {
            GameObject.Destroy(gameObject);
        }
    }
}