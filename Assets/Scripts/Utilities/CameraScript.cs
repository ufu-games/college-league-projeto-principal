﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
	public static CameraScript instance;
	private const float maxAngle = 2.5f;
	private const float maxOffset = 1f;

	Vector3 initialRotation;
	Cinemachine.CinemachineVirtualCamera m_virtualCamera;
	Transform m_virtualCameraTransform;
	Vector3 m_originalCameraPosition;
	Vector3 m_originalCameraRotation;

	/* Screen Shake */
	public float cameraTrauma;

	private OrthographicOverride m_ortographicOverride;

	void Awake() {
		if(instance == null) {
			instance = this;
		}
	}

	void Start() {
		m_ortographicOverride = FindObjectOfType<OrthographicOverride>();
		m_virtualCamera = GameObject.FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
		m_virtualCameraTransform = m_virtualCamera.transform;
		cameraTrauma = 0f;
	}

	public void ForceCameraSize(float size) {
		m_ortographicOverride.enabled = false;
		m_virtualCamera.m_Lens.OrthographicSize = size;
	}

	public void UnforceCameraSize() {
		m_ortographicOverride.enabled = true;
	}

	void Update() {
		if(cameraTrauma <= 0f) {
			return;
		}

		cameraTrauma -= Time.deltaTime;
		cameraTrauma = Mathf.Clamp(cameraTrauma, 0, 1f);

		if(cameraTrauma <= 0f) {
			this.transform.position = m_originalCameraPosition;
			this.transform.localEulerAngles = m_originalCameraRotation;
		}

		float angle = maxAngle * (cameraTrauma * cameraTrauma) * Random.Range(-1f, 1f);
		float offsetX = maxOffset * (cameraTrauma * cameraTrauma) * Random.Range(-1f, 1f);
		float offsetY = maxOffset * (cameraTrauma * cameraTrauma) * Random.Range(-1f, 1f);

		Vector3 t_position = transform.position;
		Vector3 t_rotation = transform.localEulerAngles;
		t_position.x += offsetX;
		t_position.y += offsetY;
		t_rotation.z += angle;

		this.transform.position = t_position;	
		this.transform.localEulerAngles = t_rotation;
	}

	public void AddTraumaToCamera(float amount) {
		Debug.LogWarning("Adding Trauma!");
		if(cameraTrauma <= 0f) {
			m_originalCameraPosition = this.transform.position;
			m_originalCameraRotation = this.transform.localEulerAngles;

			cameraTrauma = amount;
		} else {
			cameraTrauma += amount;
		}
	}

}
