using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace GGJ.Thoughts {
    public class ThoughtScript : MonoBehaviour {
        public Transform BG;
        Sequence sequence;
        bool autoFade;
        string thoughtText;
        float disappearDelay;
        string scramble;
        float speed;

        public void Awake() {
            sequence = DOTween.Sequence();
            if (BG) BG.localScale = Vector3.zero;
        }

        // Use this for initialization
        public Sequence PlayThought(ThoughtData thoughtData) {
            thoughtText = thoughtData.message;
            GetComponentInChildren<TextMeshPro>().text = "";
            disappearDelay = thoughtData.fadeDelay;
            autoFade = thoughtData.autoFade;
            scramble = thoughtData.scramble;
            speed = thoughtData.speed;
			
            TextMeshPro textMesh = GetComponentInChildren<TextMeshPro>();
			Vector3 bgScaleMult = new Vector3(1f / transform.localScale.x, 1f / transform.localScale.y, 1);
			float bgScaleFactor = 0;
			DOTween.To(() => bgScaleFactor, s => bgScaleFactor = s, 1f, 1f).SetEase(Ease.OutSine);

			if (scramble == string.Empty) {
                textMesh.text = thoughtText;
                sequence.Append(textMesh.DOFade(1.0f, 1.0f).SetEase(Ease.OutQuad));
            } else {
                int s = 0;
                textMesh.DOFade(1.0f, 1.5f);
                sequence.Append(DOTween.To(() => s, strLen => {
                    if(scramble != string.Empty) {
                        string newText = thoughtText.Substring(0, strLen) + "<color=blue>" + scramble.Substring(strLen, thoughtText.Length - strLen) + "</color>";
                        textMesh.text = newText;
                    } else {
                        textMesh.text = thoughtText.Substring(0, strLen);
                    }
					// Match BG size to text bounds
					BG.localScale = (Vector3.Scale(textMesh.bounds.extents, bgScaleMult) + Vector3.one) * bgScaleFactor;
                }, thoughtText.Length, thoughtText.Length / speed).SetEase(Ease.Linear));
            }
            return sequence;
        }

        public void FadeThought() {
            transform.parent = null;

            GetComponentInChildren<TextMeshPro>().DOFade(0.0f, 0.5f);
            Tweener tweenFade = BG.DOScale(0, 0.5f).OnComplete(() => KillThought()).SetAutoKill();
        }

        void KillThought() {
            GameObject.Destroy(gameObject);
        }
    }
}