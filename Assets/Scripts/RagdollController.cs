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
    [SerializeField] List<Rigidbody> bonesRigidbobies = new List<Rigidbody>();

    public List<Bone> bones = new List<Bone>();

    public Transform boneHead = null;

    private void Awake()
    {
        foreach (Rigidbody rb in bonesRigidbobies)
            bones.Add(new Bone(rb));
    }

    public List<float> GetInputs()
    {
        List<float> inputs = new List<float>();

        foreach (Bone bone in bones)
        {
            inputs.Add(bone.collisionSensor.contactPoint.x);
            inputs.Add(bone.collisionSensor.contactPoint.y);
            inputs.Add(bone.collisionSensor.contactPoint.z);
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
