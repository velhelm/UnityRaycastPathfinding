using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaycastPathfinding : MonoBehaviour {

    [SerializeField] Material touched;
    [SerializeField] Material obstacle;
    [SerializeField] Transform target;

    List<Vector3> targets = new List<Vector3>();    
    Vector3 DirectionToTarget, StartPos, CurrentPos;
    float t = 0;

    bool pathComplete = false;
    int currentTarget = 0;

    void Awake()
    {
        DirectionToTarget = (target.position - transform.position).normalized;
        StartPos = transform.position;
    }

    void Start()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, DirectionToTarget, out hit, 100f))
        {
            if (hit.collider.tag == "Finish")
            {
                Debug.Log("Trivial Solution");
                targets.Add(target.position);
                pathComplete = true;
            }
            else if (hit.collider.tag == "tile")
            {
                hit.collider.GetComponent<MeshRenderer>().material = obstacle;
                RaycastHit subHit;
                if (Physics.Raycast(hit.transform.position - DirectionToTarget, Vector3.forward, out subHit, 2f))
                {
                    targets.Add(subHit.transform.position - Vector3.forward * subHit.transform.position.z + Vector3.forward * transform.position.z);
                }
            }
        }
    }

    void FixedUpdate()
    { 
        if (pathComplete)
        {

            Vector3 prev = (currentTarget == 0) ? StartPos : targets[currentTarget - 1];
            t += .1f/(prev - targets[currentTarget]).magnitude;
            transform.position = Vector3.Lerp(prev, targets[currentTarget], t);
            if (t >=1)
            {
                if (currentTarget < targets.Count - 1)
                {
                    currentTarget++;
                    t = 0f;
                }
            }
        }
        else
        {
            ScanPerimeter();
            DirectionToTarget = (target.position - targets[targets.Count - 1]).normalized;
            if (CheckForWall(DirectionToTarget, 100f))
            {
                //Debug.Log("Wall in path: " + targets.Count.ToString()); 
            }
        }


	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "tile")
        {
            other.GetComponent<MeshRenderer>().material = touched;
        }
    }

    void ScanPerimeter()
    {
        if (!CheckForWall(Vector3.up)) { return; }
        if (!CheckForWall(Vector3.up + Vector3.right)) { return; }
        if (!CheckForWall(Vector3.right)) { return; }
        if (!CheckForWall(Vector3.right + Vector3.down)) { return; }
        if (!CheckForWall(Vector3.down)) { return; }
        if (!CheckForWall(Vector3.down + Vector3.left)) { return; }
        if (!CheckForWall(Vector3.left)) { return; }
        if (!CheckForWall(Vector3.left + Vector3.up)) { return; }
    }
    
    bool CheckForWall(Vector3 direction, float dist = 1)
    {
        RaycastHit hit;
        //Debug.Log(targets[targets.Count - 1]);
        if (Physics.Raycast(targets[targets.Count - 1], direction, out hit, dist))
        {
            
            if (hit.collider.tag == "Finish")
            {
                targets.Add(hit.transform.position);
                pathComplete = true;
                Debug.Log("Path Complete");
                Debug.Log("Nodes: " + targets.Count);
                return false;
            }
            else
            {
                if (hit.collider.tag == "tile") { hit.collider.GetComponent<MeshRenderer>().material = obstacle; }
                return true;
            }
        }
        else
        {
            targets.Add(targets[targets.Count-1] + direction.normalized);
            if (Physics.Raycast(targets[targets.Count-1], Vector3.forward, out hit, dist))
            {
                if (hit.collider.tag == "tile")
                {
                    hit.collider.GetComponent<MeshRenderer>().material = touched;
                }
            }

            return false;
        }
    }

}
