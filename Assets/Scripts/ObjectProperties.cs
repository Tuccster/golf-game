using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectProperties : MonoBehaviour 
{
    public enum ObjectType { GREEN, HOLE, SPAWN, DECORATION};
    public ObjectType _objectType = ObjectType.GREEN;

    [HideInInspector] public Transform _transform;
    public byte _holeID;
}