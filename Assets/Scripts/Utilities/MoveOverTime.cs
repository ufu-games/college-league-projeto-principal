﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOverTime : MonoBehaviour {
    [Range(1,100)]
    public float movingDistance = 5f;
    private Vector3 m_initialPosition;
    private Vector3 m_initialScale;

    private float m_fixedDeltaTimeAccumulated;

    void Start() {
        m_initialPosition = this.transform.position;
        m_initialScale = this.transform.localScale;
        m_fixedDeltaTimeAccumulated = 0f;
    }

    void FixedUpdate() {
        Vector3 tempPosition = m_initialPosition;
        m_fixedDeltaTimeAccumulated += (Time.fixedDeltaTime / 24f);
        tempPosition.x += Mathf.Cos(m_fixedDeltaTimeAccumulated) * movingDistance;
        this.transform.position = tempPosition;
    }
}
