using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasso : MonoBehaviour {

	public Sprite image1;
	public Sprite image2;
	public Sprite image3;
	public Sprite image4;

	private SpriteRenderer sr;
	private float timeStart; 
	private float timeUpdate; 

	public BoxCollider2D bx;

	private bool isUsing = false;

	// Use this for initialization
	void Start () {
		sr = this.gameObject.AddComponent<SpriteRenderer> ();
		sr.sprite = image1;
		timeStart =  Time.time;
		timeUpdate = Time.time;

		bx = this.gameObject.GetComponent<BoxCollider2D> ();
		bx.offset = new Vector2(0f,0f);
		bx.size = new Vector2(0.0f,0.0f);
		bx.isTrigger = true;
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.tag == "Enemy" && PlayerControl.currentTheme == Theme.YELLOW) {
			Destroy (other.gameObject);
			PlayerControl.score += 20;
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButton ("Fire2") && !isUsing) {
			sr.sprite = image2;
			timeStart = Time.time;

			bx.offset = new Vector2(-0.5f,0.2f);
			bx.size = new Vector2(1f,1.5f);
			isUsing = true;
		}

		if (isUsing) {
			timeUpdate = Time.time; 
			if (timeUpdate - timeStart > 1f) {
				sr.sprite = image1;
				timeStart = timeUpdate;
				bx.size = new Vector2(0.5f,0.5f);
				isUsing = false;
			} else if (timeUpdate - timeStart > 0.8f) {
				sr.sprite = image2;
				bx.offset = new Vector2(-0.5f,0.2f);
				bx.size = new Vector2(1f,1.5f);
			} else if (timeUpdate - timeStart > 0.6f) {
				sr.sprite = image3;
				bx.offset = new Vector2(0f,0f);
				bx.size = new Vector2(1.1f,1.1f);
			} else if (timeUpdate - timeStart > 0.4f) {
				bx.offset = new Vector2(0f,-0.7f);
				sr.sprite = image4;
				bx.size = new Vector2(2.6f,0.5f);
			} else if (timeUpdate - timeStart > 0.2f) {
				sr.sprite = image3;
				bx.offset = new Vector2(0f,0f);
				bx.size = new Vector2(1.1f,1.1f);
			}
		}
	}
}
