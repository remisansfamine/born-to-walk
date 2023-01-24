using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] private GameObject m_individualPrefab = null;
    [SerializeField] private Transform m_spawnPoint = null;


    [SerializeField] int populationCount = 0;



    // Start is called before the first frame update
    void Start()
    {
        int layerOffset = 6;
        for (int i = 0; i < populationCount; i++)
        {
            GameObject individual = Instantiate(m_individualPrefab, m_spawnPoint.position, m_spawnPoint.rotation);

            individual.SetLayerRecursively(layerOffset + i);

            for (int j = i + 1; j < populationCount; j++)
                Physics.IgnoreLayerCollision(individual.layer, layerOffset + j, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
