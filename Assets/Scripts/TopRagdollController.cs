using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class TopRagdollController : RagdollController
{
    public override List<float> GetInputs()
    {
        List<float> inputs = new List<float>();

        foreach (Bone bone in bones)
        {
            inputs.Add(bone.rigidbody.transform.position.x);
            inputs.Add(bone.rigidbody.transform.position.y);
            inputs.Add(bone.rigidbody.transform.position.z);

            inputs.Add(bone.rigidbody.transform.eulerAngles.x);
            inputs.Add(bone.rigidbody.transform.eulerAngles.y);
            inputs.Add(bone.rigidbody.transform.eulerAngles.z);

            inputs.Add(bone.rigidbody.angularVelocity.x);
            inputs.Add(bone.rigidbody.angularVelocity.y);
            inputs.Add(bone.rigidbody.angularVelocity.z);
        }

        return inputs;
    }

    public override float FitnessFunction(GeneticModifier modifier)
    {
        float distanceRight = Vector3.Distance(modifier.headTarget.position, boneRightArm.position);
        float distanceLeft = Vector3.Distance(modifier.headTarget.position, boneLeftArm.position);

        float averageDist = (distanceRight + distanceLeft) * 0.5f;

        float score = Mathf.Exp(-averageDist);

        Debug.Log(score);

        return score;
    }
}
