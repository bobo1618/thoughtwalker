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

		[HideInInspector] public bool destroyOnFade = false;

        Sequence sequence;
        bool autoFade;
        string thoughtText;
        float disappearDelay;
        string scramble;
		float appearDuration;

        public void Awake() {
            sequence = DOTween.Sequence();
			if (BG) BG.localScale = Vector3.zero;
		}

        // Use this for initialization
        public Sequence PlayThought(ThoughtData thoughtData) {
			gameObject.SetActive(true);

			thoughtText = thoughtData.message;
            GetComponentInChildren<TextMeshPro>().text = "";
            disappearDelay = thoughtData.fadeDelay;
            autoFade = thoughtData.autoFade;
            scramble = thoughtData.scramble;
			if (!string.IsNullOrEmpty(scramble)) while (scramble.Length < thoughtText.Length) scramble += thoughtData.scramble;
			appearDuration = thoughtText.Length / thoughtData.speed;

			TextMeshPro textMesh = GetComponentInChildren<TextMeshPro>();
			float bgScaleFactor = 0;
			if (BG) {
				BG.localScale = Vector3.zero;
				DOTween.To(() => bgScaleFactor, scale => bgScaleFactor = scale, 1f, Mathf.Min(0.67f, appearDuration)).SetEase(Ease.OutSine);
			}
			
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

			sequence.Append(DOTween.To(() => 0, strLen => {
				string newText = thoughtText.Substring(0, strLen);
				if (!string.IsNullOrEmpty(scrambleText)) {
					newText += string.Format("<color=#{0}>{1}</color>",
						scrambleColor,
						scrambleText.Substring(strLen, thoughtText.Length - strLen));
				}
				textMesh.text = newText;
				if (BG && strLen > 1) BG.localScale = (textMesh.bounds.size + (Vector3)thoughtMargin) * bgScaleFactor;
			},
			thoughtText.Length, appearDuration).SetEase(Ease.Linear));

			return sequence;
		}

		public void FadeThought() {
			//transform.parent = null;
			GetComponentInChildren<TextMeshPro>().DOFade(0.0f, 0.5f);
			Tweener tweenFade = BG.DOScale(0, 0.5f).OnComplete(() => KillThought()).SetAutoKill();
		}

		void KillThought() {
			if (destroyOnFade) GameObject.Destroy(gameObject);
			else gameObject.SetActive(false);
		}
	}
}