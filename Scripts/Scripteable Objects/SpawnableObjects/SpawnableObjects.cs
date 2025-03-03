using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "(Object)", menuName = "Scripteable Objects/SpawnableObjects")]

public class SpawnableObjects : ScriptableObject
{
    [SerializeField]
    public GameObject prefab; //The Object's Prefab

    [SerializeField]
    public string objectName; //Name Of the Object

    [SerializeField]
    [TextArea]
    public string objectDescription;

    [SerializeField]
    public Sprite objectSprite;

    public enum ObjectType{ NPC, Prop, Item, Setting }
    [SerializeField]
    public ObjectType objectType;

    [SerializeField]
    public GameObject spawnVFX;

}
