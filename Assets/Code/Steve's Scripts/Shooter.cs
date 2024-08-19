using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;         // The point from where the bullet will be fired
    public GameObject bulletPrefab;     // The bullet prefab to be instantiated
    public float bulletForce = 20f;     // The speed at which the bullet will travel

    public float autoFireRate = 0.1f;   // Fire rate for fully automatic weapon
    public float semiFireRate = 0.5f;   // Cooldown time for semi-automatic weapon
    private float nextTimeToFire = 0f;

    public enum WeaponType { SemiAutomatic, FullyAutomatic }
    public WeaponType currentWeapon = WeaponType.SemiAutomatic;

    public Image weaponIndicator;       // UI Image to show the weapon type
    public Sprite semiAutomaticSprite;  // Sprite for semi-automatic weapon
    public Sprite fullyAutomaticSprite; // Sprite for fully automatic weapon

    void Start()
    {
        UpdateWeaponIndicator();
    }

    void Update()
    {
        HandleShooting();
        HandleWeaponSwitching();
    }

    void HandleShooting()
    {
        if (currentWeapon == WeaponType.FullyAutomatic)
        {
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / autoFireRate;
                Shoot();
            }
        }
        else if (currentWeapon == WeaponType.SemiAutomatic)
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + semiFireRate;
                Shoot();
            }
        }
    }

    void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeapon = WeaponType.SemiAutomatic;
            UpdateWeaponIndicator();
            Debug.Log("Switched to Semi-Automatic");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeapon = WeaponType.FullyAutomatic;
            UpdateWeaponIndicator();
            Debug.Log("Switched to Fully Automatic");
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.right * bulletForce, ForceMode2D.Impulse);
    }

    void UpdateWeaponIndicator()
    {
        if (currentWeapon == WeaponType.SemiAutomatic)
        {
            weaponIndicator.sprite = semiAutomaticSprite;
        }
        else if (currentWeapon == WeaponType.FullyAutomatic)
        {
            weaponIndicator.sprite = fullyAutomaticSprite;
        }
    }
}
