using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Line : MonoBehaviour
{
    public LineRenderer line_renderer;

    List<Vector2> points;

    public void UpdateLine(Vector2 pos)
    {
        if (points == null)
        {
            points = new List<Vector2>();
            SetPoint(pos);
            return;
        }

        if (Vector2.Distance(points.Last(), pos) > .1f)
        {
            SetPoint(pos);
        }
    }

    public void LockLineMaterial()
    {
        Material _lockedMat = new Material(line_renderer.material);
        line_renderer.material = _lockedMat;
    }

    void SetPoint(Vector2 point)
    {
        points.Add(point);

        line_renderer.positionCount = points.Count;
        line_renderer.SetPosition(points.Count - 1, point);
    }


}
