using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSensor : MonoBehaviour
{
    private GameObject objectCollider;
    public ContactPoint? ContactPoint { get; private set; } = null;

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint = collision.contacts[0];
        objectCollider = collision.gameObject;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == objectCollider)
        {
            ContactPoint = null;
            objectCollider = null;
        }
    }
}
