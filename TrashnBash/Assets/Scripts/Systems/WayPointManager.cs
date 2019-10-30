using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WayPointManager : MonoBehaviour
{
    [Serializable]
    public class Path
    {
        public int Id;
        public List<Transform> WayPoints;
    }
    public List<Path> Paths;
    public Path GetPath(int id)
    {
        return Paths[id];
    }
}
