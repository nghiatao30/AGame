using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyPath : MonoBehaviour
{
    [Serializable]
    public class Point{
        public Vector3 position;
        public Vector3 rotation;
    }
    public List<Point> points = new List<Point>(){
        new Point(){position = new Vector3(0,0,0)},
        new Point(){position = new Vector3(0,0,1)}
    };

    public List<Vector3> GetWorldPosition()
    {
        return points.ConvertAll(p => transform.TransformPoint(p.position));
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = new Color(0.1f, 0.8f, 0.2f);
        for (int i = 0; i < points.Count - 1; i++)
        {
            Gizmos.DrawLine(
                transform.TransformPoint(points[i].position),
                transform.TransformPoint(points[i+1].position)
            );
        }
        if(points.Count > 2)
        {
            Gizmos.color = new Color(0.8f, 0.1f, 0.2f);
            Gizmos.DrawLine(
                transform.TransformPoint(points[points.Count - 1].position),
                transform.TransformPoint(points[0].position)
            );
        }
    }
}
