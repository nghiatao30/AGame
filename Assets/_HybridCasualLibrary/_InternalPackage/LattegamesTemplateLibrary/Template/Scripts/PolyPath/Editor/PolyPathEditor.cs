using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LatteGames
{
    [CustomEditor(typeof(PolyPath))]
    public class PolyPathEditor : Editor
    {
        private int selectPoint = 0;
        private bool globalMode = false;
        private void OnSceneGUI() {
            var polyPath = (PolyPath)target;
            void DrawHandles(EventType eventType)
            {
                for (int i = 0; i < polyPath.points.Count; i++)
                {
                    Handles.color = new Color(0.1f, 0.8f, 0.2f);
                    var position = polyPath.transform.TransformPoint(polyPath.points[i].position);
                    Handles.SphereHandleCap(
                        0,
                        position,
                        Quaternion.Euler(polyPath.transform.TransformVector(polyPath.points[i].rotation)),
                        HandleUtility.GetHandleSize(position)*0.2f,
                        eventType
                    );
                }
            }
            if (Event.current.type == EventType.Repaint)
                DrawHandles(EventType.Repaint);
            if(Event.current.type == EventType.Layout)
                DrawHandles(EventType.Layout);
            if(Event.current.type == EventType.MouseDown)
            {
                Vector3 mousePosition = Event.current.mousePosition;
                Ray mouseRay = HandleUtility.GUIPointToWorldRay(mousePosition);
                mousePosition = mouseRay.origin;

                float closestDist = -1;
                void UpdateControlPoint(int i, float dis)
                {
                    if(dis > closestDist && closestDist != -1)
                        return;
                    if(dis > 0.2f)
                        return;
                    selectPoint = i;
                    closestDist = dis;
                }
                float distanceToMouse(Vector3 wPosition)
                {
                    var mouse2Obj = wPosition - mousePosition;
                    return (mouse2Obj - mouseRay.direction*Vector3.Dot(mouse2Obj, mouseRay.direction)).magnitude * HandleUtility.GetHandleSize(wPosition);
                }
                
                for (int i = 0; i < polyPath.points.Count; i++)
                {
                    var dis = distanceToMouse(polyPath.transform.TransformPoint(polyPath.points[i].position));
                    UpdateControlPoint(i, dis);    
                }
            }
            if(selectPoint >= 0 && selectPoint < polyPath.points.Count)
            {
                var position = polyPath.transform.TransformPoint(polyPath.points[selectPoint].position);
                var rotation = Quaternion.Euler(polyPath.transform.TransformVector(polyPath.points[selectPoint].rotation));
                position = Handles.PositionHandle(position, globalMode?Quaternion.identity:rotation);
                rotation = Handles.RotationHandle(rotation, position);
                var newPosition = polyPath.transform.InverseTransformPoint(position);
                var newRotation = polyPath.transform.InverseTransformVector(rotation.eulerAngles);
                if(newPosition != polyPath.points[selectPoint].position
                || newRotation != polyPath.points[selectPoint].rotation)
                {
                    Undo.RecordObject(target, "Poly path change point");
                    polyPath.points[selectPoint].position = newPosition;
                    polyPath.points[selectPoint].rotation = newRotation;
                    EditorUtility.SetDirty(target);
                }
            }
            if(Event.current.type == EventType.KeyDown && selectPoint >= 0 && selectPoint < polyPath.points.Count)
            {
                var keyCode = Event.current.keyCode;
                if(keyCode == KeyCode.D)
                {
                    Undo.RecordObject(target, "Poly path insert point");
                    
                    var newPoint = new PolyPath.Point(){
                        position = polyPath.points[selectPoint].position + Quaternion.Euler(polyPath.points[selectPoint].rotation)*Vector3.forward,
                        rotation = polyPath.points[selectPoint].rotation
                    };
                    if(polyPath.points.Count > 1 && selectPoint < polyPath.points.Count - 1)
                    {
                        var previousIndex = (selectPoint - 1 + polyPath.points.Count)%polyPath.points.Count;
                        newPoint.rotation = Vector3.Slerp(polyPath.points[previousIndex].rotation, polyPath.points[selectPoint].rotation, 0.5f);
                        newPoint.position = Vector3.Lerp(polyPath.points[previousIndex].position, polyPath.points[selectPoint].position, 0.5f);
                    }
                    polyPath.points.Insert(selectPoint, newPoint);
                    EditorUtility.SetDirty(target);
                }
                if(keyCode == KeyCode.X)
                {
                    Undo.RecordObject(target, "Poly path remove point");
                    polyPath.points.RemoveAt(selectPoint);
                    EditorUtility.SetDirty(target);
                }
                if(keyCode == KeyCode.G)
                {
                    globalMode = !globalMode;
                }
            }
        }
    }
}