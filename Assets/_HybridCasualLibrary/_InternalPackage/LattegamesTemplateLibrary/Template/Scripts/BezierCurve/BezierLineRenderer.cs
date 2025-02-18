using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
    [RequireComponent(typeof(Bezier))]
    [RequireComponent(typeof(LineRenderer))]
    [ExecuteInEditMode]
    public class BezierLineRenderer : MonoBehaviour
    {
        [Min(1)] public int segmentCount = 20;
        private int SegmentCount => Math.Max(segmentCount, 1);
        public bool autoUpdate = false;

        private Bezier _bezier;
        private Bezier bezier {
            get{
                if(_bezier == null)
                    _bezier = GetComponent<Bezier>();
                return _bezier;
            }
        }

        private LineRenderer _lineRenderer;
        private LineRenderer lineRenderer{
            get{
                if(_lineRenderer == null)
                    _lineRenderer = GetComponent<LineRenderer>();
                return _lineRenderer;
            }
        }

        private void Update() {
            if(Application.isPlaying && !autoUpdate)
                return;
            Rebuild();
        }

        public void Rebuild()
        {
            lineRenderer.positionCount = (bezier.controlPoints.Count - 1)*(SegmentCount + 1) + (bezier.loop?(SegmentCount + 1):0);
            lineRenderer.loop = bezier.loop;
            lineRenderer.useWorldSpace = false;
            for (int i = 0; i < bezier.controlPoints.Count; i++)
            {
                if(!bezier.loop && i == bezier.controlPoints.Count - 1)
                    continue;
                var p0 = bezier.controlPoints[i];
                var p1 = bezier.controlPoints[(i + 1)%bezier.controlPoints.Count];
                for (int j = 0; j <= SegmentCount; j++)
                {
                    var position = bezier.BezierPathCalculation(p0.position, p0.OutTangentPosition, p1.InTangentPosition, p1.position, j*1.0f/SegmentCount);
                    lineRenderer.SetPosition(i*(SegmentCount + 1) + j, position);
                }
            }
        }
    }
}