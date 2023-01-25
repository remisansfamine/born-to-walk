using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public struct Bone
{
    public Rigidbody rigidbody;
    public CollisionSensor collisionSensor;

    public Bone(Rigidbody rb)
    {
        collisionSensor = rb.GetComponent<CollisionSensor>();
        rigidbody = rb;
    }
}

public class RagdollController : MonoBehaviour
{
    public List<Bone> bones = new List<Bone>();

    public Transform boneHead = null;

    private void Awake()
    {
        List<Rigidbody> bonesRigidbobies = new List<Rigidbody>();

        GetComponentsInChildren(bonesRigidbobies);

        foreach (Rigidbody rb in bonesRigidbobies)
            bones.Add(new Bone(rb));
    }

    public List<float> GetInputs()
    {
        List<float> inputs = new List<float>();

        foreach (Bone bone in bones)
        {
            if (bone.collisionSensor.ContactPoint != null)
            {
                ContactPoint contactPoint = bone.collisionSensor.ContactPoint.Value;
                inputs.Add(contactPoint.point.x);
                inputs.Add(contactPoint.point.y);
                inputs.Add(contactPoint.point.z);
            }
            else 
            {
                inputs.Add(-1f);
                inputs.Add(-1f);
                inputs.Add(-1f);
            }
        }

        return inputs;
    }

    public void SetOuputs(List<float> outputs)
    {
        for (int i = 0; i < bones.Count; ++i)
        {
            int outputsIndex = i * 3;
            Vector3 torque = new Vector3(outputs[outputsIndex], outputs[outputsIndex + 1], outputs[outputsIndex + 2 ]);
            torque = torque * 2 - Vector3.one;

            bones[i].rigidbody.AddTorque(torque * 1000f, ForceMode.Impulse);
        }
    }

}
