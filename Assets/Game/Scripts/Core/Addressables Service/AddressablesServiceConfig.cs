using UnityEngine;

[CreateAssetMenu(fileName = "Addressables Service Config", menuName = "Game/Configs/Addressables Service/Addressables Service Config", order = 0)]
public class AddressablesServiceConfig : ScriptableObject
{
    [Header("Debug")]
    [SerializeField] private bool _enableLogger;

    public bool EnableLogger => _enableLogger;
}