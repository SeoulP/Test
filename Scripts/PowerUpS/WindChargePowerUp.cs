using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindChargePowerUp : MonoBehaviour
{
    PlaneController plane;

    private void Start()
    {
        plane = GameObject.Find("PaperPlane").GetComponent<PlaneController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            plane.windNum++;
            Destroy(this.gameObject);
        }
    }
}
