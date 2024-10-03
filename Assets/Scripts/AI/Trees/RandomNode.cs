using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomNode : ITreeNode
{
    private Dictionary<ITreeNode, float> _dic;
    private List<float> _ogDicValues = new();

    public Dictionary<ITreeNode, float> DicValues => _dic;

    public RandomNode(Dictionary<ITreeNode, float> dic)
    {
        _dic = dic;
        foreach (var item in dic) _ogDicValues.Add(item.Value);
    }

    public void Execute()
    {
        var randomNode = MyRandoms.Roulette(_dic);
        randomNode.Execute();
    }

    public float GetWeight(ITreeNode node)
    {
        return _dic[node];
    }

    public void UpdateWeight(ITreeNode node, float newWeight)
    {
        if (!_dic.ContainsKey(node))
        {
            Debug.LogWarning($"{this}: {node} was not found in values.");
            return;
        }

        _dic[node] = newWeight;
    }

    public void ResetWeights()
    {
        int index = 0;
        Dictionary<ITreeNode, float> resetValuesDict = new();

        foreach (var item in _dic)
        {
            resetValuesDict.Add(item.Key, _ogDicValues[index]);
            index++;
        }

        _dic = resetValuesDict;
    }
}
