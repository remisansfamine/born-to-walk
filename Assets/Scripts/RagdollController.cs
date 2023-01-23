using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private List<Rigidbody> bonesRigidbobies = new List<Rigidbody>();

    private void Awake()
    {
        GetComponentsInChildren(bonesRigidbobies);

        /*foreach (Rigidbody rb in bonesRigidbobies)
            rb.useGravity = false;*/
    }

    private void GetInputs()
    {

    }

}
