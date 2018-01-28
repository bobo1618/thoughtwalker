using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MaterialRandomizer : MonoBehaviour {
	[SerializeField] Material matToRandomize;
	[SerializeField] float updateInterval = 0.25f, fadeMin, fadeMax;

	Tweener fadeTweener = null;

	void OnEnable() {
		InvokeRepeating("Randomize", updateInterval, updateInterval);
		if (!Mathf.Approximately(fadeMin, fadeMax)) RandomizeFade();
	}

	void RandomizeFade() {
		float f = matToRandomize.GetFloat("_FadeValue");
		fadeTweener = DOTween.To(() => f, fadeValue => matToRandomize.SetFloat("_FadeValue", fadeValue), Random.Range(fadeMin, fadeMax), updateInterval)
			.OnComplete(() => RandomizeFade());
	}

	void Randomize() {
		matToRandomize.SetVector("_TexOffset", new Vector2(Random.Range(0, 1f), Random.Range(0, 1f)));
	}

	void OnDisable() {
		if (fadeTweener != null) {
			fadeTweener.Kill();
			fadeTweener = null;
		}
		CancelInvoke("Randomize");
	}
}
