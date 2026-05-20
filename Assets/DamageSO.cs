using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnDamageTable", order = 2)]
public class DamageSO : ScriptableObject
{

    [SerializeField]
    public int woodenSword;
    [SerializeField]
    public int whiteSword;
    [SerializeField]
    public int magicSword;
    [SerializeField]
    public int blueBoomerang;
    [SerializeField]
    public int redBoomerang;
    [SerializeField]
    public int woodenArrows;
    [SerializeField]
    public int silverArrows;
    [SerializeField]
    public int fireAttacks;
    [SerializeField]
    public int bombDamage;
    [SerializeField]
    public int magicRod;

}


