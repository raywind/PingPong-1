﻿using UnityEngine;
using System.Collections;

public class PlayerControllerMultiScript : MonoBehaviour {

    public GameObject ballPrefab;
	public GameObject playerCamera;
	private GameObject initialPlayerCamera;
    private GameObject initialBall;

    public float maxSpeed;
    private float sidePanelWidth;

    public float width;
    public float height;

    private NetworkView networkView;

    private float syncDelay;
    private float lastSyncTime;
    private float syncTime;
    private Vector2 syncStartPosition;
    private Vector2 syncEndPosition;

    void Awake()
    {
        
		RotateToTarge (Vector2.zero);

    }

	public void RotateToTarge(Vector2 targetDirection){
		targetDirection = targetDirection - (Vector2)transform.position;
		float angle = Mathf.Atan2 (targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
		transform.localRotation = Quaternion.AngleAxis (angle - 90, new Vector3 (0, 0, 1));
	}

    // Use this for initialization
    void Start()
    {

        Network.sendRate = 100;

        Sprite sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        sidePanelWidth = GameObject.Find("WallLeft").GetComponent<Renderer>().bounds.size.x;

        Debug.Log(sidePanelWidth);

        networkView = GetComponent<NetworkView>();

        width = sprite.bounds.size.x;
        height = sprite.bounds.size.y;

        //InitializeBall();
		InitializePlayerCamera ();
		if(networkView.isMine){
			initialPlayerCamera.GetComponent<Camera>().enabled = true;
		}
		else{
			initialPlayerCamera.GetComponent<Camera>().enabled = false;
		}
    }

    // Update is called once per frame


     void Update()
    {
        Debug.Log("SEND RATE = " + Network.sendRate);
        Move();
        DoInitialFire();

    }

    public void InitializeBall()
    {
        // initialBall = (GameObject)Instantiate(ballPrefab);

        float intialBallDeviation = height;
        if (gameObject.transform.position.y > 0)
        {
            intialBallDeviation = -intialBallDeviation;
        }

        Vector2 spawnPosition = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + intialBallDeviation);

        initialBall = (GameObject)Network.Instantiate(ballPrefab, spawnPosition, transform.rotation, 1);
		initialBall.transform.parent = gameObject.transform;

    }

	void InitializePlayerCamera () {
		initialPlayerCamera = (GameObject)Instantiate (playerCamera);
		initialPlayerCamera.transform.rotation = gameObject.transform.rotation;
		if (gameObject.transform.position.y > 0) {
			initialPlayerCamera.transform.Rotate (0, -180, 0);
			initialPlayerCamera.transform.position = new Vector3 (0, 0, 10);
		} else {
			initialPlayerCamera.transform.position = new Vector3 (0, 0, -10);
		}

	} 

    void Move()
    {
        if (networkView.isMine) {
            Vector2 possition = transform.position;
            float x = Input.GetAxisRaw("Horizontal");

            Vector2 direction = new Vector2(x, 0).normalized;

            Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
            Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

            max.x = max.x - width / 2 - sidePanelWidth;
            min.x = min.x + width / 2 + sidePanelWidth;
            max.y = max.y - height / 2;
            min.y = min.y + height / 2;

            possition += direction * maxSpeed * Time.deltaTime;

            possition.x = Mathf.Clamp(possition.x, min.x, max.x);
            possition.y = Mathf.Clamp(possition.y, min.y, max.y);

            transform.position = possition;
        } else {
            SyncMovement();
        }


    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        syncTime = 0f;
        Vector3 syncPosition = Vector2.zero;
        if (stream.isWriting)
        {
            Debug.Log("Writing");
            syncPosition = gameObject.transform.position;
            stream.Serialize(ref syncPosition);
        } else {
            Debug.Log("Reading");
            stream.Serialize(ref syncPosition);
            
            syncDelay = Time.time - lastSyncTime;
            lastSyncTime = Time.time;

            syncStartPosition = transform.position;
            syncEndPosition = syncPosition;

        }

    }

    void SyncMovement() {
        syncTime += Time.deltaTime;
        //transform.position = syncEndPosition;
        transform.position = Vector2.Lerp(syncStartPosition, syncEndPosition, syncTime/ syncDelay);
    }

    void DoInitialFire()
    {
        if (initialBall != null)
        {
            //initialBall.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + height);

            if (Input.GetKey("space"))
            {
                initialBall.transform.parent = null;
                initialBall.GetComponent<BallScript>().MoveUp();
                initialBall = null;
            }
        }
    }
}
