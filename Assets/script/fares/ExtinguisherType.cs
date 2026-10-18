using UnityEngine;

public enum ExtinguisherType
{
    Water,
    CO2,
    DryChemical,
    Foam
}

public class Extinguisher : MonoBehaviour
{
    public ExtinguisherType extinguisherType;
}