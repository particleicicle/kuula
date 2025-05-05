using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelSetData", menuName = "Game/Level Set Data")]
public class LevelSetData : ScriptableObject
{
    [System.Serializable]
    public class LevelSet
    {
        public int setNumber;
        public List<string> levelSceneNames = new();
    }

    public List<LevelSet> levelSets = new();
}
