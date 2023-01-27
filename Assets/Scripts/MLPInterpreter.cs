using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public abstract class MLPInterpreter : MonoBehaviour
{
    public abstract void Initialize();
    public abstract List<float> GetInputs();
    public abstract void SetOuputs(List<float> outputs);
    public abstract float FitnessFunction(GeneticModifier modifier);
}