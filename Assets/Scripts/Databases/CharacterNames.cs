using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterNames", menuName = "Databases/CharacterNames")]
public class CharacterNames : ScriptableObject
{
    [SerializeField] private string _hongoNames;
    [SerializeField] private string _sapoNames;

    public List<string> GetRandomNames(bool getHongoNames, int count)
    {
        List<string> namesList;
        List<string> result = new List<string>();

        if (getHongoNames) namesList = new List<string>(_hongoNames.Split(';'));
        else namesList = new List<string>(_sapoNames.Split(';'));        

        for (int i = 0; count > result.Count; i++)
        {
            string randomName = namesList.ElementAt(Random.Range(0, namesList.Count));
            result.Add(randomName);
            namesList.Remove(randomName);
        }

        return namesList;
    }
}
