using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.Windows;
using System.Linq;
using Newtonsoft.Json;

public class PopulationManager : MonoBehaviour
{
    [Header("Instantiation settings")]
    [SerializeField] private GameObject m_individualPrefab = null;
    [SerializeField] private Transform spawnPoint = null;
    [SerializeField] private int populationCount = 0;
    [SerializeField] private bool useGrid = false;
    [SerializeField] private int maxColumnCount = 5;
    [SerializeField] private float spacing = 4f;

    [SerializeField] private bool useLayerIgnore = true;
    [SerializeField] private int layerOffset = 6;

    [Header("Generation settings")]
    [SerializeField] private bool reproduceGeneration = true;
    [SerializeField] private float mutationRate = 0.01f;
    [SerializeField] private int maxFixedStep = 0;
    [SerializeField] private float timeScale = 1f;
    [Tooltip("Elite count")]
    [SerializeField] private int elitism = 0;

    [Header("Goal")]
    [SerializeField] private Transform headTarget = null;
    [SerializeField] private bool teleportTarget = true;
    [SerializeField] private float goalDistance = 25f;


    private List<GeneticModifier> population = new List<GeneticModifier>();

    private float fitnessSum = 0;
    private int generationCount = 0;
    private int currentFixedStep = 0;

    private float initialFixedDeltaTime = 0;

    void Start()
    {
        initialFixedDeltaTime = Time.fixedDeltaTime;

        for (int i = 0; i < populationCount; i++)
        {
            GeneticModifier individualGen =  InstantiateIndividual(population.Count);
            population.Add(individualGen);    
        }

        if (useLayerIgnore)
        {
            for (int lai = layerOffset; lai < populationCount + layerOffset; lai++)
            {
                for (int lbi = lai + 1; lbi < populationCount + layerOffset; lbi++)
                    Physics.IgnoreLayerCollision(lai, lbi, true);
            }

            SetLayers();
        }

        generationCount++;

    }

    private void Update()
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = initialFixedDeltaTime * Time.timeScale;
    }

    private void FixedUpdate()
    {
        currentFixedStep++;

        if (maxFixedStep > currentFixedStep || !reproduceGeneration)
            return;

        NewGeneration();
        currentFixedStep = 0;
    }

    private void SetLayers()
    {
        for (int i = 0; i < populationCount; i++)
        {
            population[i].gameObject.SetLayerRecursively(layerOffset + i);
        }
    }

    private GeneticModifier InstantiateIndividual(int currentPopulationCount)
    {
        Vector3 spawnPointPos = spawnPoint.position;

        if (useGrid)
        {
            spawnPointPos += new Vector3(spacing * (currentPopulationCount % maxColumnCount), 0f, spacing * (currentPopulationCount / maxColumnCount));
        }

        GameObject individual = Instantiate(m_individualPrefab, spawnPointPos, spawnPoint.rotation);

        GeneticModifier geneticModifier = individual.GetComponent<GeneticModifier>();
        geneticModifier.Initialize(headTarget);

        return geneticModifier;
    }

    private void CalculateFitnessSum()
    {
        fitnessSum = 0;
        GeneticModifier best = population[0];

        for (int i = 0; i < population.Count; ++i)
        {
            fitnessSum += population[i].CalculateFitness();

            if (population[i].Fitness > best.Fitness)
                best = population[i];
        }

        Debug.Log("Best Fitness:" + best.Fitness);
    }

    private GeneticModifier ChooseParent(List<GeneticModifier> possibleParents)
    {
        int randomElite = Random.Range(0, possibleParents.Count - 1);

        return possibleParents[randomElite];
    }

    public int CompareIndvidual(GeneticModifier a, GeneticModifier b)
    {
        if (a.Fitness > b.Fitness)
            return -1;

        if (a.Fitness < b.Fitness)
            return 1;

        return 0;
    }

    private void NewGeneration()
    {
        if (population.Count <= 0) 
            return;

        CalculateFitnessSum();
        population.Sort(CompareIndvidual);

        if (teleportTarget)
        {
            Vector2 circle = Random.insideUnitCircle;
            circle.Normalize();
            circle *= goalDistance;

            headTarget.position = new Vector3(circle.x, 0f, circle.y);
        }


        List<GeneticModifier> newPopulation = new List<GeneticModifier>();

        for (int i = 0; i < population.Count; ++i)
        {
            if (i < elitism)
            {
                GeneticModifier child = InstantiateIndividual(newPopulation.Count);
                child.mlp = new MLPNetwork(population[i].mlp);
                newPopulation.Add(child);
            }
            else
            {
                List<GeneticModifier> possibleParents = population.GetRange(0, elitism);
                GeneticModifier parentA = ChooseParent(possibleParents);

                possibleParents.Remove(parentA);

                GeneticModifier parentB = ChooseParent(possibleParents);

                GeneticModifier child = InstantiateIndividual(newPopulation.Count);

                GeneticModifier.Crossover(ref child, parentA, parentB);

                child.Mutate(mutationRate);

                newPopulation.Add(child);
            }
        }

        DestroyPopulation();

        population = newPopulation;

        if (useLayerIgnore)
            SetLayers();

        generationCount++;
    }

    void DestroyPopulation()
    {
        for (int i = 0; i < population.Count; ++i)
            Destroy(population[i].gameObject);

        population.Clear();
    }

    public void SaveToJSON(string filename)
    {
        string directory = "SavedNeuralNetworks/";
        string dirpath = directory;

        if (!Directory.Exists(dirpath))
            Directory.CreateDirectory(dirpath);

        string jsonStr = JsonConvert.SerializeObject(population.Select((GeneticModifier individual) => individual.mlp));

        System.IO.File.WriteAllText(dirpath + "/" + filename, jsonStr);
    }    

    public void LoadFromJSON(string filename)
    {
        string directory = "SavedNeuralNetworks/";
        string dirpath = directory;

        string fullpath = dirpath + "/" + filename;

        DestroyPopulation();

        if (System.IO.File.Exists(fullpath))
        {
            string jsonStr = System.IO.File.ReadAllText(fullpath);

            List<MLPNetwork> mlpList = JsonConvert.DeserializeObject(jsonStr, typeof(List<MLPNetwork>)) as List<MLPNetwork>;
            populationCount = mlpList.Count;

            foreach (MLPNetwork mlp in mlpList)
            {
                mlp.LinkLayers();

                GeneticModifier individualGen = InstantiateIndividual(population.Count);
                individualGen.mlp = mlp;
                population.Add(individualGen);
            }
        }
    }
}
