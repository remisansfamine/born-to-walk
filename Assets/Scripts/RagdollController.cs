using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using UnityEngine.WSA;

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
            float torque = outputs[i];
            float swingTorque =  outputs[i * 2];

            bones[i].rigidbody.AddRelativeTorque(bones[i].characterJoint.axis * (torque * 1000f), ForceMode.Impulse);
            bones[i].rigidbody.AddRelativeTorque(bones[i].characterJoint.swingAxis * (swingTorque * 1000f), ForceMode.Impulse);
        }
    }

    public override float FitnessFunction(GeneticModifier modifier)
    {
        //Reach target for walk
        float distance = Vector3.Distance(modifier.headTarget.position, boneHead.position);
        
        //To standing
        float headHeightScore =  boneHead.position.y / 2.5f;

        float angleFoot1 = Vector3.Angle(footCollisionSensor[0].transform.forward, Vector3.up);
        float angleFoot2 = Vector3.Angle(footCollisionSensor[1].transform.forward, Vector3.up);
        float inclinationFootScore = (1f - (angleFoot1 / 180f) / 2) + (1f - (angleFoot2 / 180f) / 2);

        float angleTorso = Vector3.Angle(torso.up, Vector3.up);
        float inclinationScore = 1f - (angleTorso / 180f);

        float score = headHeightScore * 1f;

        return score;
    }

}
