using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scripteable Objects/AdventurerSample")]
public class AdventurerData : ScriptableObject
{
    [SerializeField] int maxHealth;
    [SerializeField] int maxAmmo;
    [SerializeField] int speed;
}
