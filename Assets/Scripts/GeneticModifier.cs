using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine.SocialPlatforms.Impl;

public class GeneticModifier : MonoBehaviour
{
    public MLPNetwork mlp = new MLPNetwork();

    public MLPInterpreter mlpInterpreter;

    public Transform headTarget = null;

    public float Fitness { get; private set; }


    public void Initialize(Transform _headTarget)
    {
        mlp.Initialize();
        mlpInterpreter.Initialize();
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
        return mlpInterpreter.FitnessFunction(this);
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
                childHiddenLayer.perceptrons[hpi] = new Perceptron((chooseParentA ? hiddenLayerA : hiddenLayerB).perceptrons[hpi]);
            }
        }

        Layer outputLayerA = parentA.mlp.outputLayer;
        Layer outputLayerB = parentB.mlp.outputLayer;

        for (int hpi = 0; hpi < outputLayerA.perceptrons.Count; hpi++)
        {
            bool chooseParentA = Random.Range(0.0f, 1.0f) < 0.5f;
            child.mlp.outputLayer.perceptrons[hpi] = new Perceptron((chooseParentA ? outputLayerA : outputLayerB).perceptrons[hpi]);
        }
    }
}
