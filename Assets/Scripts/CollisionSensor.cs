using UnityEngine;

public class CollisionSensor : MonoBehaviour
{
    private GameObject objectCollider;

    public Vector3 contactPoint = Vector3.zero;
    public Vector3 contactNormal = Vector3.zero;
    public bool hasCollisionPoint = false;

    private void OnCollisionEnter(Collision collision)
    {
        contactPoint = collision.GetContact(0).point - transform.position;
        contactNormal = collision.GetContact(0).normal;
        hasCollisionPoint = true;
        objectCollider = collision.gameObject;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == objectCollider)
        {
            contactPoint = Vector3.zero;
            contactNormal = Vector3.zero;
            hasCollisionPoint = false;
            objectCollider = null;
        }
    }
}
