using UnityEngine;
using System.Collections;
using System;

public class Swipe : MonoBehaviour {
    private float fingerStartTime = 0.0f;
    private Vector2 fingerStartPos = Vector2.zero;
     
    private bool isSwipe = false;
    private float minSwipeDist = 50.0f;
    private float maxSwipeTime = 0.5f;

    public static int Up = 0;
    public static int Down = 1;
    public static int Left = 2;
    public static int Right = 3;

    public delegate void Delegate(int swipe);
    public event Delegate swipeDelegate;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if (Input.touchCount > 0) {
            foreach (Touch touch in Input.touches) {
                switch (touch.phase) {
                case TouchPhase.Began :
                    /* this is a new touch */
                    isSwipe = true;
                    fingerStartTime = Time.time;
                    fingerStartPos = touch.position;
                    break;

                case TouchPhase.Canceled :
                    /* The touch is being canceled */
                    isSwipe = false;
                    break;

                case TouchPhase.Ended :
                    float gestureTime = Time.time - fingerStartTime;
                    float gestureDist = (touch.position - fingerStartPos).magnitude;
                    
                    if (isSwipe && gestureTime < maxSwipeTime && gestureDist > minSwipeDist) {
                        Vector2 direction = touch.position - fingerStartPos;
                        Vector2 swipeType = Vector2.zero;
                        
                        if (Math.Abs(direction.x) > Math.Abs(direction.y)) {
                            // the swipe is horizontal:
                            swipeType = Vector2.right * Math.Sign(direction.x);
                            if (swipeDelegate!=null) {
                                if (Math.Sign(direction.x)==1) {
                                    swipeDelegate(Right);
                                } else if (Math.Sign(direction.x)==-1) {
                                    swipeDelegate(Left);
                                }
                            }

                        } else {
                            // the swipe is vertical:
                            swipeType = Vector2.up * Math.Sign(direction.y);
                            if (swipeDelegate!=null) {
                                if (Math.Sign(direction.y)==1) {
                                    swipeDelegate(Up);
                                } else if (Math.Sign(direction.y)==-1) {
                                    swipeDelegate(Down);
                                }
                            }
                        }
                    }

                    break;
                } // End Switch
            } // End for
        } // End if
	} // End Update
}



