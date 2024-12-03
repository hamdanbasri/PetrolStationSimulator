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

    [Header("CustomerInventory")]
    public int itemToPurchaseAmount;
    public Transform itemSpawnPoint;
    public CustomerSelection customerSelection;
    
    [Header("Variables")]
    public Transform cashierCounter;
    public float selectItemToPurchaseDuration = 10f;
    public bool agentHasDestination;
    public bool itemToPurchaseSelected;
    public bool isWaitingInLine;

    [Header("Private")]
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        customerSelection = GetComponent<CustomerSelection>();
        itemSpawnPoint = GameObject.Find("ItemSpawnPoint").transform;
        selectItemToPurchaseDuration = GameManager.Instance.itemSelectionDuration;
        cashierCounter = GameObject.Find("CashierLine_1").transform;
        //agent.stoppingDistance = distanceThreshold;

        //Populating the destinations
        GameObject[] inventoryCollectors = GameObject.FindGameObjectsWithTag("InventoryCollector");
        destination = new Transform[inventoryCollectors.Length];
        for (int i = 0; i < inventoryCollectors.Length; i++)
        {
            destination[i] = inventoryCollectors[i].transform;
        }

        itemToPurchaseAmount = Random.Range(1,2);
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
                else
                {
                    CheckCashRegisterAvailability();
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

        if (customerSelection.customerInventory.Count == itemToPurchaseAmount)
        {
            currentDestination = cashierCounter;
            agentHasDestination = true;
            itemToPurchaseSelected = true;
        }
        else
        {
            SetDestination();
        }
    }

    IEnumerator WaitInLine()
    {
        isWaitingInLine = true;

        yield return new WaitForSeconds(2);
        CheckCashRegisterAvailability();

        isWaitingInLine = false;
    }

    public void CheckCashRegisterAvailability()
    {
        if (!GameManager.Instance.cashRegisterOccupied)
        {
            for(int i = 0; i < customerSelection.customerInventory.Count; i++)
            {
                customerSelection.customerInventory[i].GetComponent<Rigidbody>().isKinematic = false;
                Instantiate(customerSelection.customerInventory[i], itemSpawnPoint.position, Quaternion.identity);
                Debug.Log("Instantiated");
            }

        }
        else
        {
            if(!isWaitingInLine)
            {
                StartCoroutine(WaitInLine());
            }
        }
    }
}
