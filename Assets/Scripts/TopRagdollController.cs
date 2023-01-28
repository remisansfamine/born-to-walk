using System.Collections.Generic;
using UnityEngine;

public class TopRagdollController : RagdollController
{
    [SerializeField] private float minArmDistance = 0.01f;
    [SerializeField] private float maxArmDistance = 2f;

    public override List<float> GetInputs()
    {
        List<float> inputs = new List<float>();

        return inputs;
    }

    public override float FitnessFunction(GeneticModifier modifier)
    {
        float distanceLeft = Vector3.Distance(modifier.targetTransform.position, boneLeftArm.position);
        float distanceRight = Vector3.Distance(modifier.targetTransform.position, boneRightArm.position);

        float score = 0.5f * (2f - Mathf.Clamp(distanceLeft, minArmDistance, maxArmDistance) / maxArmDistance
                                 - Mathf.Clamp(distanceRight, minArmDistance, maxArmDistance) / maxArmDistance);

        return score;
    }
}
