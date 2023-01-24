using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

public class GeneticModifier : MonoBehaviour
{
    [SerializeField] private MLPNetwork mlp = new MLPNetwork();

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

        foreach (Perceptron hiddenPerceptron in mlp.outputLayer.perceptrons)
        {
            if (Random.Range(0.0f, 1.0f) < mutationRate)
                hiddenPerceptron.Mutate();
        }
    }

    public static void Crossover(ref MLPNetwork child, in MLPNetwork parentA, in MLPNetwork parentB)
    {
        for (int hli = 0; hli < child.hiddenLayers.Count; hli++)
        {
            Layer childHiddenLayer = child.hiddenLayers[hli];

            Layer hiddenLayerA = parentA.hiddenLayers[hli];
            Layer hiddenLayerB = parentB.hiddenLayers[hli];

            for (int hpi = 0; hpi < childHiddenLayer.perceptrons.Count; hpi++)
            {
                bool chooseParentA = Random.Range(0.0f, 1.0f) < 0.5f;
                childHiddenLayer.perceptrons[hpi] = (chooseParentA ? hiddenLayerA : hiddenLayerB).perceptrons[hpi].Clone() as Perceptron;
            }
        }

        Layer outputLayerA = parentA.outputLayer;
        Layer outputLayerB = parentB.outputLayer;

        for (int hpi = 0; hpi < outputLayerA.perceptrons.Count; hpi++)
        {
            bool chooseParentA = Random.Range(0.0f, 1.0f) < 0.5f;
            child.outputLayer.perceptrons[hpi] = (chooseParentA ? outputLayerA : outputLayerB).perceptrons[hpi].Clone() as Perceptron;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
