﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfScene : MonoBehaviour, IInteractable {

	public void Interact(){
		LevelManagement.LevelManager.instance.LoadLevel("Hub");
	}
}
