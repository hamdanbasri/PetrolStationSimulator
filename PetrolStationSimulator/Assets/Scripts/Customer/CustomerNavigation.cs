using UnityEngine;
using UnityEngine.AI;

public class CustomerNavigation : MonoBehaviour
{
    [Header("Destinations")]
    public Transform[] destination;
    public Transform currentDestination;
    public Transform exitDestination;
    public float distanceToDestination;
    public float distanceThreshold;

    [Header("Variables")]
    public float currentFuel;
    public float fuelTankSize;
    public bool agentHasDestination;

    [Header("Private")]
    private NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetDestination();
    }

    // Update is called once per frame
    void Update()
    {
        if (agentHasDestination)
        {
            distanceToDestination = Vector3.Distance(agent.transform.position, currentDestination.position);

            if (distanceToDestination > distanceThreshold)
            {
                agent.destination = currentDestination.position;
            }
            else
            {
                agentHasDestination = false;
            }
        }

        if(currentFuel >= fuelTankSize)
        {
            agent.destination = exitDestination.position;
        }
    }

    public void SetDestination()
    {
        int randomDestination = Random.Range(0,destination.Length);
        currentDestination = destination[randomDestination];
        agentHasDestination = true;
    }
}
