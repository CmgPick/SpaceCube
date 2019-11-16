﻿using UnityEngine;

public class FlyCam: MonoBehaviour{

    public Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 5.0f;
    public float ySpeed = 5.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public float zoomRate = 10.0f;
    public float zoomDampening = 5.0f;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;

    private Vector3 FirstPosition;
    private Vector3 SecondPosition;
    private Vector3 delta;
    private Vector3 lastOffset;
    private Vector3 lastOffsettemp;

    private bool canMove = false;
    private int touchCount;
    int touch;

    void Start() { Init(); }
    void OnEnable() { Init(); }

    public void Init() {

        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }

    public void MoveCam() {

        //get current touch
        touch = Input.touchCount == 1 ? 0 : 1;

        canMove = true;
        touchCount ++;
        Debug.Log("the cam can move");

    }

    public void StopCam(){

        canMove = false;
        touchCount--;
        Debug.Log("the cam cant move");

    }


    void LateUpdate(){

        // ZOOM
        //if (Input.touchCount == 2 && canMove){

        if (touchCount >= 2 && canMove){

            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPreviousPosition = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePreviousPosition = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPreviousPosition - touchOnePreviousPosition).magnitude;
            float TouchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagDiff = prevTouchDeltaMag - TouchDeltaMag;

            desiredDistance += deltaMagDiff * Time.deltaTime * zoomRate * 0.0025f * Mathf.Abs(desiredDistance);
        }

        //  ORBIT
        //if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved && canMove)  {

        if (touchCount == 1 && canMove){

            Vector2 touchposition = Input.GetTouch(touch).deltaPosition;
            xDeg += touchposition.x * xSpeed * 0.002f;
            yDeg -= touchposition.y * ySpeed * 0.002f;
            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

        }


            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = transform.rotation;
            rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
            transform.rotation = rotation;

            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
            currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

            position = target.position - (rotation * Vector3.forward * currentDistance);
            //position = position - targetOffset;
            transform.position = position;
        
    }


    private static float ClampAngle(float angle, float min, float max) {

        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}