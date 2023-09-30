using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSegment
{
    //Branch Variables
    public int branchIndex;
    
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector3> lerpStartPos = new List<Vector3>();
    public List<Vector3> lerpEndPos = new List<Vector3>();
    public Vector3 centerVector = Vector3.zero;
    public float sidesAngle;
    public float radius;
    public float curBend = 0;
    public float curRotation = 0;
    public float curTapering = 1;

    //L System 
    public LSystemBranch branch = LSystemBranch.A;
    public int indexA = 0;
    public int indexB = 0;
    public int indexC = 0;
    public int indexD = 0;
    public char curRule;
}
