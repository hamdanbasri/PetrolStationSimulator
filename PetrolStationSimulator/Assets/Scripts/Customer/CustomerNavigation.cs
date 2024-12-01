using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class CustomerNavigation : MonoBehaviour
{
    [Header("Destinations")]
    public Transform[] destination;
    public Transform currentDestination;
    public float distanceToDestination;
    public float distanceThreshold;
    
    [Header("Variables")]
    public Transform cashierCounter;
    public float selectItemToPurchaseDuration = 10f;
    public bool agentHasDestination;
    public bool itemToPurchaseSelected;

    [Header("Private")]
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.stoppingDistance = distanceThreshold;
        SetDestination();
    }

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

                if(!itemToPurchaseSelected)
                {
                    StartCoroutine(PickItemToPurchase());
                }
            }
        }
    }

    public void SetDestination()
    {
        int randomDestination = Random.Range(0,destination.Length);
        currentDestination = destination[randomDestination];
        agentHasDestination = true;
    }

    IEnumerator PickItemToPurchase()
    {
        yield return new WaitForSeconds(selectItemToPurchaseDuration);
        currentDestination = cashierCounter;
        agentHasDestination = true;
        itemToPurchaseSelected = true;
    }
}
