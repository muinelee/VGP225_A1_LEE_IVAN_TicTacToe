using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SportsCarController carController = other.GetComponent<SportsCarController>();
            if (carController != null)
            {
                carController.ActivateSpeedBoost();
            }
        }
    }
}