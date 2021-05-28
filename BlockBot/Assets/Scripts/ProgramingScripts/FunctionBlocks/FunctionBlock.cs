using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FunctionBlock
{
    string name;
    GameObject blockPrefab;

    public string Name {
        get { return name; }
        set { name = value; }
    }

    public GameObject BlockPrefab {
        get { return blockPrefab; }
        set { blockPrefab = value; }
    }

    public FunctionBlock(string _name, GameObject prefab) {
        Name = _name;
        BlockPrefab = prefab;
    }

}
