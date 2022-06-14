using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarPathFinding : MonoBehaviour
{
    /*public static List<NodeBase> FindPath(NodeBase startNode, NodeBase targetNode)
    {
        var toSearch = new List<NodeBase>() { startNode };
        var processed = new List<NodeBase>();

        while (toSearch.Any())
        {
            var currnet = toSearch[0];
            foreach (var t in toSearch)
                if (t.F < currnet.F || t.F == currnet.F && t.H < currnet.H)
                    currnet = t;

            processed.Add(currnet);
            toSearch.Remove(currnet);

            foreach (var neighbor in currnet.Neighbors.Where(t => t.Walkable && !processed.Contains(t)))
            {
                var inSearch = toSearch.Contains(neighbor);
                var costToNeighbot = currnet.G + currnet.GetDistance(neighbor);

                if(!inSearch || costToNeighbot < neighbor.G)
                {
                    neighbor.SetG(costToNeighbot);
                    neighbor.SetConnection(currnet);

                    if (!inSearch)
                    {
                        neighbor.SetH(neighbor.GetDistance(targetNode));
                        toSearch.Add(neighbor);
                    }
                }
            }
        }
    }*/



    public class NodeBase
    {
        public NodeBase Connection { get; private set; }
        public float G { get; private set; }
        public float H { get; private set; }
        public float F => G + H;

        public void SetConnection(NodeBase nodeBase) => Connection = nodeBase;
        public void SetG(float g) => G = g;
        public void SetH(float h) => H = h;
    }
}


