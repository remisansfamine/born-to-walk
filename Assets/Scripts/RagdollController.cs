using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Bone
{
    public Rigidbody rigidbody;
    public CollisionSensor collisionSensor;
    public CharacterJoint characterJoint;

    public Bone(Rigidbody rb)
    {
        rigidbody = rb;
        collisionSensor = rb.GetComponent<CollisionSensor>();
        
        rb.TryGetComponent(out characterJoint);

    }
}

public class RagdollController : MLPInterpreter
{ 
    [SerializeField] private Rigidbody bonesHipsRb;
    [SerializeField] private List<Rigidbody> bonesRigidbobies = new List<Rigidbody>();

    public List<Bone> bones = new List<Bone>();

    public Transform boneHead = null;
    public Transform boneLeftArm = null;
    public Transform boneRightArm = null;

    private void Awake()
    {
        foreach (Rigidbody rb in bonesRigidbobies)
            bones.Add(new Bone(rb));

        bones.Add(new Bone(bonesHipsRb));
    }

    public override List<float> GetInputs()
    {
        List<float> inputs = new List<float>();

        foreach (Bone bone in bones)
        {
            inputs.Add(bone.rigidbody.transform.position.x);
            inputs.Add(bone.rigidbody.transform.position.y);
            inputs.Add(bone.rigidbody.transform.position.z);

            inputs.Add(bone.rigidbody.angularVelocity.x);
            inputs.Add(bone.rigidbody.angularVelocity.y);
            inputs.Add(bone.rigidbody.angularVelocity.z);
        }

        return inputs;
    }

    public override void SetOuputs(List<float> outputs)
    {
        for (int i = 0; i < bones.Count - 1; ++i)
        {
            float torque = outputs[i];
            float swingTorque =  outputs[i * 2];

            bones[i].rigidbody.AddRelativeTorque(bones[i].characterJoint.axis * (torque * 100f), ForceMode.Impulse);
            bones[i].rigidbody.AddRelativeTorque(bones[i].characterJoint.swingAxis * (swingTorque * 100f), ForceMode.Impulse);
        }
    }

    public override float FitnessFunction(GeneticModifier modifier)
    {
        float distance = Vector3.Distance(modifier.headTarget.position, boneHead.position);
        float score = Mathf.Exp(-distance);

        return score;
    }

}
