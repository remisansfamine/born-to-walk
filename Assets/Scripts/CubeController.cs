using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody))]
public class CubeController : MonoBehaviour
{

    private Rigidbody cubeRigidbody = null;
    private CollisionSensor collisionSensor = null;

    private void Awake()
    {
        cubeRigidbody = GetComponent<Rigidbody>();
        collisionSensor = GetComponent<CollisionSensor>();
    }

    public List<float> GetInputs()
    {
        List<float> inputs = new List<float>();

        /*inputs.Add(collisionSensor.contactPoint.x);
        inputs.Add(collisionSensor.contactPoint.y);
        inputs.Add(collisionSensor.contactPoint.z);

        inputs.Add(transform.position.x);
        inputs.Add(transform.position.y);
        inputs.Add(transform.position.z);

        inputs.Add(transform.eulerAngles.x);
        inputs.Add(transform.eulerAngles.y);
        inputs.Add(transform.eulerAngles.z);

        inputs.Add(cubeRigidbody.velocity.x);
        inputs.Add(cubeRigidbody.velocity.y);
        inputs.Add(cubeRigidbody.velocity.z);

        inputs.Add(cubeRigidbody.angularVelocity.x);
        inputs.Add(cubeRigidbody.angularVelocity.y);
        inputs.Add(cubeRigidbody.angularVelocity.z);*/

        return inputs;
    }

    public void SetOuputs(List<float> outputs)
    {
        cubeRigidbody.velocity = Vector3.zero;
        Vector3 force = new Vector3(outputs[0], outputs[1], outputs[2]);
        cubeRigidbody.AddForce(force * 10f, ForceMode.Impulse);
        /*Vector3 torque = new Vector3(outputs[3], outputs[4], outputs[5]) * 2f - Vector3.one;
        cubeRigidbody.AddRelativeTorque(torque * 10f, ForceMode.Impulse);*/

    }
}
