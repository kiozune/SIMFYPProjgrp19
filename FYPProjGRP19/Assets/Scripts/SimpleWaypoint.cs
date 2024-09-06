using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // add this

public class SimpleWaypoint : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    private int currentWaypoint = 0;
    private NavMeshAgent agent;



    // Start is called before the first frame update
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(waypoints[currentWaypoint].position);
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (Vector3.Distance(transform.position, waypoints
        [currentWaypoint].transform.position) <2.0f)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
            {
                currentWaypoint = 0;
            }
            agent.SetDestination(waypoints[currentWaypoint].position);
        }
        
    }
}
