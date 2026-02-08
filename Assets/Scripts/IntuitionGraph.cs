using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class IntuitionGraph
{
    public List<string> nodes;
    public List<int> nodeMultiplers;
    public int[,] edges;
    public int[,] weights;

    public IntuitionGraph(List<string> nodes) 
    {
        this.nodes = nodes;
        this.nodeMultiplers = new List<int>();
        foreach (var node in nodes)
        {
            this.nodeMultiplers.Add(1);
        }
        this.edges = new int[nodes.Count, nodes.Count];
        this.weights = new int[nodes.Count, nodes.Count];
        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = 0; j < nodes.Count; j++)
            {
                this.edges[i, j] = 0;
                this.weights[i, j] = 0;
            }
        }
    }

    public void AddEdge(string a, string b)
    {
        int idx_a = nodes.IndexOf(a);
        int idx_b = nodes.IndexOf(b);
        AddEdge(idx_a, idx_b);
    }

    public void AddEdge(int a, int b)
    {
        this.edges[a,b] = 1;
    }


    public void AddEdgesToAllTargets(string source)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            AddEdge(nodes[nodes.IndexOf(source)], nodes[i]);
        }
    }

    public void AddWeight(string a, string b, int value)
    {
        int idx_a = nodes.IndexOf(a);
        int idx_b = nodes.IndexOf(b);
        AddWeight(idx_a, idx_b, value);
    }

    public void AddWeight(int a, int b, int value)
    {
        this.weights[a,b] = value;
    }

    public void SetMultiplier(string character, int mult)
    {
        SetMultiplier(this.nodes.IndexOf(character), mult);
    }

    public void SetMultiplier(int characterIdx, int mult)
    {
        this.nodeMultiplers[characterIdx] = mult;
    }

    public List<string> GetNeutralIntuition(string victim)
    {
        List<string> intuitionCulprits = new List<string>();
        int minValue = 10;
        int victimIdx = nodes.IndexOf(victim);
        foreach (var node in nodes.Where(x => x != victim).ToList())
        {
            int intuitionValue = 0;
            int suspectIdx = nodes.IndexOf(node);
            for (int i = 0; i < nodes.Count; i++)
            {
                if (i != suspectIdx && i != victimIdx)
                {
                    intuitionValue += weights[i, suspectIdx];
                }
            }
            if (intuitionValue < minValue)
            {
                intuitionCulprits.Clear();
                minValue = intuitionValue;
                intuitionCulprits.Add(node);
            }
            else if (intuitionValue == minValue)
            {
                intuitionCulprits.Add(node);
            }
        }
        Debug.Log($"Neutral Intuition Culprits: {string.Join(',', intuitionCulprits)} with value {minValue}");
        return intuitionCulprits;
    }

    public List<string> GetIntuition(string victim)
    {
        List<string> intuitionCulprits = new List<string>();
        int minValue = 10;
        int victimIdx = nodes.IndexOf(victim);
        foreach (var node in nodes.Where(x=>x!=victim).ToList())
        {
            int intuitionValue = 0;
            int suspectIdx = nodes.IndexOf(node);
            for (int i = 0; i < nodes.Count; i++)
            {
                if (i!= suspectIdx && i != victimIdx)
                {
                    intuitionValue += nodeMultiplers[i] * weights[i, suspectIdx] * edges[i,suspectIdx];
                }
            }
            if (intuitionValue < minValue)
            {
                intuitionCulprits.Clear();
                minValue = intuitionValue;
                intuitionCulprits.Add(node);
            }
            else if (intuitionValue == minValue)
            {
                intuitionCulprits.Add(node);
            }
        }
        Debug.Log($"Intuition Culprits: {string.Join(',', intuitionCulprits)} with value {minValue}");
        return intuitionCulprits;
    }

    internal void PrintGraph()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            string line = $"{nodes[i]}: ";
            for (int j = 0; j < nodes.Count; j++)
            {
                line += $"{nodes[j]} ({weights[i, j]}) |";
            }
            Debug.Log(line);
        }
    }
}
