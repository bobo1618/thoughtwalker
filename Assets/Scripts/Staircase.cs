using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Staircase : MonoBehaviour {
    public Transform linkedStaircase;
    private GameObject toTransport;
    private bool moving = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(toTransport != null) {
            if(Input.GetKey("w")) {
                TransportObject();
            }
        }
    }

    public void OnTriggerStay2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player") {
            toTransport = collision.gameObject;
        }
    }

    public void OnTriggerExit2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player" && !moving) {
            toTransport = null;
        }
    }

    public void TransportObject() {
        moving = true;
        //toTransport.GetComponent<SpriteRenderer>().enabled = false;
        toTransport.GetComponent<Animator>().SetBool("Fade", true);
        BeginTransport();
    }

    void BeginTransport() {
        toTransport.GetComponent<Collider2D>().enabled = false;
        toTransport.GetComponent<Rigidbody2D>().simulated = false;
        toTransport.transform.DOMove(linkedStaircase.transform.position, 1f).SetDelay(0.5f).OnComplete(() => HandleDoneTransport());
    }

    void HandleDoneTransport() {
        moving = false;
        toTransport.GetComponent<Animator>().SetBool("Fade", false);
        toTransport.GetComponent<Collider2D>().enabled = true;
        toTransport.GetComponent<Rigidbody2D>().simulated = true;
        toTransport = null;
    }
}
