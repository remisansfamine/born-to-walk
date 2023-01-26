using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public interface IMLPInterpreter
{
    public List<float> GetInputs();
    public void SetOuputs(List<float> outputs);
}
