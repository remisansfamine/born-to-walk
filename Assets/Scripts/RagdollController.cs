using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Bone
{
    public Rigidbody rigidbody;
    public CollisionSensor collisionSensor;

    public Bone(Rigidbody rb)
    {
        collisionSensor= rb.GetComponent<CollisionSensor>();
        rigidbody = rb;
    }

}

public class RagdollController : MonoBehaviour
{
    private List<Bone> bones = new List<Bone>();

    private void Awake()
    {
        List<Rigidbody> bonesRigidbobies = new List<Rigidbody>();

        GetComponentsInChildren(bonesRigidbobies);

        foreach (Rigidbody rb in bonesRigidbobies)
            bones.Add(new Bone(rb));
    }

    private void GetInputs()
    {

    }

}
