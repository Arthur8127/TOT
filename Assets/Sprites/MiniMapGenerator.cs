using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapGenerator : MonoBehaviour
{
    public LineRenderer line;

    public void SetPoint()
    {
        line.positionCount++;
        line.SetPosition(line.positionCount-1, transform.position);
    }
    public void RemovePoint()
    {
        line.positionCount--;
    }
}
