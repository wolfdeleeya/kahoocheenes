using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CustomEditor(typeof(SplineController))]
public class SplineControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {

        var obj = (SplineController) target;
        if (GUILayout.Button("Recalculate Spline"))
            obj.RecalculateSpline();
        if (GUILayout.Button("Calculate Lengths"))
            obj.RecalculateLengths();
        if (GUILayout.Button("Recalculate Equidistant Spline"))
            obj.RecalculateEquidistantSpline();
        base.OnInspectorGUI();
    }
}

public class SplineController : MonoBehaviour
{
    [SerializeField] private List<Transform> splinePoints;
    [SerializeField] private int pointsSampleSize;
    [SerializeField, Range(0, 0.25f)] private float calculationStepLength;
    [SerializeField, Range(0, 0.25f)] private float firstProximityCheckStepLength;

    [SerializeField] private List<Vector3> calculatedPoints = new List<Vector3>();
    [SerializeField] private List<float> lengths = new List<float>();
    [SerializeField] private float totalLength;
    
    [Header("Debug properties")] [SerializeField]
    private int debugPointsSampleSize;

    [SerializeField] private Color debugLineColor;
    [SerializeField] private Color debugPointsSpheresColor;
    [SerializeField] private bool showPointSpheres;
    [SerializeField] private float debugPointSphereRadius;

    [SerializeField] private bool showTraveller;
    [SerializeField, Range(0, 1)] private float debugTravellerPosition;
    [SerializeField] private float debugTravellerSphereRadius;
    [SerializeField] private Color debugTravellerSphereColor;

    [SerializeField] private bool showProximityCheck;
    [SerializeField] private Transform debugTransformForProximityCheck;
    [SerializeField] private Color debugProximityPointColor;
    [SerializeField] private float debugProximityPointWidth;
    [SerializeField] private Color debugProximityCheckSphereColor;

    [SerializeField] private bool showCalculatedPath;
    [SerializeField] private Color calculatedPathColor;

    public Vector3 GetPoint(float t)
    {
        int i0, i1, i2, i3;
        int len = splinePoints.Count;

        i1 = (int) (t * len) % len;
        i0 = i1 == 0 ? len - 1 : i1 - 1;
        i2 = (i1 + 1) % len;
        i3 = (i2 + 1) % len;

        t = t * len - (int) (t * len);
        float tt = t * t, ttt = tt * t;

        float mul0 = -ttt + 2 * tt - t;
        float mul1 = 3 * ttt - 5 * tt + 2;
        float mul2 = -3 * ttt + 4 * tt + t;
        float mul3 = ttt - tt;

        Vector3 result = Vector3.zero;
        Vector3 p0 = splinePoints[i0].position,
            p1 = splinePoints[i1].position,
            p2 = splinePoints[i2].position,
            p3 = splinePoints[i3].position;

        result = mul0 * p0 + mul1 * p1 + mul2 * p2 + mul3 * p3;
        result /= 2;

        return result;
    }

    public Vector3 GetEquidistantPoint(float t)
    {
        float lenOfT = t * totalLength;
        int i1 = 0;

        while (lenOfT > lengths[i1] + 1)
            lenOfT -= lengths[i1++];

        int i0, i2, i3;
        int len = splinePoints.Count;

        i0 = i1 == 0 ? len - 1 : i1 - 1;
        i2 = (i1 + 1) % len;
        i3 = (i2 + 1) % len;

        t = lenOfT / lengths[i1];
        float tt = t * t, ttt = tt * t;

        float mul0 = -ttt + 2 * tt - t;
        float mul1 = 3 * ttt - 5 * tt + 2;
        float mul2 = -3 * ttt + 4 * tt + t;
        float mul3 = ttt - tt;

        Vector3 result = Vector3.zero;
        Vector3 p0 = splinePoints[i0].position,
            p1 = splinePoints[i1].position,
            p2 = splinePoints[i2].position,
            p3 = splinePoints[i3].position;

        result = mul0 * p0 + mul1 * p1 + mul2 * p2 + mul3 * p3;
        result /= 2;

        return result;
    }

    public Vector3 GetPrecalculatedPoint(float t) => calculatedPoints[(int) ((calculatedPoints.Count - 1) * t)];

    public void RecalculateSpline()
    {
        calculatedPoints.Clear();
        float deltaT = 1f / pointsSampleSize;
        for (float t = 0; t <= 1; t += deltaT)
            calculatedPoints.Add(GetPoint(t));
    }

    public void RecalculateEquidistantSpline()
    {
        calculatedPoints.Clear();
        RecalculateLengths();
        float deltaT = 1f / pointsSampleSize;
        for (float t = 0; t <= 1; t += deltaT)
            calculatedPoints.Add(GetEquidistantPoint(t));
    }

    public void RecalculateLengths()
    {
        lengths.Clear();
        totalLength = 0;
        for (int i = 0; i < splinePoints.Count; ++i)
        {
            lengths.Add(CalculateSegmentLength(i));
            totalLength += lengths[i];
        }
    }

    public Vector3 FindClosestPointOnSpline(Vector3 refPosition)
    {
        float closestT = 0, secondClosestT = 0;
        float closestDist = (refPosition - GetPrecalculatedPoint(0)).magnitude, secondClosestDist = closestDist;

        for (float t = firstProximityCheckStepLength; t <= 1; t += firstProximityCheckStepLength)
        {
            Vector3 nextPoint = GetPrecalculatedPoint(t);
            float distance = (refPosition - nextPoint).magnitude;
            if (distance < closestDist)
            {
                closestDist = distance;
                closestT = t;
            }
            else if (distance < secondClosestDist)
            {
                secondClosestDist = distance;
                secondClosestT = t;
            }
        }

        float minT = closestT < secondClosestT ? closestT : secondClosestT;
        float maxT = closestT > secondClosestT ? closestT : secondClosestT;
        float deltaT = 1f / calculatedPoints.Count;

        for (float t = minT; t <= maxT; t += deltaT)
        {
            Vector3 nextPoint = GetPrecalculatedPoint(t);
            float distance = (refPosition - nextPoint).magnitude;
            if (distance < closestDist)
            {
                closestDist = distance;
                closestT = t;
            }
        }

        return GetPrecalculatedPoint(closestT);
    }

    private float CalculateSegmentLength(int index) //calculate length for segment at given index
    {
        Vector3 oldPoint = GetPoint(LocalTToGlobalT(index, 0)), newPoint;
        float len = 0;
        for (float t = calculationStepLength; t < 1; t += calculationStepLength)
        {
            newPoint = GetPoint(LocalTToGlobalT(index, t));
            len += (newPoint - oldPoint).magnitude;
            oldPoint = newPoint;
        }

        newPoint = GetPoint(LocalTToGlobalT(index, 1));
        len += (newPoint - oldPoint).magnitude;

        return len;
    }

    private float LocalTToGlobalT(int index, float t) //calculate global t from t that is calculated for certain segment
    {
        int len = splinePoints.Count;
        t = Mathf.Clamp01(t);
        return (index + t) / len;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = debugLineColor;
        float deltaT = 1f / debugPointsSampleSize;
        Vector3 lastPoint = GetPoint(0);
        for (float t = deltaT; t <= 1; t += deltaT)
        {
            Vector3 currentPoint = GetPoint(t);
            Gizmos.DrawLine(lastPoint, currentPoint);
            lastPoint = currentPoint;
        }

        if (showCalculatedPath)
        {
            Gizmos.color = calculatedPathColor;
            lastPoint = GetPrecalculatedPoint(0);
            foreach (var point in calculatedPoints)
            {
                Gizmos.DrawLine(lastPoint, point);
                lastPoint = point;
            }
        }

        if (showPointSpheres)
        {
            Gizmos.color = debugPointsSpheresColor;
            foreach (var point in splinePoints)
                Gizmos.DrawSphere(point.position, debugPointSphereRadius);
        }

        if (showTraveller)
        {
            Gizmos.color = debugTravellerSphereColor;
            Gizmos.DrawSphere(GetEquidistantPoint(debugTravellerPosition), debugTravellerSphereRadius);
        }

        if (showProximityCheck)
        {
            var position = debugTransformForProximityCheck.position;
            Vector3 nearestPoint = FindClosestPointOnSpline(position);
            float distance = (nearestPoint - position).magnitude;

            Gizmos.color = debugProximityCheckSphereColor;
            Gizmos.DrawSphere(position, distance);

            Gizmos.color = debugProximityPointColor;
            Gizmos.DrawSphere(position, debugProximityPointWidth);
            Gizmos.DrawSphere(nearestPoint, debugProximityPointWidth);
        }
    }
}