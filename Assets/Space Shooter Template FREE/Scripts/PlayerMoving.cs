﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script defines the borders of ‘Player’s’ movement. Depending on the chosen handling type, it moves the ‘Player’ together with the pointer.
/// </summary>

[System.Serializable]
public class Borders
{
    [Tooltip("offset from viewport borders for player's movement")]
    public float minXOffset = 1.5f, maxXOffset = 1.5f, minYOffset = 1.5f, maxYOffset = 1.5f;
    [HideInInspector] public float minX, maxX, minY, maxY;
}

public class PlayerMoving : MonoBehaviour
{

    [Tooltip("offset from viewport borders for player's movement")]
    public Borders borders;
    Camera mainCamera;
    bool controlIsActive = true;
    public int speed = 10;
    public static PlayerMoving instance; //unique instance of the script for easy access to the script

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        ResizeBorders();                //setting 'Player's' moving borders deending on Viewport's size
    }

    private void Update()
    {
        if (controlIsActive)
        {
#if UNITY_STANDALONE || UNITY_EDITOR    //if the current platform is not mobile, set pc settings
            // if (Input.GetKey(KeyCode.A))
            // {
            //     transform.Translate(x: -speed * Time.deltaTime, y: 0, z: 0);
            // }
            // if (Input.GetKey(KeyCode.D))
            // {
            //     transform.Translate(x: speed * Time.deltaTime, y: 0, z: 0);
            // }
            // if (Input.GetKey(KeyCode.S))
            // {
            //     transform.Translate(x: 0, y: -speed * Time.deltaTime, z: 0);
            // }
            // if (Input.GetKey(KeyCode.W))
            // {
            //     transform.Translate(x: 0, y: speed * Time.deltaTime, z: 0);
            // }
            float horiz = Input.GetAxis("Horizontal");
            float vert = Input.GetAxis("Vertical");
            transform.Translate(
                horiz * speed * Time.deltaTime,
                vert * speed * Time.deltaTime,
                0
            );
#endif

#if UNITY_IOS || UNITY_ANDROID //if current platform is mobile, 

            if (Input.touchCount == 1) // if there is a touch
            {
                Touch touch = Input.touches[0];
                Vector3 touchPosition = mainCamera.ScreenToWorldPoint(touch.position);  //calculating touch position in the world space
                touchPosition.z = transform.position.z;
                transform.position = Vector3.MoveTowards(transform.position, touchPosition, 30 * Time.deltaTime);
            }

            float horiz = Input.acceleration.x;
            float vert = Input.acceleration.y;
            transform.Translate(
            horiz*speed*Time.deltaTime,
            vert*speed*Time.deltaTime,
            0
        );

#endif
            float x;
            if (transform.position.x > (borders.maxX + borders.maxXOffset))
            {
                x = borders.minX;
            }
            else if (transform.position.x < (borders.minX - borders.minXOffset))
            {
                x = borders.maxX;
            }
            else
            {
                x = transform.position.x;
            }
            transform.position = new Vector3    //if 'Player' crossed the movement borders, returning him back 
                (
                x,
                Mathf.Clamp(transform.position.y, borders.minY, borders.maxY),
                0
                );
        }
    }

    //setting 'Player's' movement borders according to Viewport size and defined offset
    void ResizeBorders()
    {
        borders.minX = mainCamera.ViewportToWorldPoint(Vector2.zero).x + borders.minXOffset;
        borders.minY = mainCamera.ViewportToWorldPoint(Vector2.zero).y + borders.minYOffset;
        borders.maxX = mainCamera.ViewportToWorldPoint(Vector2.right).x - borders.maxXOffset;
        borders.maxY = mainCamera.ViewportToWorldPoint(Vector2.up).y - borders.maxYOffset;
    }
}
