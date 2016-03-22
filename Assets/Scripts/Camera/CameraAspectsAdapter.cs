﻿using UnityEngine;
using System.Collections;

public class CameraAspectsAdapter : MonoBehaviour {


    void Awake() {
        Camera camera = Camera.main;
        float screenHeight = 2f * camera.orthographicSize;
        float screenwidth = screenHeight * camera.aspect;

        Debug.Log(screenHeight);
        Debug.Log(screenwidth);

        Debug.Log(16 / screenwidth);

        gameObject.GetComponent<Camera>().orthographicSize = camera.orthographicSize * 16 / screenwidth;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
