using UnityEngine;
using UnityEditor;

namespace LatteGames
{
    [CustomEditor(typeof(Bezier))]
    public class BezierEditor : Editor
    {
        private int selectPoint = 0;
        private bool globalMode;
        private bool positionControlMode = false;
        private bool PositionHandleControl => positionControlMode;
        private bool FreeMoveHandleControl => !PositionHandleControl;

        private void OnSceneGUI()
        {
            var bezier = (Bezier)target;
            void DrawHandles(EventType eventType)
            {
                for (int i = 0; i < bezier.controlPoints.Count; i++)
                {
                    Handles.color = new Color(0.1f, 0.8f, 0.2f);
                    var position = bezier.transform.TransformPoint(bezier.controlPoints[i].position);
                    var rotation = bezier.transform.rotation * bezier.controlPoints[i].OutRotation;
                    Handles.SphereHandleCap(
                        0,
                        position,
                        bezier.transform.rotation * bezier.controlPoints[i].InRotation,
                        HandleUtility.GetHandleSize(position) * 0.2f,
                        eventType
                    );

                    if (i < bezier.controlPoints.Count - 1 || bezier.loop)
                    {
                        var p0 = bezier.controlPoints[i];
                        var p1 = bezier.controlPoints[(i + 1) % bezier.controlPoints.Count];
                        var startP = p0.position;
                        var endP = p1.position;
                        var inTan = bezier.controlPoints[i].OutTangentPosition;
                        var outTan = bezier.controlPoints[i + 1].InTangentPosition;
                        Handles.DrawBezier(
                            bezier.transform.TransformPoint(startP),
                            bezier.transform.TransformPoint(endP),
                            bezier.transform.TransformPoint(inTan),
                            bezier.transform.TransformPoint(outTan),
                            new Color(0.1f, 0.8f, 0.2f),
                            null,
                            10f
                        );
                    }

                    Handles.DrawLine(position, position + rotation * Vector3.up * HandleUtility.GetHandleSize(position));
                }
            }
            if (Event.current.type == EventType.Repaint)
                DrawHandles(EventType.Repaint);
            if (Event.current.type == EventType.Layout)
                DrawHandles(EventType.Layout);
            if (Event.current.type == EventType.MouseDown)
            {

                Vector3 mousePosition = Event.current.mousePosition;
                Ray mouseRay = HandleUtility.GUIPointToWorldRay(mousePosition);
                mousePosition = mouseRay.origin;

                float closestDist = -1;
                void UpdateControlPoint(int i, float dis)
                {
                    if (dis >= closestDist && closestDist != -1)
                        return;
                    if (dis > 0.2f)
                        return;
                    selectPoint = i;
                    closestDist = dis;
                }
                float distanceToMouse(Vector3 wPosition)
                {
                    var mouse2Obj = wPosition - mousePosition;
                    return (mouse2Obj - mouseRay.direction * Vector3.Dot(mouse2Obj, mouseRay.direction)).magnitude * HandleUtility.GetHandleSize(wPosition);
                }
                if (selectPoint >= 0 && selectPoint < bezier.controlPoints.Count)
                {
                    var p = bezier.controlPoints[selectPoint];
                    var pos = distanceToMouse(bezier.transform.TransformPoint(p.position));
                    var inD = distanceToMouse(bezier.transform.TransformPoint(p.InTangentPosition));
                    var outD = distanceToMouse(bezier.transform.TransformPoint(p.OutTangentPosition));
                    UpdateControlPoint(selectPoint, pos);
                    UpdateControlPoint(selectPoint, inD);
                    UpdateControlPoint(selectPoint, outD);
                }
                for (int i = 0; i < bezier.controlPoints.Count; i++)
                {
                    var dis = distanceToMouse(bezier.transform.TransformPoint(bezier.controlPoints[i].position));
                    UpdateControlPoint(i, dis + Mathf.Epsilon);
                }
            }
            if (selectPoint >= 0 && selectPoint < bezier.controlPoints.Count && bezier.controlPoints.Count > 1)
            {
                var p = bezier.controlPoints[selectPoint];

                var position = bezier.transform.TransformPoint(p.position);

                var inPos = bezier.transform.TransformPoint(p.inTangentHandle);
                var inRot = bezier.transform.rotation * p.InRotation;

                var outPos = bezier.transform.TransformPoint(p.outTangentHandle);
                var outRot = bezier.transform.rotation * p.OutRotation;

                var controlHandleSize = 1;//HandleUtility.GetHandleSize(position);

                Handles.DrawLine(position, inPos);
                Handles.DrawLine(position, outPos);
                Handles.DrawLine(position, position + outRot * Vector3.up * controlHandleSize);

                Quaternion discControl(Vector3 discPos, Quaternion discRot)
                {
#if UNITY_2022_3_OR_NEWER
                    var newHandlePos = Handles.FreeMoveHandle(
                        discPos + discRot * Vector3.up * controlHandleSize * 0.5f,
                        controlHandleSize * 0.05f,
                        Vector3.zero,
                        Handles.CircleHandleCap);
#else
                    var newHandlePos = Handles.FreeMoveHandle(
                        discPos + discRot * Vector3.up * controlHandleSize * 0.5f,
                        Quaternion.identity,
                        controlHandleSize * 0.05f,
                        Vector3.zero,
                        Handles.CircleHandleCap);
#endif
                    Handles.DrawWireDisc(discPos, discRot * Vector3.forward, controlHandleSize * 0.5f);
                    var disp = newHandlePos - discPos;
                    disp -= Vector3.Dot(disp, discRot * Vector3.forward) * Vector3.forward;
                    Handles.DrawLine(discPos, newHandlePos);
                    return Quaternion.LookRotation(discRot * Vector3.forward, disp);
                }

                Vector3 PositionHandleCustom(Vector3 pos, Quaternion rot)
                {
                    if (PositionHandleControl)
                        return Handles.PositionHandle(pos, globalMode ? Quaternion.identity : rot);
                    if (FreeMoveHandleControl)
#if UNITY_2022_3_OR_NEWER
                        return Handles.FreeMoveHandle(pos, 0.2f * HandleUtility.GetHandleSize(pos), Vector3.zero, Handles.SphereHandleCap);
#else
                        return Handles.FreeMoveHandle(pos, Quaternion.identity, 0.2f * HandleUtility.GetHandleSize(pos), Vector3.zero, Handles.SphereHandleCap);
#endif
                    return position;
                }
                Handles.color = Color.red;
                var newPosition = PositionHandleCustom(position, outRot);
                var newPosOffset = newPosition - position;

                Handles.color = Color.blue;
                var newInPos = PositionHandleCustom(inPos, inRot) + newPosOffset;
                var newInRot = inRot;
                if ((newInPos - newPosition).magnitude > Mathf.Epsilon)
                    newInRot = Quaternion.LookRotation((newPosition - newInPos).normalized, inRot * Vector3.up);
                newInRot = discControl(newInPos, newInRot);

                var newOutPos = PositionHandleCustom(outPos, outRot) + newPosOffset;
                var newOutRot = outRot;
                if ((newOutPos - newPosition).magnitude > Mathf.Epsilon)
                    newOutRot = Quaternion.LookRotation((newOutPos - newPosition).normalized, outRot * Vector3.up);
                newOutRot = discControl(newOutPos, newOutRot);

                var inTanChange = Quaternion.Angle(newInRot, inRot) > 0.0f;
                var outTanChange = Quaternion.Angle(newOutRot, outRot) > 0.0f;

                if (newPosition != position
                || newInPos != inPos
                || inTanChange
                || newOutPos != outPos
                || outTanChange)
                {
                    Undo.RecordObject(target, "Bezier Control point");
                    p.position = bezier.transform.InverseTransformPoint(newPosition);

                    p.inTangentHandle = bezier.transform.InverseTransformPoint(newInPos);
                    p.outTangentHandle = bezier.transform.InverseTransformPoint(newOutPos);

                    p.inTangentTwistHandle = bezier.transform.InverseTransformPoint(newInPos + newInRot * Vector3.up);
                    p.outTangentTwistHandle = bezier.transform.InverseTransformPoint(newOutPos + newOutRot * Vector3.up);

                    if (!p.broken && inTanChange)
                    {
                        float l = (p.outTangentHandle - p.position).magnitude;
                        p.outTangentHandle = p.position + p.InRotation * Vector3.forward * l;
                        p.outTangentTwistHandle = p.outTangentHandle + p.InRotation * Vector3.up;
                    }
                    if (!p.broken && outTanChange)
                    {
                        float l = (p.inTangentHandle - p.position).magnitude;
                        p.inTangentHandle = p.position - p.OutRotation * Vector3.forward * l;
                        p.inTangentTwistHandle = p.inTangentHandle + p.OutRotation * Vector3.up;
                    }
                    EditorUtility.SetDirty(target);
                }
            }
            if (Event.current.type == EventType.KeyDown && selectPoint >= 0 && selectPoint < bezier.controlPoints.Count)
            {
                var keyCode = Event.current.keyCode;
                if (keyCode == KeyCode.D)
                {
                    Undo.RecordObject(target, "Poly path insert point");
                    bool append = true;
                    var newPoint = new Bezier.Point();
                    newPoint.position = bezier.controlPoints[selectPoint].position + bezier.controlPoints[selectPoint].InRotation * Vector3.forward;
                    newPoint.inTangentHandle = newPoint.position + bezier.controlPoints[selectPoint].InRotation * Vector3.back;
                    newPoint.outTangentHandle = newPoint.position + bezier.controlPoints[selectPoint].OutRotation * Vector3.forward;
                    newPoint.inTangentTwistHandle = newPoint.inTangentHandle + bezier.controlPoints[selectPoint].InRotation * Vector3.up;
                    newPoint.outTangentTwistHandle = newPoint.outTangentHandle + bezier.controlPoints[selectPoint].OutRotation * Vector3.up;

                    if (bezier.controlPoints.Count > 1 && selectPoint < bezier.controlPoints.Count - 1)
                    {
                        var previousIndex = (selectPoint - 1 + bezier.controlPoints.Count) % bezier.controlPoints.Count;
                        var previousPoint = bezier.controlPoints[previousIndex];
                        var currentPoint = bezier.controlPoints[selectPoint];

                        newPoint.position = bezier.BezierPathCalculation(
                            previousPoint.position,
                            previousPoint.outTangentHandle,
                            currentPoint.inTangentHandle,
                            currentPoint.position,
                            0.5f
                        );

                        var inRotation = Quaternion.Lerp(previousPoint.OutRotation, currentPoint.InRotation, 0.5f);
                        newPoint.inTangentHandle = newPoint.position + inRotation * Vector3.back;
                        newPoint.outTangentHandle = newPoint.position + inRotation * Vector3.forward;
                        newPoint.inTangentTwistHandle = newPoint.inTangentHandle + inRotation * Vector3.up;
                        newPoint.outTangentTwistHandle = newPoint.outTangentHandle + inRotation * Vector3.up;
                        append = false;
                    }
                    if (append)
                        bezier.controlPoints.Add(newPoint);
                    else
                        bezier.controlPoints.Insert(selectPoint, newPoint);
                    EditorUtility.SetDirty(target);
                }
                if (keyCode == KeyCode.X)
                {
                    Undo.RecordObject(target, "Poly path remove point");
                    bezier.controlPoints.RemoveAt(selectPoint);
                    EditorUtility.SetDirty(target);
                }
            }
            Handles.BeginGUI();
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, new Color(0.3f, 0.3f, 0.3f));
            texture.Apply();
            GUI.skin.box.normal.background = texture;
            var rectBox = new Rect(10, 10, 110, 60);
            GUI.Box(rectBox, GUIContent.none);
            GUIStyle contentStyle = new GUIStyle();
            contentStyle.padding = new RectOffset(5, 5, 5, 5);
            GUILayout.BeginArea(rectBox, GUIContent.none, contentStyle);
            if (selectPoint >= 0 && selectPoint < bezier.controlPoints.Count)
                bezier.controlPoints[selectPoint].broken = GUILayout.Toggle(bezier.controlPoints[selectPoint].broken, "Broken");
            globalMode = GUILayout.Toggle(globalMode, "Global");
            positionControlMode = GUILayout.Toggle(positionControlMode, "Handle style");
            GUILayout.EndArea();
            Handles.EndGUI();

            if (Event.current.type == EventType.KeyDown)
            {
                var keyCode = Event.current.keyCode;
                if (keyCode == KeyCode.G)
                {
                    globalMode = !globalMode;
                }
                if (keyCode == KeyCode.C)
                {
                    positionControlMode = !positionControlMode;
                }
            }
        }
    }
}