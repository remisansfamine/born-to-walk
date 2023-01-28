using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class TopRagdollController : RagdollController
{
    public override List<float> GetInputs()
    {
        List<float> inputs = new List<float>();

        return inputs;
    }

    public override float FitnessFunction(GeneticModifier modifier)
    {
        float distanceLeft = Vector3.Distance(modifier.headTarget.position, boneLeftArm.position);

        float score = 1f - Mathf.Clamp(distanceLeft, 0.01f, 10f) / 10f;

        return score;
    }
}
