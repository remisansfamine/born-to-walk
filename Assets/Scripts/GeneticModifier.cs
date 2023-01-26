using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine.SocialPlatforms.Impl;

public class GeneticModifier : MonoBehaviour
{
    public MLPNetwork mlp = new MLPNetwork();

    [SerializeField] private CubeController mlpInterpreter;

    private Transform headTarget = null;

    public float Fitness { get; private set; }


    public void Initialize(Transform _headTarget)
    {
        mlp.Initialize();
        headTarget = _headTarget;
    }

    private void FixedUpdate()
    {
        List<float> inputs = mlpInterpreter.GetInputs();

        Vector3 dir = transform.position - headTarget.position;
        inputs.Add(dir.x);
        inputs.Add(dir.y);
        inputs.Add(dir.z);

        List<float> outputs = mlp.FeedForward(inputs);

        mlpInterpreter.SetOuputs(outputs);   
    }


    public float FitnessFunction()
    {
        float score = 0;

        //float distance = Vector3.Distance(headTarget.position, mlpInterpreter.boneHead.position);
        float distance = Vector3.Distance(headTarget.position, transform.position);

        //score = Mathf.Exp(-distance);
        score = 1f - Mathf.Clamp(distance, 0.01f, 100f) / 100f;
        Debug.Log("Distance:" + distance);
        return score;
    }

    public float CalculateFitness()
    {
        Fitness = FitnessFunction();
        return Fitness;
    }

    public void Mutate(float mutationRate)
    {
        foreach (Layer hiddenLayer in mlp.hiddenLayers)
        {
            foreach (Perceptron hiddenPerceptron in hiddenLayer.perceptrons)
            {
                if (Random.Range(0.0f, 1.0f) < mutationRate)
                    hiddenPerceptron.Mutate();
            }
        }

        foreach (Perceptron outputPerceptron in mlp.outputLayer.perceptrons)
        {
            if (Random.Range(0.0f, 1.0f) < mutationRate)
                outputPerceptron.Mutate();
        }
    }

    public static void Crossover(ref GeneticModifier child, in GeneticModifier parentA, in GeneticModifier parentB)
    {
        for (int hli = 0; hli < child.mlp.hiddenLayers.Count; hli++)
        {
            Layer childHiddenLayer = child.mlp.hiddenLayers[hli];

            Layer hiddenLayerA = parentA.mlp.hiddenLayers[hli];
            Layer hiddenLayerB = parentB.mlp.hiddenLayers[hli];

            for (int hpi = 0; hpi < childHiddenLayer.perceptrons.Count; hpi++)
            {
                bool chooseParentA = Random.Range(0.0f, 1.0f) < 0.5f;
                childHiddenLayer.perceptrons[hpi] = (chooseParentA ? hiddenLayerA : hiddenLayerB).perceptrons[hpi].Clone() as Perceptron;
            }
        }

        Layer outputLayerA = parentA.mlp.outputLayer;
        Layer outputLayerB = parentB.mlp.outputLayer;

        for (int hpi = 0; hpi < outputLayerA.perceptrons.Count; hpi++)
        {
            bool chooseParentA = Random.Range(0.0f, 1.0f) < 0.5f;
            child.mlp.outputLayer.perceptrons[hpi] = (chooseParentA ? outputLayerA : outputLayerB).perceptrons[hpi].Clone() as Perceptron;
        }
    }
}
