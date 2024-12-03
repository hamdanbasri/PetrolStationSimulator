using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Cashier")]
    public bool cashRegisterOccupied;

    [Header("Customer")]
    public float itemSelectionDuration;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Assign the instance
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }
}
