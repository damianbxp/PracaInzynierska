using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FcnFolder : FunctionBlock
{
    public List<FunctionBlock> functionBlocks = new List<FunctionBlock>();

    public FcnFolder(string _name, GameObject prefab) : base(_name, prefab) {
        
    }

}
