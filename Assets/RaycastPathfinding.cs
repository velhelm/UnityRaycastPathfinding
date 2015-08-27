using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaycastPathfinding : MonoBehaviour {

    [SerializeField] Material touched;
    [SerializeField] Material obstacle;
    [SerializeField] Transform target;
    [SerializeField]
    bool Use3d = false;
    int size;

    List<Vector3> targets = new List<Vector3>();
    List<Vector3> deadEnds = new List<Vector3>();

    Vector3 DirectionToTarget, StartPos, CurrentPos;
    float t = 0;

    bool pathComplete = false, initialized = false;
    int currentTarget = 0;

    void Awake()
    {
        DirectionToTarget = (target.position - transform.position).normalized;
        StartPos = transform.position;
    }

    void Start()
    {
        size = (Use3d) ? 26 : 8;

        //Initialize();
    }

    void Initialize()
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
            else if (hit.collider.tag == "Obstacle")
            {
                hit.collider.GetComponent<MeshRenderer>().material = obstacle;
                targets.Add(transform.position);
                Debug.Log("Calculating");
            }
        }
        initialized = true;
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
        else if (initialized)
        {
            if (targets.Count > 0)
            {
                ScanPerimeter();
            
                DirectionToTarget = (target.position - targets[targets.Count - 1]).normalized;
                Vector3 exitScan = CheckForWall(DirectionToTarget, 100f);
                if (exitScan == target.position) { targets.Add(exitScan); }
            }
            else
            {
                Debug.Log("NO SOLUTION FOUND");
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            targets.Clear();
            deadEnds.Clear();
            pathComplete = false;
            currentTarget = 0;
            Initialize();
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obstacle")
        {
            other.GetComponent<MeshRenderer>().material = (other.GetComponent<MeshRenderer>().material == obstacle) ? obstacle : touched;
        }
    }

    void ScanPerimeter()
    {
        Vector3[] adjList = new Vector3[size];

        adjList[0] = CheckForWall(Vector3.forward);
        adjList[1] = CheckForWall(Vector3.right);
        adjList[2] = CheckForWall(Vector3.back);
        adjList[3] = CheckForWall(Vector3.left);
        adjList[4] = CheckForWall(Vector3.forward + Vector3.right);
        adjList[5] = CheckForWall(Vector3.right + Vector3.back);
        adjList[6] = CheckForWall(Vector3.back + Vector3.left);
        adjList[7] = CheckForWall(Vector3.left + Vector3.forward);

        if (Use3d)
        {
            adjList[8] = CheckForWall(Vector3.up);
            adjList[9] = CheckForWall(Vector3.down);

            adjList[10] = CheckForWall(Vector3.up + Vector3.forward);
            adjList[11] = CheckForWall(Vector3.up + Vector3.forward + Vector3.right);
            adjList[12] = CheckForWall(Vector3.up + Vector3.right);
            adjList[13] = CheckForWall(Vector3.up + Vector3.right + Vector3.back);
            adjList[14] = CheckForWall(Vector3.up + Vector3.back);
            adjList[15] = CheckForWall(Vector3.up + Vector3.back + Vector3.left);
            adjList[16] = CheckForWall(Vector3.up + Vector3.left);
            adjList[17] = CheckForWall(Vector3.up + Vector3.left + Vector3.forward);

            adjList[18] = CheckForWall(Vector3.down + Vector3.forward);
            adjList[19] = CheckForWall(Vector3.down + Vector3.forward + Vector3.right);
            adjList[20] = CheckForWall(Vector3.down + Vector3.right);
            adjList[21] = CheckForWall(Vector3.down + Vector3.right + Vector3.back);
            adjList[22] = CheckForWall(Vector3.down + Vector3.back);
            adjList[23] = CheckForWall(Vector3.down + Vector3.back + Vector3.left);
            adjList[24] = CheckForWall(Vector3.down + Vector3.left);
            adjList[25] = CheckForWall(Vector3.down + Vector3.left + Vector3.forward);
        }

        float min = float.MaxValue;
        int closestToTarget = -1;

        for (int i=0; i<size; i++)
        {
            if (targets.Contains(adjList[i]) || adjList[i] == transform.position)
            {
                //Debug.LogWarning("Adjacency List already contains " + adjList[i].ToString());
                continue;
            }

            if (deadEnds.Contains(adjList[i]))
            {
                //Debug.LogWarning("Dead end reached... Continuing");
                continue;
            }

            if ((target.position - adjList[i]).sqrMagnitude < min)
            {
                min = (target.position - adjList[i]).sqrMagnitude;
                closestToTarget = i;
            }
        }

        if (closestToTarget == -1)
        {
            //Debug.LogWarning("Deadend Reached!");
            deadEnds.Add(targets[targets.Count - 1]);
            targets.RemoveAt(targets.Count - 1);
        }
        else
        { targets.Add(adjList[closestToTarget]); }
    }
    
    Vector3 CheckForWall(Vector3 direction, float dist = 1)
    {
        RaycastHit hit;
        //Debug.Log(targets[targets.Count - 1]);
        if (Physics.Raycast(targets[targets.Count - 1], direction, out hit, dist))
        {
            
            if (hit.collider.tag == "Finish")
            {
                pathComplete = true;
                Debug.Log("Path Complete");
                Debug.Log("Nodes: " + targets.Count);
                return hit.transform.position;
            }
            else
            {
                if (hit.collider.tag == "Obstacle") { hit.collider.GetComponent<MeshRenderer>().material = obstacle; }
                return transform.position;
            }
        }
        else
        {
            return targets[targets.Count - 1] + direction;
        }
    }

}
