using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody))]
public class CubeController : MLPInterpreter
{

    private Rigidbody cubeRigidbody = null;
    private CollisionSensor collisionSensor = null;

    private void Awake()
    {
        cubeRigidbody = GetComponent<Rigidbody>();
        collisionSensor = GetComponent<CollisionSensor>();
    }

    public override List<float> GetInputs()
    {
        List<float> inputs = new List<float>();

        return inputs;
    }

    public override void SetOuputs(List<float> outputs)
    {
        cubeRigidbody.velocity = Vector3.zero;
        Vector3 force = new Vector3(outputs[0], outputs[1], outputs[2]);
        cubeRigidbody.AddForce(force * 100f, ForceMode.Impulse);

    }

    public override float FitnessFunction(GeneticModifier modifier)
    {
        float distance = Vector3.Distance(modifier.headTarget.position, transform.position);

        float score = 1f - Mathf.Clamp(distance, 0.01f, 100f) / 100f;

        return score;
    }
}
