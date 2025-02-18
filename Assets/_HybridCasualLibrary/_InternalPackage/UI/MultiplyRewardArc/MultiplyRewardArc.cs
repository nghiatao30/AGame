using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MultiplyRewardArc : MonoBehaviour
{
    [SerializeField] protected RectTransform arcBound;
    [SerializeField] protected List<SegmentInfo> segments;
    [SerializeField] protected Image arrowImg;
    [SerializeField] protected float runSpeed;
    [SerializeField] protected AnimationCurve runCurve;

    protected Coroutine startRunCoroutine;
    protected bool isRunning;
    protected float multiplierResult = 1;

    public float MultiplierResult { get => multiplierResult; }

    protected Vector3 GetBezierPos(Vector3 startPos, Vector3 midPos, Vector3 endPos, float value)
    {
        var bezierPos_1 = Vector3.Lerp(startPos, midPos, value);
        var bezierPos_2 = Vector3.Lerp(midPos, endPos, value);
        return Vector3.Lerp(bezierPos_1, bezierPos_2, value);
    }

    protected Vector3 GetNormalAtBezierPoint(Vector3 startPos, Vector3 midPos, Vector3 endPos, float value)
    {
        var currentPos = GetBezierPos(startPos, midPos, endPos, value);
        var adjacencyPos = GetBezierPos(startPos, midPos, endPos, value + (value > 0.5f ? -0.01f : 0.01f));
        var tangent = value > 0.5f ? adjacencyPos - currentPos : currentPos - adjacencyPos;
        return Vector3.Cross(tangent, Vector3.forward);
    }

    [Button, HideInEditorMode]
    public virtual void StartRun()
    {
        if (startRunCoroutine != null)
        {
            StopCoroutine(startRunCoroutine);
        }
        startRunCoroutine = StartCoroutine(CR_StartRun());
    }

    [Button, HideInEditorMode]
    public virtual void StopRun()
    {
        isRunning = false;
    }

    protected virtual IEnumerator CR_StartRun()
    {
        isRunning = true;
        Vector3[] v = new Vector3[4];
        arcBound.GetWorldCorners(v);
        var startPos = new Vector3(v[0].x, v[0].y, transform.position.z);
        var endPos = new Vector3(v[3].x, v[0].y, transform.position.z);
        var midPos = new Vector3(transform.position.x, v[1].y, transform.position.z);
        var t = 0f;
        var runningValue = 0f;
        while (isRunning)
        {
            t += Time.deltaTime * runSpeed;
            runningValue = runCurve.Evaluate(Mathf.PingPong(t, 1f));
            var currentPos = GetBezierPos(startPos, midPos, endPos, runningValue);
            var adjacencyPos = GetBezierPos(startPos, midPos, endPos, runningValue + (runningValue > 0.5f ? -0.01f : 0.01f));
            var tangent = runningValue > 0.5f ? adjacencyPos - currentPos : currentPos - adjacencyPos;
            var normal = Vector3.Cross(tangent, Vector3.forward);
            arrowImg.transform.position = currentPos;
            arrowImg.transform.rotation = Quaternion.LookRotation(Vector3.forward, normal);

            var totalWeight = segments.Sum(segment => segment.weight);
            var endNormalizedWeight = 0f;
            foreach (var segment in segments)
            {
                endNormalizedWeight += segment.weight / totalWeight;
                if (runningValue <= endNormalizedWeight)
                {
                    multiplierResult = segment.multiplier;
                    break;
                }
            }

            yield return null;
        }
    }

#if UNITY_EDITOR
    [SerializeField, FoldoutGroup("Draw Gizmos")] protected List<Color> segmentColor;
    [SerializeField, FoldoutGroup("Draw Gizmos")] protected float handleSphereRadius;
    [SerializeField, Range(0f, 1f), FoldoutGroup("Draw Gizmos")] protected float currentValue;

    protected void OnDrawGizmosSelected()
    {
        var totalWeight = segments.Sum(segment => segment.weight);
        Vector3[] v = new Vector3[4];
        arcBound.GetWorldCorners(v);
        var startPos = new Vector3(v[0].x, v[0].y, transform.position.z);
        var endPos = new Vector3(v[3].x, v[0].y, transform.position.z);
        var midPos = new Vector3(transform.position.x, v[1].y, transform.position.z);

        var endNormalizedWeight = 0f;
        for (var i = 0; i < segments.Count; i++)
        {
            var segment = segments[i];
            var startNormalizedWeight = endNormalizedWeight;
            endNormalizedWeight += segment.weight / totalWeight;
            var startLinePos = GetBezierPos(startPos, midPos, endPos, startNormalizedWeight);
            var endLinePos = GetBezierPos(startPos, midPos, endPos, endNormalizedWeight);
            Gizmos.color = segmentColor[i % segmentColor.Count];
            Gizmos.DrawLine(startLinePos, endLinePos);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(GetBezierPos(startPos, midPos, endPos, currentValue), handleSphereRadius);
    }
#endif
    [Serializable]
    public struct SegmentInfo
    {
        public float weight;
        public float multiplier;
    }
}
