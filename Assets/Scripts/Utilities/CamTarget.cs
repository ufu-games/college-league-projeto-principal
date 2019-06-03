﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamTarget : MonoBehaviour
{
    public static CamTarget instance = null;

    [ReadOnly]
    public List<PointOfInterest> m_pointsInView = new List<PointOfInterest>();
    [ReadOnly]
    public List<PointOfInterest> m_pointsOfInterest = new List<PointOfInterest>();

    CinemachineVirtualCamera m_cam = null;
    GameObject m_player = null;

    void Start()
    {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        m_cam = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>();
        m_player = GameObject.FindGameObjectWithTag("Player");
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
        foreach(GameObject go in allObjects) {
            if (go.activeInHierarchy) {
                PointOfInterest point = go.GetComponent<PointOfInterest>();
                if(point != null) {
                    m_pointsOfInterest.Add(point);
                }
            }
        }
}

    void Update()
    {
        if(m_cam.Follow != transform) {
            m_cam.Follow = transform;
        }
        Vector2 point = FindPoint();
        transform.position = new Vector3(point.x,point.y,transform.position.z);
    }

    Vector2 FindPoint() {
        Vector2 tempVect = Vector2.zero;

        float HighestX = m_player.transform.position.x;
        float HighestY = m_player.transform.position.y;
        float LowestX = m_player.transform.position.x;
        float LowestY = m_player.transform.position.y;

        foreach(PointOfInterest point in m_pointsInView) {
            if(point.ThePointOfInterest.position.x > HighestX) {
                HighestX = point.ThePointOfInterest.position.x;
            }
            if(point.ThePointOfInterest.position.y > HighestY) {
                HighestY = point.ThePointOfInterest.position.y;
            }
            if(point.ThePointOfInterest.position.x < LowestX) {
                LowestX = point.ThePointOfInterest.position.x;
            }
            if(point.ThePointOfInterest.position.y < LowestY) {
                LowestY = point.ThePointOfInterest.position.y;
            }
        }

        tempVect.x = (HighestX + LowestX)/2;
        tempVect.y = (HighestY + LowestY)/2;
        return tempVect;
    }

    public void AddPointInView(PointOfInterest point) {
        if(!m_pointsInView.Contains(point)) {
            m_pointsInView.Add(point);
        }
    }

    public void RemovePointInView(PointOfInterest point) {
        m_pointsInView.RemoveAll(p => p == point);
    }
}