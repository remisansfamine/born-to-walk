using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CubeController : MLPInterpreter
{

    private Rigidbody cubeRigidbody = null;
    private CollisionSensor collisionSensor = null;

    [SerializeField] private float minDistance = 0.01f;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private float forceModifier = 100f;

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
        Vector3 outForce = new Vector3(outputs[0], outputs[1], outputs[2]);
        cubeRigidbody.AddForce(outForce * forceModifier, ForceMode.Impulse);

    }

    public override float FitnessFunction(GeneticModifier modifier)
    {
        float distance = Vector3.Distance(modifier.targetTransform.position, transform.position);

        float score = 1f - Mathf.Clamp(distance, minDistance, maxDistance) / maxDistance;

        return score;
    }
}
