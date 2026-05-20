using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTableManager : MonoBehaviour
{
    [SerializeField]
    public DamageSO damageTable;
    public static DamageTableManager _instance;
    

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

    }
}
public enum WEAPON_TYPE
{
    TYPE_SWORD,
    TYPE_BOOMERANG,
    TYPE_ARROW,
    TYPE_ARROW_SILVER,
    TYPE_FIRE,
    TYPE_BOMB,
    TYPE_MAGICROD
}

public struct HitPacket
{
    public WEAPON_TYPE type;
    public MOVEDIRECTION knockbackDirection;
    public int damageAmount;
}