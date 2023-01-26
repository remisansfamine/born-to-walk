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

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Rigidbody bonesHipsRb;
    [SerializeField] private List<Rigidbody> bonesRigidbobies = new List<Rigidbody>();

    public List<Bone> bones = new List<Bone>();

    public Transform boneHead = null;

    private void Awake()
    {
        foreach (Rigidbody rb in bonesRigidbobies)
            bones.Add(new Bone(rb));

        bones.Add(new Bone(bonesHipsRb));
    }

    public List<float> GetInputs()
    {
        List<float> inputs = new List<float>();

        foreach (Bone bone in bones)
        {
            /*inputs.Add(bone.collisionSensor.contactPoint.x);
            inputs.Add(bone.collisionSensor.contactPoint.y);
            inputs.Add(bone.collisionSensor.contactPoint.z);*/

            inputs.Add(bone.rigidbody.transform.position.x);
            inputs.Add(bone.rigidbody.transform.position.y);
            inputs.Add(bone.rigidbody.transform.position.z);

            inputs.Add(bone.rigidbody.transform.eulerAngles.x);
            inputs.Add(bone.rigidbody.transform.eulerAngles.y);
            inputs.Add(bone.rigidbody.transform.eulerAngles.z);

            inputs.Add(bone.rigidbody.velocity.x);
            inputs.Add(bone.rigidbody.velocity.y);
            inputs.Add(bone.rigidbody.velocity.z);

            inputs.Add(bone.rigidbody.angularVelocity.x);
            inputs.Add(bone.rigidbody.angularVelocity.y);
            inputs.Add(bone.rigidbody.angularVelocity.z);
        }

        return inputs;
    }

    public void SetOuputs(List<float> outputs)
    {
        for (int i = 0; i < bones.Count - 1; ++i)
        {
            float torque = outputs[i] * 2f - 1f;
            bones[i].rigidbody.AddTorque(bones[i].characterJoint.swingAxis * (torque * 100f), ForceMode.Impulse);
        }
    }

}
