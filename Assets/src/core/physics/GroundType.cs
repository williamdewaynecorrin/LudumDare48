using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundType : MonoBehaviour
{
    [SerializeField]
    private EGroundType type = EGroundType.Grass;

    public EGroundType Type()
    {
        return type;
    }
}

public enum EGroundType
{
    Grass,
    Wood,
    Block
}