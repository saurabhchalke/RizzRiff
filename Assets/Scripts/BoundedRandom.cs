using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoundedRandom : MonoBehaviour
{
    private Vector3 destination;
    public Vector3 min, max;

    private void Start()
    {
        destination = RandomDestination();
        GetComponent<NavMeshAgent>().SetDestination(destination);
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, destination) < 2.5f)
        {
            destination = RandomDestination();
            GetComponent<NavMeshAgent>().SetDestination(destination);
        }
    }

    private Vector3 RandomDestination()
    {
        return new Vector3(Random.Range(min.x, max.x), 2f, Random.Range(min.z, max.z));
    }
}
