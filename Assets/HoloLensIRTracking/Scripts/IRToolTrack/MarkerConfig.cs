using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MarkerConfig
{
    public int count;
    public int id;
    public float scale;
    public Fiducial[] fiducials;
    public Pivot pivot;
}

[Serializable]
public class Fiducial
{
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class Pivot
{
    public float x;
    public float y;
    public float z;
    public float rx;
    public float ry;
    public float rz;
}
