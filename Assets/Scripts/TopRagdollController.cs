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

        /*foreach (Bone bone in bones)
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
        }*/

        return inputs;
    }

    public override float FitnessFunction(GeneticModifier modifier)
    {
        /*
         float distance = Vector3.Distance(modifier.headTarget.position, transform.position);

        //score = Mathf.Exp(-distance);
        float score = 1f - Mathf.Clamp(distance, 0.01f, 100f) / 100f;
         */

        //float distanceRight = Vector3.Distance(modifier.headTarget.position, boneRightArm.position);
        float distanceLeft = Vector3.Distance(modifier.headTarget.position, boneLeftArm.position);

        float score = 1f - Mathf.Clamp(distanceLeft, 0.01f, 10f) / 10f;

        return score;
    }
}
