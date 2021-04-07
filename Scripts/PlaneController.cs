using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlaneController : MonoBehaviour
{
    [SerializeField]
    public int updraftNum = 3;
    [SerializeField]
    public int windNum = 3;
    [SerializeField]
    float throwForce = 5;
    [SerializeField]
    float stoppingPower = 2;
    [SerializeField]
    float doubleTapTimelimit = 4f;
    float doubleTapTimer;
    float holdTime = 1f;
    float currHeldTime = 0f;
    bool tapped = false;
    bool gameStarted = false;

    [SerializeField]
    GameObject ground;
    [SerializeField]
    Camera cam;

    [SerializeField]
    TMP_Text updraftUI;
    [SerializeField]
    TMP_Text windUI;
    [SerializeField]
    TMP_Text currentSpeed;
    [SerializeField]
    TMP_Text currentHeight;
    [SerializeField]
    TMP_Text distanceTraveled;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationY;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;
        rb.AddForce(Vector3.forward * throwForce * 100);
        doubleTapTimer = doubleTapTimelimit;
    }


    private void Update()
    {
        updraftUI.text = ("Total Updrafts : " + updraftNum);
        windUI.text = ("Wind Charges: " + windNum);
        currentSpeed.text = ("Speed: " + (rb.velocity.z).ToString("#.00"));
        currentHeight.text = ("Height: " + (rb.position.y).ToString("#.00"));
        distanceTraveled.text = ("Distance: " + (rb.position.z / 10).ToString("#.00"));

        if (rb.velocity.magnitude > 10)
        {
            gameStarted = true;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (windNum > 0)
            {
                currHeldTime += 1f * Time.deltaTime;
                if (currHeldTime >= holdTime)
                {
                    windNum--;
                    rb.AddForce(Vector3.forward * 200);
                    currHeldTime = 0;
                }


            }
        }
        else
        {
            currHeldTime = 0;
        }


        // Double tap to boost
        doubleTapTimer += .01f;

        // If you press space bar, and you have updrafts left, tapped is equal to true (default false), AND timer has reached the time limit,
        // set the y velocity = 0, add upward force, and reduce the number of updrafts.
        // Also set tapped = false, and reset the doubletapTimer.

        // If tapped is NOT equal to true, set it equal to true for the next time around.

        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (updraftNum > 0)
            {
                if (tapped)
                {
                    if (doubleTapTimer <= doubleTapTimelimit)
                    {

                        Debug.Log("You have double pressed spacebar!");
                        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                        rb.AddForce(Vector3.up * 250);
                        updraftNum--;
                        tapped = false;
                        doubleTapTimer = 0;
                    }
                }
                else if (tapped == false)
                {
                    
                    tapped = true;
                }
                Debug.Log(Time.time + ": Tapped is " + tapped);
            }
            
            Debug.Log("Current doubleTapTimer time is " + doubleTapTimer);
        }

        // if timer is ever past time limit, reset.
        if (doubleTapTimer > doubleTapTimelimit)
        {
            doubleTapTimer = 0;
            tapped = false;
        }

        // Add some sort of force that allows the player to get back up from difficult positions
        // If below height 250, you slow down, otherwise, progressively speed up.
        
        if (rb.velocity.magnitude < 150)
        {
            rb.AddForce(0, 0, Mathf.Clamp(((transform.position.y - 250) / 200), 0, 5));
        }
        

        if (rb.velocity.magnitude <= 1 && gameStarted || transform.position.y <= ground.transform.position.y + 2 && gameStarted)
        {
            GameOver();
        }

        rb.transform.rotation = Quaternion.Euler(Mathf.Clamp(-rb.velocity.y, -90, 15), 0, 0);
        cam.transform.position = new Vector3(200, transform.position.y + 30, transform.position.z);

        transform.position = new Vector3(0, transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("PowerUp"))
        {
            rb.AddForce(new Vector3(0, 0, rb.velocity.z / -2), ForceMode.VelocityChange);
        }
    }

    void GameOver()
    {
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        Debug.Log("Game Over!");
    }
}
