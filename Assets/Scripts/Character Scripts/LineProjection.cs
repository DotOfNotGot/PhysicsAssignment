using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineProjection : MonoBehaviour
{

    private LineRenderer _line;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalculateTrajectory(float force, Vector3 dir, Collider ballCollider, int maxSteps)
    {
        // TODO: Calculate the trajectory of the ball to then project a line.

        RaycastHit hitInfo;

        Physics.Raycast(ballCollider.bounds.min, dir, out hitInfo);
        Physics.Raycast(ballCollider.bounds.max, dir, out hitInfo);
    }
    
}
