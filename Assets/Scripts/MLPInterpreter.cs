using System.Collections.Generic;
using UnityEngine;

public abstract class MLPInterpreter : MonoBehaviour
{
    public abstract List<float> GetInputs();
    public abstract void SetOuputs(List<float> outputs);
    public abstract float FitnessFunction(GeneticModifier modifier);
}