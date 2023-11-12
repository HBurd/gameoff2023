using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    None,
    Honey,
    Pollen,
    RoyalJelly,
}

public class Resevoir : MonoBehaviour
{
    public ResourceType resource_type = ResourceType.None;
    public float resource_amount = 0.0f;
}
