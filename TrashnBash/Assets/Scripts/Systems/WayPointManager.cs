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
        public Transform WayPointsHolder;
        [HideInInspector]
        public List<Transform> WayPoints;
        public Color pathColor;

        public void SetupWaypoints()
        {
            WayPoints = new List<Transform>();
            foreach (Transform child in WayPointsHolder)
                WayPoints.Add(child);
        }
    }
    public List<Path> Paths;

    public Path GetPath(int id)
    {
        if (pathIdDictionary.ContainsKey(id))
            return pathIdDictionary[id];
        return null;
    }

    Dictionary<int, Path> pathIdDictionary = new Dictionary<int, Path>();

    private void Awake()
    {
        foreach (Path path in Paths)
            pathIdDictionary.Add(path.Id, path);
    }

    private void OnDrawGizmos()
    {
        foreach(Path path in Paths)
        {
            if (path.WayPointsHolder == null)
                continue;

            Gizmos.color = path.pathColor;
            int i = 0;
            foreach(Transform child in path.WayPointsHolder)
            {
                if (i < path.WayPointsHolder.childCount - 1)
                {
                    Gizmos.DrawLine(child.transform.position, path.WayPointsHolder.GetChild(i + 1).transform.position);
                    Gizmos.DrawWireSphere(child.transform.position, 0.25f);
                }

                i++;
            }
        }
    }
}
