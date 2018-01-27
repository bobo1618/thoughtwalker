using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomOverlay : MonoBehaviour {
    private bool isEntered;
    private bool isOccupied;
	
	// Update is called once per frame
	void Update () {
        float fadeAmount = 1f;

        if(isEntered) {
            fadeAmount = 0.5f;
        }

        if(isOccupied) {
            fadeAmount = 0f;
        }

        SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
        if(sRenderer.color.a < fadeAmount || sRenderer.color.a > fadeAmount) {
            DoFade(fadeAmount);
        }
    }

    public void DoFade(float fadeAmount) {
        GetComponent<SpriteRenderer>().DOFade(fadeAmount, 1.0f);
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player") {
            isEntered = true;
            isOccupied = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player") {
            isOccupied = false;
        }
    }
}
