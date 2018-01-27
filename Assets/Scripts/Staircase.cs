using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Staircase : MonoBehaviour {
    public Transform linkedStaircase;
    private GameObject toTransport;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerStay2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player") {
            if(Input.GetKeyDown("w")) {
                toTransport = collision.gameObject;
                TransportObject();
            }
        }
    }

    public void TransportObject() {
        //toTransport.GetComponent<SpriteRenderer>().enabled = false;
        toTransport.GetComponent<SpriteRenderer>().DOFade(0f, 0.5f).OnComplete(() => BeginTransport());
    }

    void BeginTransport() {
        toTransport.GetComponent<Collider2D>().enabled = false;
        toTransport.transform.DOMove(linkedStaircase.transform.position, 1f).OnComplete(() => HandleDoneTransport());
    }

    void HandleDoneTransport() {
        toTransport.GetComponent<SpriteRenderer>().DOFade(1f, 0.5f);
        toTransport.GetComponent<Collider2D>().enabled = true;
    }
}
