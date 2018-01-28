using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TextDilateTweener : MonoBehaviour {
	[SerializeField] Material textMat;
	[SerializeField] float fromVal, toVal, durationMin, durationMax;

	void Start () {
		TweenDilate();
	}

	void TweenDilate() {
		DOTween.To(() => fromVal, fd => textMat.SetFloat("_FaceDilate", fd), toVal, Random.Range(durationMin, durationMax)).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear).OnComplete(() => TweenDilate());
	}
}
