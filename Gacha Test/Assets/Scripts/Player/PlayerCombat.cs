using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] EquipmentManager equipment;
    [SerializeField] Transform avatarHolder;
    [SerializeField] CharacterAvatar[] characterAvatars;
    [SerializeField] Character[] characterInfo;

    public Character[] currentParty;
    [SerializeField] CharacterAvatar activeCharacterAvatar;
    public Character activeCharacterInfo;

    public ParticleSystem hitFX;
    public GameObject deathFX;

    public bool isDead;

    public int level;
    public int currentExperience;
    public float currentHealth;
    public float maxHealth;
    public int def;
    public int atk;
    public int skill;
    public float critRate;
    public float critDmg;
    public float recharge;

    void SetupAttributes()
    {
        level = activeCharacterInfo.level;
        currentExperience = activeCharacterInfo.currentExperience;
        currentHealth = activeCharacterInfo.currenthP;
        maxHealth = activeCharacterInfo.maxHP;
        def = activeCharacterInfo.def;
        atk = activeCharacterInfo.atk;
        skill = activeCharacterInfo.skill;
        critRate = activeCharacterInfo.critRate;
        critDmg = activeCharacterInfo.critDmg;
        recharge = activeCharacterInfo.recharge;
    }

    void WriteBackToCharacters()
    {
        activeCharacterInfo.level = Mathf.FloorToInt(level);
        activeCharacterInfo.currentExperience = Mathf.FloorToInt(currentExperience);
        activeCharacterInfo.currenthP = Mathf.FloorToInt(currentHealth);
    }

    public void FullPartyMaxHeal()
    {
        foreach (var character in currentParty)
        {
            character.currenthP = character.maxHP;
            SetupAttributes();
        }
    }

    private void Start()
    {
        characterAvatars = avatarHolder.GetComponentsInChildren<CharacterAvatar>();

        ChangeActiveCharacter(4);
    }

    void Update()
    {
        UIManager.instance.UpdateHealthBar(currentHealth, maxHealth);
    }

    public void ChangeActiveCharacter(int i)
    {
        if (activeCharacterInfo != null)
            WriteBackToCharacters();

        foreach (var avatar in characterAvatars)
        {
            avatar.gameObject.SetActive(false);
        }

        activeCharacterAvatar = characterAvatars[i];
        activeCharacterInfo = characterInfo[i];

        activeCharacterAvatar.gameObject.SetActive(true);

        SetupCharacterAvatar();
        SetupAttributes();

        UIManager.instance.UpdateCurrentCharacterLevelDisplay(level); 
    }

    void SetupCharacterAvatar()
    {
        if (activeCharacterAvatar.currentShield)
        {
            equipment.currentShield = activeCharacterAvatar.currentShield;
            equipment.activeEquipment[1] = activeCharacterAvatar.activeEquipment[1];
            equipment.idleEquipment[1] = activeCharacterAvatar.idleEquipment[1];
        }

        equipment.currentWeapon = activeCharacterAvatar.currentWeapon;
        equipment.activeEquipment[0] = activeCharacterAvatar.activeEquipment[0];
        equipment.idleEquipment[0] = activeCharacterAvatar.idleEquipment[0];

        if (activeCharacterAvatar.baseAtkCols.Length > 0)
            player.baseAtkCols = activeCharacterAvatar.baseAtkCols;

        player.atkFX = activeCharacterAvatar.atkFX;
        player.baseAtkCD = activeCharacterInfo.baseAtkCD;

        if (activeCharacterAvatar.baseAtkProjectileBarrel != null)
        {
            player.baseAtkProjectileBarrel = activeCharacterAvatar.baseAtkProjectileBarrel;
            player.baseAtkProjectile = activeCharacterInfo.baseAtkProjectile;
        }

        player.anim.avatar = activeCharacterInfo.animatorAvatar;
        player.anim.SetFloat("WeaponType", activeCharacterInfo.weaponType);

        if (player.isIdle)
            equipment.Sheath();
        else
            equipment.Unsheath();
    }

    void Death()
    {
        if (!isDead)
        {
            isDead = true;

            //player.anim.SetTrigger("Death");
        }
    }

    void ReceiveDamage(float amount)
    {
        float result = currentHealth - (amount * (100 / (100 + def)));

        if (result <= 0)
        {
            currentHealth = 0;
            Death();
        }
        else
        {
            currentHealth -= amount;
        }

        player.anim.SetTrigger("Hurt");
        hitFX.Play();
    }

    public float BaseAtkDamage(string source)
    {
        return Random.Range(0.9f, 1.1f) * (atk * activeCharacterInfo.baseAtkMultipliers[int.Parse(source)]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDead)
        {
            if (other.tag == "Damage")
            {
                if (other.GetComponentInParent<EnemyCombat>() && other.GetComponentInParent<EnemyCombat>() != this)
                {
                    if (other.GetComponentInParent<EnemyCombat>())
                        ReceiveDamage(other.GetComponentInParent<EnemyCombat>().BaseAtkDamage(other.name));
                }

                if (other.GetComponentInParent<Projectile>() && other.GetComponentInParent<Projectile>().player != this)
                {
                    ReceiveDamage(other.GetComponentInParent<Projectile>().DealDamage());
                    other.GetComponentInParent<Projectile>().Destruct();
                }
            }
        }
    }
}
