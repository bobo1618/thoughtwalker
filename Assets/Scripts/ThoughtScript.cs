using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace GGJ.Thoughts {
    public class ThoughtScript : MonoBehaviour {
        public Transform BG;
		public Vector2 thoughtMargin;

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
			Vector3 bgScaleMult = new Vector3(1f / transform.lossyScale.x, 1f / transform.lossyScale.y, 1);
			float bgScaleFactor = 0;
			DOTween.To(() => bgScaleFactor, scale => bgScaleFactor = scale, 1f, 1f).SetEase(Ease.OutSine);

			//if (scramble == string.Empty) {
   //             textMesh.text = thoughtText;
   //             sequence.Append(textMesh.DOFade(1.0f, 1.0f).SetEase(Ease.OutQuad));
   //         } else {
            int strIndex = 0;
            textMesh.DOFade(1.0f, 1.5f);
			string scrambleText = !string.IsNullOrEmpty(scramble) ? scramble.Substring(0, thoughtText.Length) : string.Empty, temp = thoughtText;
			if (!string.IsNullOrEmpty(scrambleText)) {
				int newlineIndex = temp.IndexOf('\n');
				while (newlineIndex >= 0) {
					scrambleText = scrambleText.Substring(0, newlineIndex) + '\n' + scrambleText.Substring(newlineIndex + 1);
					newlineIndex = temp.IndexOf('\n', newlineIndex + 1);
				}
			}
            sequence.Append(DOTween.To(() => strIndex, strLen => {
                if(scrambleText != string.Empty) {
                    string newText = thoughtText.Substring(0, strLen) + "<color=#ffffff66>" + scrambleText.Substring(strLen, thoughtText.Length - strLen) + "</color>";
                    textMesh.text = newText;
                } else {
                    textMesh.text = thoughtText.Substring(0, strLen);
                }
				// Match BG size to text bounds
				BG.localScale = (Vector3.Scale(textMesh.bounds.extents, bgScaleMult) + (Vector3)thoughtMargin) * bgScaleFactor;
            }, thoughtText.Length, thoughtText.Length / speed).SetEase(Ease.Linear));
            //}
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