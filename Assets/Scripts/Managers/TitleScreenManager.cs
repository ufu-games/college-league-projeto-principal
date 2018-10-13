﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour {

	public AudioClip pressClip;

	private IEnumerator LoadNext() {
		yield return new WaitForSeconds(1.0f);
		LevelManagement.LevelManager.instance.LoadNextLevel();
	}
	
	void Update () {
		if(Input.GetButtonDown("Submit")) {
			if(pressClip) SoundManager.instance.PlaySfx(pressClip);
			StartCoroutine(LoadNext());
		}
	}
}