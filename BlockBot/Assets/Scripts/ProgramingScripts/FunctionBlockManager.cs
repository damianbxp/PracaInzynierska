using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FunctionBlockManager : MonoBehaviour
{
    List<FunctionBlock> functionBlocks = new List<FunctionBlock>();
    public Transform FunctionsContent;
    public List<GameObject> BlockPrefabs;

    private void Start() {
        LoadFunctions();
        AddFcnBlocks();
    }

    void LoadFunctions() {
        FcnFolder basic = new FcnFolder("Basic", BlockPrefabs[0]);
        FcnFolder advanced = new FcnFolder("Advanced", BlockPrefabs[0]);
        FcnFolder loop = new FcnFolder("Loop", BlockPrefabs[0]);

        basic.functionBlocks.Add(new FcnMove("Move", BlockPrefabs[1]));
        basic.functionBlocks.Add(new FcnMove("Move Round", BlockPrefabs[1]));
        basic.functionBlocks.Add(new FcnMove("Move Linear", BlockPrefabs[1]));

        advanced.functionBlocks.Add(new FcnMove("Advanced Move", BlockPrefabs[1]));
        loop.functionBlocks.Add(new FcnMove("Loop Move", BlockPrefabs[1]));

        functionBlocks.Add(basic);
        functionBlocks.Add(advanced);
        functionBlocks.Add(loop);

    }

    void AddFcnBlocks() {
        foreach(FcnFolder folder in functionBlocks) {
            AddFcnBlockToUI(folder, FunctionsContent);
        }
    }

    void AddFcnBlockToUI(FunctionBlock block, Transform parent) {
        Transform spawnedBlock = Instantiate(block.BlockPrefab, parent).transform;
        spawnedBlock.name = block.Name;
        spawnedBlock.GetChild(0).GetComponent<Text>().text = block.Name;
        Debug.Log(block.GetType());
        if(block.GetType() == typeof(FcnFolder)) {
            FcnFolder folder = block as FcnFolder;
            foreach(FunctionBlock insideBlock in folder.functionBlocks) {
                AddFcnBlockToUI(insideBlock, spawnedBlock);
            }
        }
    }
    public void expand() {

    }
}
