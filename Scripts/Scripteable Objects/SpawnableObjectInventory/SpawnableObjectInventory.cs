using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "(InventoryType)", menuName = "Scripteable Objects/SpawnableObjectInventory")]
public class SpawnableObjectInventory : ScriptableObject
{
    public SpawnableObjects[] spawnableObjects;
}
