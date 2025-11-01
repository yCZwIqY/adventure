using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    [Tooltip("모든 무기 리스트")] public List<WeaponData> allWeapons;

    private List<WeaponData> _ownedWeapons = new List<WeaponData>();
    public List<WeaponData> ownedWeapons => _ownedWeapons;

    private WeaponData _equippedWeapon;
    public WeaponData equippedWeapon => _equippedWeapon;

    private PlayerController playerController;
    private PlayerWallet playerWallet;

    private float power;
    private float knockbackPower;

    public GameObject parringEffect;
    public GameObject attackEffect;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        playerWallet = FindAnyObjectByType<PlayerWallet>();

        if (playerController != null)
        {
            power = playerController.playerCombat.power;
            knockbackPower = playerController.playerCombat.knockbackPower;
        }

        if (allWeapons.Any())
        {
            _equippedWeapon = allWeapons[0];
        }
    }

    public bool TryPurchaseWeapon(WeaponData weapon)
    {
        if (weapon == null || _ownedWeapons.Contains(weapon))
        {
            return false;
        }

        if (playerWallet.TrySpend(weapon.price))
        {
            AcquireWeapon(weapon);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void EquipWeapon(WeaponData weapon)
    {
        if (weapon == null || !_ownedWeapons.Contains(weapon))
        {
            return;
        }

        UnEquipWeapon();
        _equippedWeapon = weapon;

        if (playerController != null && _equippedWeapon != null)
        {
            playerController.playerCombat.power = _equippedWeapon.power;
            playerController.playerCombat.knockbackPower = _equippedWeapon.knockbackPower;
            playerController.playerCombat.attackEffect = _equippedWeapon.attackEffect;
        }
    }

    public void UnEquipWeapon()
    {
        if (_equippedWeapon != null && playerController != null)
        {
            playerController.playerCombat.power = power;
            playerController.playerCombat.knockbackPower = knockbackPower;
            playerController.playerCombat.attackEffect = attackEffect;
        }

        _equippedWeapon = allWeapons[0];
    }

    public void AcquireWeapon(WeaponData weaponData)
    {
        if (!_ownedWeapons.Contains(weaponData))
        {
            _ownedWeapons.Add(weaponData);
        }
    }
}