using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSensor : MonoBehaviour
{
    private GameObject objectCollider;

    public Vector3 contactPoint = Vector3.zero;

    private void OnCollisionEnter(Collision collision)
    {
        contactPoint = collision.GetContact(0).point - transform.position;
        objectCollider = collision.gameObject;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == objectCollider)
        {
            contactPoint = Vector3.zero;
            objectCollider = null;
        }
    }
}
