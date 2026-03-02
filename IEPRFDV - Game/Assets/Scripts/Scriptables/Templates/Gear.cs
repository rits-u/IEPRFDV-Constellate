using UnityEditor.Rendering;
using UnityEngine;

[CreateAssetMenu(fileName = "Gear", menuName = "Constellate/Gear")]
public class Gear : MonoBehaviour
{
    [Header("Stats")]
    public int HP;
    public int ATK;
    public int SP;

    [Header("Info")]
    public int tier;
    public string effect;
}
