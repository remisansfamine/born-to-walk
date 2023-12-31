using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EActivationType
{
    RELU,
    TANH,
    SIGMOID,
    NONE,
}

[System.Serializable]
public class Perceptron
{
    public float initialWeightRange = 1.0f;

    public float biasWeight = 0f;
    public List<float> weights = new List<float>();
    public float state = 0f;
    public float error = 0f;

    public Perceptron() { }
    public Perceptron(Perceptron other)
    {
        initialWeightRange = other.initialWeightRange;
        biasWeight = other.biasWeight;
        weights = new List<float>(other.weights);
        state = other.state;
        error = other.error;
    }

    public Perceptron(int connectionCount, float initialWeightRange)
    {
        this.initialWeightRange = initialWeightRange;

        for (int i = 0; i < connectionCount; i++)
            weights.Add(UnityEngine.Random.Range(-initialWeightRange, initialWeightRange));

        biasWeight = UnityEngine.Random.Range(-initialWeightRange, initialWeightRange);
    }

    public void Mutate()
    {
        for (int i = 0; i < weights.Count; i++)
            weights[i] += UnityEngine.Random.Range(-.1f, .1f);

        biasWeight += UnityEngine.Random.Range(-.1f, .1f);

    }
}

[System.Serializable]
public class Layer
{
    public List<Perceptron> perceptrons = new List<Perceptron>();

    [JsonIgnore]
    public Layer prevLayer = null;

    [JsonIgnore]
    public Layer nextLayer = null;

    public void InitPerceptrons(int nbPerceptrons, float initialWeightRange, int nbPrevLayerPerceptrons)
    {
        for (int i = 0; i < nbPerceptrons; i++)
            perceptrons.Add(new Perceptron(nbPrevLayerPerceptrons, initialWeightRange));
    }

    public Layer() { }

    public Layer(Layer other)
    {
        foreach (Perceptron otherPerceptron in other.perceptrons)
            perceptrons.Add(new Perceptron(otherPerceptron));
    }
}

[System.Serializable]
public class MLPNetwork
{
    [SerializeField] public float gain = 0.3f;
    [SerializeField] public bool useBias = false;
    [SerializeField] public float initialWeightRange = 0.3f;

    [SerializeField] public EActivationType hiddenLayerActivation = EActivationType.SIGMOID;
    [SerializeField] public EActivationType outputLayerActivation = EActivationType.TANH;

    [SerializeField] public float sigmoidBeta = 1f;

    [SerializeField] public int nbInputPerceptrons = 2;
    [SerializeField] public List<int> nbHiddenPerceptrons = new List<int>();
    [SerializeField] public int nbOutputPerceptrons = 1;

    public MLPNetwork() { }

    public MLPNetwork(MLPNetwork other)
    {
        gain = other.gain;
        useBias = other.useBias;
        initialWeightRange = other.initialWeightRange;
        hiddenLayerActivation = other.hiddenLayerActivation;
        outputLayerActivation = other.outputLayerActivation;
        sigmoidBeta = other.sigmoidBeta;

        nbInputPerceptrons = other.nbInputPerceptrons;
        nbHiddenPerceptrons = new List<int>(other.nbHiddenPerceptrons);
        nbOutputPerceptrons = other.nbOutputPerceptrons;

        InitPerceptrons();


        inputLayer = new Layer(other.inputLayer);

        hiddenLayers = new List<Layer>();

        foreach (Layer otherLayer in other.hiddenLayers)
            hiddenLayers.Add(new Layer(otherLayer));

        outputLayer = new Layer(other.outputLayer);

        LinkLayers();
    }

    public Layer inputLayer { get; private set; } = new Layer();
    public List<Layer> hiddenLayers { get; private set; } = new List<Layer>();
    public Layer outputLayer { get; private set; } = new Layer();

    public void Initialize()
    {
        InitPerceptrons();
    }

    public void LinkLayers()
    {
        inputLayer.nextLayer = hiddenLayers.First();

        for (int hl = 0; hl < hiddenLayers.Count; hl++)
        {
            hiddenLayers[hl].prevLayer = hl == 0 ? inputLayer : hiddenLayers[hl - 1];

            hiddenLayers[hl].nextLayer = hl == hiddenLayers.Count - 1 ? outputLayer : hiddenLayers[hl + 1];
        }

        outputLayer.prevLayer = hiddenLayers.Last();
    }

    private void InitPerceptrons()
    {
        int lastLayerPeceptronCount = 0;

        inputLayer.InitPerceptrons(nbInputPerceptrons, initialWeightRange, lastLayerPeceptronCount);

        lastLayerPeceptronCount = inputLayer.perceptrons.Count;

        for (int i = 0; i < nbHiddenPerceptrons.Count; i++)
        {
            Layer newHiddenLayer = new Layer();

            newHiddenLayer.InitPerceptrons(nbHiddenPerceptrons[i], initialWeightRange, lastLayerPeceptronCount);

            lastLayerPeceptronCount = nbHiddenPerceptrons[i];

            hiddenLayers.Add(newHiddenLayer);
        }

        outputLayer.InitPerceptrons(nbOutputPerceptrons, initialWeightRange, lastLayerPeceptronCount);

        LinkLayers();
    }

    float SigmoidThreshold(float input) => 1f / (1f + Mathf.Exp(-sigmoidBeta * input));
    float TanhThreshold(float input) => System.MathF.Tanh(input);
    float ReLUThreshold(float input) => Mathf.Max(0, input);

    float Threshold(float input, EActivationType type)
    {
        switch (type)
        {
            default:
            case EActivationType.NONE:
                return 0f;

            case EActivationType.SIGMOID:
                return SigmoidThreshold(input);

            case EActivationType.TANH:
                return TanhThreshold(input);

            case EActivationType.RELU:
                return ReLUThreshold(input);
        }
    }

    float SigmoidThresholdDerivative(float input) => input * (1f - input);
    float TanhThresholdDerivative(float input) => 1f - (input * input);

    float ReLUThresholdDerivative(float input) => input > 0f ? 1f : 0f;

    float ThresholdDerivative(float input, EActivationType type)
    {
        switch (type)
        {
            default:
            case EActivationType.NONE:
                return 0f;

            case EActivationType.SIGMOID:
                return SigmoidThresholdDerivative(input);

            case EActivationType.TANH:
                return TanhThresholdDerivative(input);

            case EActivationType.RELU:
                return ReLUThresholdDerivative(input);
        }
    }


    public List<float> FeedForward(List<float> inputs)
    {
        if (inputs.Count != inputLayer.perceptrons.Count)
        {
            Debug.LogError("Different inputs count and inputs perceptrons count");
            return null;
        }

        for (int ip = 0; ip < inputLayer.perceptrons.Count; ip++)
            inputLayer.perceptrons[ip].state = inputs[ip];


        foreach (Layer hiddenLayer in hiddenLayers)
        {
            Layer prevLayer = hiddenLayer.prevLayer;

            foreach (Perceptron currLayerPerceptron in hiddenLayer.perceptrons)
            {
                float sum = useBias ? currLayerPerceptron.biasWeight : 0f;

                for (int lp = 0; lp < prevLayer.perceptrons.Count; lp++)
                    sum += currLayerPerceptron.weights[lp] * prevLayer.perceptrons[lp].state;

                float thresholdedSum = Threshold(sum, hiddenLayerActivation);

                currLayerPerceptron.state = thresholdedSum;
            }
        }

        foreach (Perceptron currLayerPerceptron in outputLayer.perceptrons)
        {
            Layer prevLayer = outputLayer.prevLayer;

            float sum = useBias ? currLayerPerceptron.biasWeight : 0f;

            for (int lp = 0; lp < prevLayer.perceptrons.Count; lp++)
                sum += currLayerPerceptron.weights[lp] * prevLayer.perceptrons[lp].state;

            float thresholdedSum = Threshold(sum, outputLayerActivation);

            currLayerPerceptron.state = thresholdedSum;
        }

        return outputLayer.perceptrons.Select(perceptron => perceptron.state).ToList();
    }

    public void BackPropagation(List<float> outputs)
    {
        if (outputs.Count != outputLayer.perceptrons.Count)
        {
            Debug.LogError("Different outputs count and outputs perceptrons count");
            return;
        }

        for (int o = 0; o < outputLayer.perceptrons.Count; o++)
        {
            Perceptron currPerceptron = outputLayer.perceptrons[o];

            float state = currPerceptron.state;

            float error = ThresholdDerivative(state, outputLayerActivation) * (outputs[o] - state);

            currPerceptron.error = error;
        }


        for (int hl = hiddenLayers.Count - 1; hl >= 0; hl--)
        {
            Layer hiddenLayer = hiddenLayers[hl];

            Layer nextLayer = hiddenLayer.nextLayer;

            for (int hp = 0; hp < hiddenLayer.perceptrons.Count; hp++)
            {
                Perceptron currPerceptron = hiddenLayer.perceptrons[hp];

                float sum = 0f;

                foreach (Perceptron nextPerceptron in nextLayer.perceptrons)
                    sum += nextPerceptron.weights[hp] * nextPerceptron.error;

                float state = currPerceptron.state;

                float error = ThresholdDerivative(state, hiddenLayerActivation) * sum;

                currPerceptron.error = error;
            }
        }






        Layer prevLayer = outputLayer.prevLayer;

        foreach (Perceptron currPerceptron in outputLayer.perceptrons)
        {
            float error = currPerceptron.error;

            for (int plp = 0; plp < prevLayer.perceptrons.Count; plp++)
            {
                float prevState = prevLayer.perceptrons[plp].state;

                float deltaWeight = gain * error * prevState;
                currPerceptron.weights[plp] += deltaWeight;
            }

            currPerceptron.biasWeight += gain * error;
        }


        foreach (Layer hiddenLayer in hiddenLayers)
        {
            prevLayer = hiddenLayer.prevLayer;

            foreach (Perceptron currPerceptron in hiddenLayer.perceptrons)
            {
                float error = currPerceptron.error;

                for (int plp = 0; plp < prevLayer.perceptrons.Count; plp++)
                {
                    float prevState = prevLayer.perceptrons[plp].state;

                    float deltaWeight = gain * error * prevState;
                    currPerceptron.weights[plp] += deltaWeight;
                }

                currPerceptron.biasWeight += gain * error;
            }
        }
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
