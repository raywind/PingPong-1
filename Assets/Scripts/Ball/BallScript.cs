﻿using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {

    public float initialSpeed;
    public float maxSpeed;
    public float minSpeed;
    public float acceleration;
    public float playerTouchRandomhDeviation;
    public float magnitude;
    public Vector2 velocity;

    private float horizontalMove;
    private Rigidbody2D rigidbody2D;
    private Vector2 newDirection;


    // used for velocity calculation
    private Vector2 lastPosition;

    // Use this for initialization
    void Start () {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.velocity = new Vector2(2, 2);




    }
	
	// Update is called once per frame
	void Update () {

    }

    void FixedUpdate()
    {
        magnitude = rigidbody2D.velocity.magnitude;

        // Get pos 2d of the ball.
        Vector2 position = transform.position;

        // Velocity calculation. Will be used for the bounce
        velocity = position - lastPosition;
        lastPosition = position;


    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        magnitude = rigidbody2D.velocity.magnitude;

        if (coll.gameObject.tag == "PlayerPaddleTag") {

            horizontalMove = Input.GetAxisRaw("Horizontal");
            

            if (horizontalMove == 0)
                newDirection = rigidbody2D.velocity;

            if (horizontalMove > 0) {
                newDirection = rigidbody2D.velocity + Vector2.right;
                newDirection += new Vector2(Random.Range(-playerTouchRandomhDeviation, playerTouchRandomhDeviation), Random.Range(-playerTouchRandomhDeviation, playerTouchRandomhDeviation));
            }


            if (horizontalMove < 0) {
                newDirection = rigidbody2D.velocity + Vector2.left;
                newDirection += new Vector2(Random.Range(-playerTouchRandomhDeviation, playerTouchRandomhDeviation), Random.Range(-playerTouchRandomhDeviation, playerTouchRandomhDeviation));
            }


            //rigidbody2D.velocity = Vector2.ClampMagnitude(newDirection * (1 + acceleration), maxSpeed);

            newDirection = newDirection * (magnitude / newDirection.magnitude);
            rigidbody2D.velocity = newDirection;

        }

        if (coll.gameObject.tag == "EnemyBlockTag")
        {
            Debug.Log("EnemyBlockTag Collision");

            // Normal
            Vector3 normal = coll.contacts[0].normal;

            //Direction
            Vector3 normalizedVelocity = velocity.normalized;

            // Reflection
            Vector3 reflection = Vector3.Reflect(normalizedVelocity, normal).normalized;

            // Assign normalized reflection with the constant speed
            rigidbody2D.velocity = new Vector2(reflection.x, reflection.y) * magnitude;

            Destroy(coll.gameObject);
        }

    }

    void OnTriggerEnter2D(Collider2D coll)
    {

        if (coll.gameObject.tag == "WallTag")
        {
            Debug.Log("Wall Collision");
            rigidbody2D.velocity = new Vector2(-rigidbody2D.velocity.x, rigidbody2D.velocity.y);
            //rigidbody2D.velocity = new Vector2(-rigidbody2D.velocity.x + Random.Range(-0.1f, 0.1f), rigidbody2D.velocity.y + Random.Range(-0.1f, 0.1f));
        }

        if (coll.gameObject.tag == "RoofTag")
        {
            Debug.Log("Roof Collision");
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, -rigidbody2D.velocity.y);
        }

        if (coll.gameObject.tag == "PlayerPaddleTag")
        {
            Debug.Log("PlayerPaddleTag Collision");
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, -rigidbody2D.velocity.y);

        }





    }


    public void MoveUp() {

        rigidbody2D.velocity = new Vector2(0, initialSpeed);
    }



}