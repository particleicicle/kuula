using UnityEngine;
using System.Collections.Generic;
using System.Text;

[CreateAssetMenu(fileName = "LevelSetData", menuName = "Game/Level Set Data")]
public class LevelSetData : ScriptableObject
{
    [System.Serializable]
    public class LevelSet
    {
        public string name {
            get{
                if(_name.Length < 1)
                    _name = new StringBuilder("Set ").Append(setNumber).ToString();

                return _name;
            }
            set{
                _name = value;
            }
        }
        [SerializeField]
        private string _name;
        public int setNumber;
        public List<string> levelSceneNames = new();
    }

    public List<LevelSet> levelSets = new();
}
