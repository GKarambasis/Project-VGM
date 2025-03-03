using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    public SpawnableObjects objectInformation;

    private void OnEnable()
    {
        if(objectInformation.spawnVFX != null)
        {
            Instantiate(objectInformation.spawnVFX, gameObject.transform);
        }
    }

}
