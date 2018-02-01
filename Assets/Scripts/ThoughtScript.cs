using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace GGJ.Thoughts {
    public class ThoughtScript : MonoBehaviour {
        public Transform BG;
		public string scrambleColor = "ffffff88";
		public Vector2 thoughtMargin;

        Sequence sequence;
        bool autoFade;
        string thoughtText;
        float disappearDelay;
        string scramble;
		float appearDuration;

        public void Awake() {
            sequence = DOTween.Sequence();
        }

        // Use this for initialization
        public Sequence PlayThought(ThoughtData thoughtData) {
            thoughtText = thoughtData.message;
            GetComponentInChildren<TextMeshPro>().text = "";
            disappearDelay = thoughtData.fadeDelay;
            autoFade = thoughtData.autoFade;
            scramble = thoughtData.scramble;
			if (!string.IsNullOrEmpty(scramble)) while (scramble.Length < thoughtText.Length) scramble += thoughtData.scramble;
			appearDuration = thoughtText.Length / thoughtData.speed;

			TextMeshPro textMesh = GetComponentInChildren<TextMeshPro>();
			float bgScaleFactor = 0;
			DOTween.To(() => bgScaleFactor, scale => bgScaleFactor = scale, 1f, Mathf.Min(0.67f, appearDuration)).SetEase(Ease.OutSine);

			//if (scramble == string.Empty) {
   //             textMesh.text = thoughtText;
   //             sequence.Append(textMesh.DOFade(1.0f, 1.0f).SetEase(Ease.OutQuad));
   //         } else {
            int strIndex = 0;
            textMesh.DOFade(1.0f, 1.5f);
			string scrambleText = !string.IsNullOrEmpty(scramble) ? scramble.Substring(0, thoughtText.Length) : string.Empty;
			string temp = thoughtText;
			if (!string.IsNullOrEmpty(scrambleText)) {
				int newlineIndex = temp.IndexOf('\n');
				while (newlineIndex >= 0) {
					scrambleText = scrambleText.Substring(0, newlineIndex) + '\n' + scrambleText.Substring(newlineIndex + 1);
					newlineIndex = temp.IndexOf('\n', newlineIndex + 1);
				}
			}
			sequence.Append(DOTween.To(() => strIndex, strLen => {
				if (scrambleText != string.Empty) {
					string newText = string.Format("{0}<color=#{1}>{2}</color>",
						thoughtText.Substring(0, strLen),
						scrambleColor,
						scrambleText.Substring(strLen, thoughtText.Length - strLen));
					textMesh.text = newText;
				}
				else {
					textMesh.text = thoughtText.Substring(0, strLen);
				}
				if (BG) BG.localScale = (textMesh.bounds.size + (Vector3)thoughtMargin) * bgScaleFactor;
			}, thoughtText.Length, appearDuration).SetEase(Ease.Linear).OnComplete(() => {
				if (thoughtData.endAppearPFX) {
					thoughtData.endAppearPFX.transform.position = transform.position;
					if (BG) thoughtData.endAppearPFX.transform.localScale = BG.lossyScale;
					thoughtData.endAppearPFX.Play();
				}
			}));
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