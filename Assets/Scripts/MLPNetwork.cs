using System.Collections.Generic;
using UnityEngine;

enum EActivationType
{
    SIGMOID,
    TANH,
    RELU,
}

public class Weight
{
    public Weight(float _Value)
    {
        WeightValue = _Value;
        OldValue = _Value;
    }

    private float WeightValue;
    public float Value 
    {
        get 
        { 
            return WeightValue; 
        } 

        set
        {
            OldValue = WeightValue;
            WeightValue = value;
        }
    }

    public float OldValue;

}

public class Neuron
{
    public float Value;

    public float Error;

    public float BiasWeight = 0.2f;

    //Neuron, Weight
    public Dictionary<Neuron, Weight> PreviousNeurons;
}

[System.Serializable]
public class TrainResult
{
    public List<float> Inputs = new List<float>();
    public float Value;
}

public class MLPNetwork : MonoBehaviour
{
    [SerializeField] private EActivationType ActivationTypeOutput;
    [SerializeField] private EActivationType ActivationTypeHidden;

    [HideInInspector] public List<Neuron> OutputLayers;
    private List<Neuron> HiddenLayers;
    private List<Neuron> InputLayers;

    [SerializeField] private int InputLayerCount = 2;
    [SerializeField] private int OutputLayerCount = 1;
    [SerializeField] private int HiddenLayerCount = 2;

    private int Epochs = 10000;
    [SerializeField] private float Gain = 0.3f;

    [SerializeField] private bool UseBias;

    public List<TrainResult> TrainResults;

    TrainResult GetOutputValue()
    {
        foreach (TrainResult TrainResult in TrainResults)
        {
            bool ResultFind = true;
            for (int i = 0; i < InputLayerCount; ++i)
            {
                if (TrainResult.Inputs[i] != InputLayers[i].Value)
                {
                    ResultFind = false;
                    break;
                }
            }

            if (ResultFind)
                return TrainResult;
        }

        Debug.LogError("There is no result for this inputs !");
        return null;
    }

    private void Start()
    {
        InputLayers = new List<Neuron>();
        OutputLayers = new List<Neuron>();
        HiddenLayers = new List<Neuron>();

        for (int i = 0; i < InputLayerCount; ++i)
            InputLayers.Add(new Neuron());

        for (int i = 0; i < HiddenLayerCount; ++i)
            HiddenLayers.Add(new Neuron());

        for (int i = 0; i < OutputLayerCount; ++i)
            OutputLayers.Add(new Neuron());

        ConnectNeuron(InputLayers, HiddenLayers);
        ConnectNeuron(HiddenLayers, OutputLayers);
    }

    private void ConnectNeuron(List<Neuron> PreviousNeurons, List<Neuron> NextNeurons)
    {
        Dictionary<Neuron, Weight> PreviousNeuronConnections = new Dictionary<Neuron, Weight>();

        foreach (Neuron PreviousNeuron in PreviousNeurons)
        {
            PreviousNeuronConnections.Add(PreviousNeuron, new Weight(Random.Range(0.01f, 0.2f)));
        }

        foreach (Neuron NextNeuron in NextNeurons)
        {
            NextNeuron.PreviousNeurons = PreviousNeuronConnections;
        }
    }

    public void UpdateInput(float Input, int Index)
    {
        if (Index >= InputLayers.Count)
            return;

        InputLayers[Index].Value = Input;
    }

    public void Train()
    {
        List<float> InputValues = new List<float>();

        foreach (Neuron InputLayer in InputLayers)
        {
            InputValues.Add(InputLayer.Value);
        }

        for (int i = 0; i < Epochs; i++)
        {
            TrainResult TrainResultOutput = TrainResults[i % TrainResults.Count];

            for (int j = 0; j < InputLayerCount; ++j)
                InputLayers[j].Value = TrainResultOutput.Inputs[j];

            FeedForward();
            BackPropagation(TrainResultOutput);
        }

        for (int i = 0; i < InputValues.Count; ++i)
            InputLayers[i].Value = InputValues[i];
    }

    public void FeedForward()
    {
        foreach (Neuron HiddenNeuron in HiddenLayers)
        {
            float Sum = UseBias ? HiddenNeuron.BiasWeight : 0.0f;

            foreach (var InputNeuron in HiddenNeuron.PreviousNeurons)
            {
                Sum += InputNeuron.Key.Value * InputNeuron.Value.Value;
            }

            HiddenNeuron.Value = Activation(Sum, ActivationTypeHidden);
        }

        foreach (Neuron OutputNeuron in OutputLayers)
        {
            float Sum = UseBias ? OutputNeuron.BiasWeight : 0.0f;

            foreach (var HiddenNeuron in OutputNeuron.PreviousNeurons)
            {
                Sum += HiddenNeuron.Key.Value * HiddenNeuron.Value.Value;
            }

            OutputNeuron.Value = Activation(Sum, ActivationTypeOutput);
        }
    }

    void BackPropagation(TrainResult TrainResultOutput)
    {
        for (int i = OutputLayerCount - 1; i >= 0; --i)
        {
            Neuron OutputNeuron = OutputLayers[i];
            OutputNeuron.Error = Derivative(OutputNeuron.Value, ActivationTypeOutput) * (TrainResultOutput.Value - OutputNeuron.Value);
        }

        for (int i = HiddenLayerCount - 1; i >= 0; --i)
        {
            Neuron HiddenNeuron = HiddenLayers[i];

            float ErrorSum = 0.0f;

            foreach (Neuron OutputNeuron in OutputLayers)
            {
                Weight Weight;

                if (OutputNeuron.PreviousNeurons.TryGetValue(HiddenNeuron, out Weight))
                    ErrorSum += Weight.Value * OutputNeuron.Error;
            }

            HiddenNeuron.Error = Derivative(HiddenNeuron.Value, ActivationTypeHidden) * ErrorSum;
        }

        for (int i = OutputLayerCount - 1; i >= 0; --i)
        {
            Neuron OutputNeuron = OutputLayers[i];

            foreach (var keyValuePair in OutputNeuron.PreviousNeurons)
            {
                Weight Weight = keyValuePair.Value;
                float deltaWeight = Gain * (OutputNeuron.Error * keyValuePair.Key.Value);
                Weight.Value += deltaWeight;
            }

            OutputNeuron.BiasWeight += OutputNeuron.Error * Gain;
        }

        for (int i = HiddenLayerCount - 1; i >= 0; --i)
        {
            Neuron HiddenNeuron = HiddenLayers[i];

            foreach (var InputNeuron in HiddenNeuron.PreviousNeurons)
            {
                float deltaWeight = Gain * (HiddenNeuron.Error * InputNeuron.Key.Value);
                HiddenNeuron.PreviousNeurons[InputNeuron.Key].Value += deltaWeight;
            }

            HiddenNeuron.BiasWeight += HiddenNeuron.Error * Gain;
        }
    }

    float Activation(float input, EActivationType ActivationType)
    {
        switch (ActivationType)
        {
            case EActivationType.RELU:
                return ReLu(input);
            case EActivationType.SIGMOID:
                return Sigmoid(input);
            case EActivationType.TANH:
                return Tanh(input);
        }

        return 0.0f;
    }

    float Derivative(float input, EActivationType ActivationType)
    {
        switch (ActivationType)
        {
            case EActivationType.RELU:
                return DerivativeReLu(input);
            case EActivationType.SIGMOID:
                return DerivativeSigmoid(input);
            case EActivationType.TANH:
                return DerivativeTanh(input);
        }

        return 0.0f;
    }

    float Sigmoid(float input)
    {
        return  1.0f / (1.0f + Mathf.Exp(-input));
    }

    float Tanh(float input)
    {
        return (Mathf.Exp(input) - Mathf.Exp(-input)) / (Mathf.Exp(input) + Mathf.Exp(-input));
    }

    float ReLu(float input)
    {
        return Mathf.Max(0, input);
    }

    float DerivativeSigmoid(float input)
    {
        return input * (1.0f - input);
    }

    float DerivativeTanh(float input)
    {
        return 1.0f - (input * input);
    }

    float DerivativeReLu(float input)
    {
        return input > 0 ? 1 : 0;
    }
}
