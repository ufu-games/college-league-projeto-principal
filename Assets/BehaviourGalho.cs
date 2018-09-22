﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourGalho : MonoBehaviour {
	public GameObject Tree;
	private Vector3 RotationCenter;
	public float normalSpeed;
	public float angrySpeed;
	private float speed;
	public float damage;
	public int orientation = 1;
	public Vector3 offset;
	public float fastSpeedTime;

    // Use this for initialization
    void Start () {
		this.RotationCenter = this.Tree.transform.position + this.offset;
		this.speed = normalSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(this.RotationCenter, new Vector3(0,0,1), this.orientation*this.speed*Time.deltaTime);
	}

	public void SetHighSpeed(){
		this.speed = angrySpeed;
	}

	private IEnumerator RotationTime(){
		yield return new WaitForSeconds (this.fastSpeedTime);
		this.speed = normalSpeed;
	}
}
