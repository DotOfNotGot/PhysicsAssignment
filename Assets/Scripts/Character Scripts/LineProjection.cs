using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class LineProjection : MonoBehaviour
{
    private LineRenderer _line;

    [SerializeField, Range(10, 100)] private int linePoints = 20;
    [SerializeField, Range(0.01f, 0.25f)] private float timeBetweenPoints = 0.1f;

    // Start is called before the first frame update
    void Awake()
    {
        _line = GetComponent<LineRenderer>();
        _line.alignment = LineAlignment.View;
        _line.enabled = false;
    }

    public void CalculateTrajectory(float force, Vector3 dir, Collider ballCollider, Rigidbody ballRb, bool shouldLob)
    {
        _line.enabled = true;
        _line.positionCount = Mathf.CeilToInt(linePoints / timeBetweenPoints) + 1;
        Vector3 startPos = ballCollider.transform.position;
        Vector3 startVel = force * dir.normalized / ballRb.mass;
        int i = 0;
        _line.SetPosition(i, startPos);

        for (float t = 0; t < linePoints; t += timeBetweenPoints)
        {
            i++;
            Vector3 point = startPos + startVel * t;

            if (shouldLob)
            {
                point.y = (startPos.y + startVel.y * t + (Physics.gravity.y / 2f * t * t));
            }

            Vector3 lastPos = _line.GetPosition(i - 1);


            if (Physics.BoxCast(point, 
                    ballCollider.bounds.extents / 2,
                    (point - lastPos).normalized, 
                    out var hitInfo,
                    quaternion.identity, 
                    Vector3.Distance(point, lastPos))
                &&
                !hitInfo.collider.Equals(ballCollider))
            {
                _line.SetPosition(i, hitInfo.point);
                _line.positionCount = i + 1;
                return;
            }
            else
            {
                _line.SetPosition(i, point);
            }
        }
    }

    public void LineEnabled(bool value)
    {
        _line.enabled = value;
        _line.positionCount = 0;
    }
}