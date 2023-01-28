using System.Collections.Generic;
using UnityEngine;

public class Bone
{
    public Rigidbody rigidbody;
    public CharacterJoint characterJoint;
    public CollisionSensor collisionSensor;

    public Bone(Rigidbody rb)
    {
        rigidbody = rb;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        rb.TryGetComponent(out characterJoint);
        rb.TryGetComponent(out collisionSensor);

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

    public Transform torso;
    public Transform leftLeg;
    public Transform rightLeg;

    public List<CollisionSensor> footCollisionSensor = new List<CollisionSensor>();

    [SerializeField] private float torqueModifier = 1000f;

    [SerializeField] private float maxHeadHeight = 2.5f;

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
            if (bone.characterJoint)
            {
                inputs.Add(bone.characterJoint.currentTorque.x);
                inputs.Add(bone.characterJoint.currentTorque.y);
                inputs.Add(bone.characterJoint.currentTorque.z);

                inputs.Add(bone.characterJoint.currentForce.x);
                inputs.Add(bone.characterJoint.currentForce.y);
                inputs.Add(bone.characterJoint.currentForce.z);
            }

            inputs.Add(bone.rigidbody.transform.localPosition.x);
            inputs.Add(bone.rigidbody.transform.localPosition.y);
            inputs.Add(bone.rigidbody.transform.localPosition.z);

            inputs.Add(bone.rigidbody.transform.localRotation.x);
            inputs.Add(bone.rigidbody.transform.localRotation.y);
            inputs.Add(bone.rigidbody.transform.localRotation.z);

        }

        return inputs;
    }

    public override void SetOuputs(List<float> outputs)
    {
        for (int i = 0; i < bones.Count - 1; ++i)
        {
            float outTorque = outputs[i];
            float outSwingTorque =  outputs[i * 2];

            float torque = outTorque * torqueModifier;
            float swingTorque = outSwingTorque * torqueModifier;

            bones[i].rigidbody.AddRelativeTorque(torque * bones[i].characterJoint.axis, ForceMode.Impulse);
            bones[i].rigidbody.AddRelativeTorque(swingTorque * bones[i].characterJoint.swingAxis, ForceMode.Impulse);
        }
    }

    public override float FitnessFunction(GeneticModifier modifier)
    {
        //To standing
        float headHeightScore =  boneHead.position.y / maxHeadHeight;

        float score = headHeightScore * 1f;

        return score;
    }

}
