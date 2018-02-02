using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GGJ.Management;
using GGJ.JournalStuff;

public class Staircase : MonoBehaviour {
    public Transform uplink, downlink;

	static bool moving = false;	// Static so only one staircase is active at a time

	Player toTransport;
	
	// Update is called once per frame
	void Update () {
		if (GameManager.Instance.IsVideoPlaying || Journal.Instance.IsVisible) return;
		float moveDir = Input.GetAxisRaw("Vertical");
        if (moveDir != 0 && toTransport != null) TransportObject(moveDir > 0);
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") toTransport = collision.GetComponent<Player>();
    }

	public void OnTriggerExit2D(Collider2D collision) {
		if (collision.gameObject.tag == "Player" && !moving) toTransport = null;
	}

    public void TransportObject(bool moveUp) {
		if (moving || !toTransport) return;
		Transform moveTo = moveUp ? uplink : downlink;
		if (!moveTo) return;
        moving = true;
		toTransport.Fade(true);
        toTransport.transform.DOMove(moveTo.position, 1f).SetDelay(0.5f).OnComplete(() => HandleDoneTransport());
    }

    void HandleDoneTransport() {
        moving = false;
		toTransport.Fade(false);
		toTransport = null;
    }
}
