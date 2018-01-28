using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MaterialRandomizer : MonoBehaviour {
	[SerializeField] Material matToRandomize;
	[SerializeField] float updateInterval = 0.25f, fadeMin, fadeMax;

	void Start() {
		InvokeRepeating("Randomize", updateInterval, updateInterval);
		if (!Mathf.Approximately(fadeMin, fadeMax)) RandomizeFade();
	}

	void RandomizeFade() {
		float f = matToRandomize.GetFloat("_FadeValue");
		DOTween.To(() => f, fadeValue => matToRandomize.SetFloat("_FadeValue", fadeValue), Random.Range(fadeMin, fadeMax), updateInterval)
			.OnComplete(() => RandomizeFade());
	}

	void Randomize() {
		matToRandomize.SetTextureOffset("_MainTex", new Vector2(Random.Range(0, 1f), Random.Range(0, 1f)));
	}
}
