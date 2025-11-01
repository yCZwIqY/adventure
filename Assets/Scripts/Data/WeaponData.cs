using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
public class WeaponData : ScriptableObject
{
    public string id;
    public string name;
    public string description;
    public int price;

    public float power;
    public float knockbackPower;

    public GameObject weapon;
    public GameObject attackEffect;
    public GameObject parringEffect;
    public AudioClip attackSound;
    public AudioClip parringSound;
}