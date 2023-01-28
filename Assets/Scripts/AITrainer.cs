using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrainArray
{
    public List<float> inputs;
    public List<float> outputs;
}


public class AITrainer : MonoBehaviour
{
    private MLPNetwork mlp = null;

    [SerializeField] private List<TrainArray> trainArrays = new List<TrainArray>();

    private void Awake()
    {
        mlp = GetComponent<MLPNetwork>();
    }

    public void Train(int iterationsCount)
    {
        if (mlp is null)
            return;

        for (int iter = 0; iter < iterationsCount; iter++)
        {
            TrainArray currentTrainArray = trainArrays[iter % trainArrays.Count];

            mlp.FeedForward(currentTrainArray.inputs);
            mlp.BackPropagation(currentTrainArray.outputs);
        }
    }
}
