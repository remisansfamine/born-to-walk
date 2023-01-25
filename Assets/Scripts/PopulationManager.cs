using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] private GameObject m_individualPrefab = null;
    [SerializeField] private Transform m_spawnPoint = null;

    [SerializeField] private int populationCount = 0;
    [SerializeField] private int layerOffset = 6;

    [SerializeField] private Transform headTarget = null;

    private List<GeneticModifier> population = new List<GeneticModifier>();

    public float BestFitness { get; private set; }
    public List<MLPNetwork> BestGens = new List<MLPNetwork>();

    private float fitnessSum = 0;
    [SerializeField] private float mutationRate = 0.01f;

    private int generationCount = 0;

    [SerializeField] private int maxFixedStep = 0;
    private int currentFixedStep = 0;

    void Start()
    {
        for (int lai = layerOffset; lai < populationCount; lai++)
        {
            for (int lbi = lai; lbi < populationCount; lbi++)
            {
                Physics.IgnoreLayerCollision(lai, lbi, true);
            }
        }

        for (int i = 0; i < populationCount; i++)
        {
            GeneticModifier individualGen =  InstantiateIndividual();
            population.Add(individualGen);    
        }

        SetLayers();

        generationCount++;

        Time.timeScale = 5f;
    }

    private void FixedUpdate()
    {
        currentFixedStep++;

        if (maxFixedStep > currentFixedStep)
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

    private GeneticModifier InstantiateIndividual()
    {
        GameObject individual = Instantiate(m_individualPrefab, m_spawnPoint.position, m_spawnPoint.rotation);

        int i = population.Count - 1;

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

        BestFitness = best.Fitness;

        //TODO: le loup
        BestGens.Add(best.MLP.Clone() as MLPNetwork);
    }

    private GeneticModifier ChooseParent()
    {
        float randomNumber = Random.Range(0f, 1f) * fitnessSum;

        for (int i = 0; i < population.Count; i++)
        {
            if (randomNumber < population[i].Fitness)
                return population[i];

            randomNumber -= population[i].Fitness;
        }

        return null;
    }

    private void NewGeneration()
    {
        if (population.Count <= 0) 
            return;

        CalculateFitnessSum();

        List<GeneticModifier> newPopulation = new List<GeneticModifier>();

        for (int i = 0; i < population.Count; ++i)
        {
            GeneticModifier parentA = ChooseParent();
            GeneticModifier parentB = ChooseParent();

            GeneticModifier child = InstantiateIndividual();

            GeneticModifier.Crossover(ref child, parentA, parentB);

            child.Mutate(mutationRate);

            newPopulation.Add(child);
        }

        for (int i = 0; i < population.Count; ++i)
            Destroy(population[i].gameObject);

        population = newPopulation;

        SetLayers();

        generationCount++;
    }
    
}
