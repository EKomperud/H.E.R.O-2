using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public struct NStateInfo
{
    public NPlayerController p;
    public Rigidbody2D r;
    public BoxCollider2D b;
    public Player j;
    public Animator ac;
    public SpriteRenderer sr;
    public NStateSO bd;

    public NStateInfo(NPlayerController p, Rigidbody2D r, BoxCollider2D b, Player j, Animator ac, SpriteRenderer sr, NStateSO bd)
    {
        this.p = p;
        this.r = r;
        this.b = b;
        this.j = j;
        this.ac = ac;
        this.sr = sr;
        this.bd = bd;
    }
}

public class NPlayerController : MonoBehaviour {

    #region Members
    /// Private Members
    
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D bc;
    [SerializeField] private Animator ac;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private NStateSO balanceData;
    [SerializeField] private int playerNumber;

    [Tooltip("air, fire, water, earth, plasma")]
    [SerializeField] private int[] weaponCounts;
    private NLevelManager levelManager;
    private bool living;
    private Player joystick;
    private NState[] movementStates;
    private NState movementState;
    private SortedList<EElement, Queue<NWeapon>> weapons;
    private bool avatar;
    private EElement weaponEquipped;
    private NWeapon weapon;
    private NTotem totem;
    private float totemDist;

    private bool grabButton;
    private bool dischargeButton;
    private bool swapButton;
    #endregion

    #region Monobehaviour
    void Start() {
        joystick = ReInput.players.GetPlayer(playerNumber);
        NStateInfo info = new NStateInfo(this, rb, bc, joystick, ac, sr, balanceData);
        movementStates = new NState[7];
        movementStates[0] = new NStateNormal(info, EState.normal);
        movementStates[1] = new NStateJump1(info, EState.jump1);
        movementStates[2] = new NStateJump2(info, EState.jump2);
        movementStates[3] = new NStateAshes(info, EState.ashes);
        movementStates[4] = new NStateAirborne(info, EState.airborne);
        movementStates[5] = new NStateFrozen(info, EState.frozen);
        movementStates[6] = new NStateSucc(info, EState.succ, transform.position);
        movementState = movementStates[1];

        weapons = new SortedList<EElement, Queue<NWeapon>>(6);
        weapons[EElement.air] = new Queue<NWeapon>(weaponCounts[0]);
        weapons[EElement.fire] = new Queue<NWeapon>(weaponCounts[1]);
        weapons[EElement.water] = new Queue<NWeapon>(weaponCounts[2]);
        weapons[EElement.earth] = new Queue<NWeapon>(weaponCounts[3]);
        weapons[EElement.plasma] = new Queue<NWeapon>(weaponCounts[4]);
        weapons[EElement.none] = new Queue<NWeapon>(1);
        weaponEquipped = EElement.air;
        weapon = null;
        totem = null;
        totemDist = Mathf.Infinity;
        living = true;
        avatar = false;
    }

    void Update()
    {

        if (movementState != null)
        {
            movementState.StateUpdate();
        }

        grabButton = joystick.GetButtonDown("Grab Weapon");
        if (grabButton && totem != null && totem.GetCooledDown())
        {
            if (weapons[totem.GetElement()].Count < weaponCounts[(int)totem.GetElement()])
            {
                if (avatar)
                    SpawnWeapons();
                else if (!avatar && (!(weapon != null) || totem.GetElement() == weaponEquipped))
                    SpawnWeapons();
            }   
        }

        dischargeButton = joystick.GetButtonDown("Discharge Weapon");
        if (dischargeButton && weapon != null)
        {
            weapon.Discharge(new Vector2(joystick.GetAxis("Aim Horizontal"), joystick.GetAxis("Aim Vertical")).normalized);
            weapons[weaponEquipped].Dequeue();
            weapon = null;
            if (weapons[weaponEquipped].Count != 0)
            {
                SetWeaponAngles(weaponEquipped, Mathf.Infinity, Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
                weapon = weapons[weaponEquipped].Peek();
            }
            else
            {
                if (avatar && CycleWeaponEquipped())
                {
                    weapon = weapons[weaponEquipped].Peek();
                    SetAllWeaponAngles();
                }
                else
                {
                    weapon = null;
                }
            }
        }

        swapButton = joystick.GetButtonDown("Swap Weapon");
        if (avatar && swapButton)
        {
            EElement oldWeapon = weaponEquipped;
            if (CycleWeaponEquipped())
            {
                weapon = weapons[weaponEquipped].Peek();
            }
        }
    }

    void FixedUpdate()
    {
        if (movementState != null)
        {
            movementState.StateFixedUpdate();
        }
    }
    #endregion

    #region Public Methods
    public void TryStateTransition(EState state)
    {
        if (movementState.AskValidTransition(state))
        {
            movementState.ExitState();
            movementState = movementStates[(int)state];
            movementState.EnterState();
        }
    }

    public void TryAddTotem(NTotem totem, float dist)
    {
        if (totem.Equals(this.totem))
        {
            totemDist = dist;
        }
        else if (dist < totemDist)
        {
            this.totem = totem;
            totemDist = dist;
        }
    }

    public void RemoveTotem()
    {
        totem = null;
        totemDist = Mathf.Infinity;
    }

    public NWeapon GetWeapon()
    {
        return weapon;
    }

    public void HitByFire()
    {
        if (movementState.AskValidTransition(EState.ashes))
        {
            StartCoroutine("DeathByFire");
        }
    }

    public void HitByWater()
    {
        TryStateTransition(EState.frozen);
    }

    public void HitByPlasma(Vector3 blackHole)
    {
        if (living)
        {
            living = false;
            rb.simulated = false;
            NStateInfo info = new NStateInfo(this, rb, bc, joystick, ac, sr, balanceData);
            movementStates[6] = new NStateSucc(info, EState.succ, blackHole);
            TryStateTransition(EState.succ);
            levelManager.RemovePlayer(gameObject);
        }
    }

    public void AddLevelManager(NLevelManager levelManager)
    {
        this.levelManager = levelManager;
    }

    public int GetPlayerNumber()
    {
        return playerNumber;
    }

    public bool GetLivingStatus()
    {
        return living;
    }
    #endregion

    #region Private Helpers
    private void SpawnWeapons()
    {
        totem.StartCooldown();
        int weaponsCount = totem.GetWeaponCount();
        EElement element = totem.GetElement();
        int originalWeaponCount = weapons[element].Count;

        for (int i = 0; i < weaponsCount && weapons[element].Count < weaponCounts[(int)element]; i++)
        {
            Transform weaponTransform = Instantiate(totem.GetWeapon());
            NWeapon w = weaponTransform.GetComponent<NWeapon>();
            w.SetWielder(this, joystick);
            weaponTransform.SetParent(transform);
            weapons[element].Enqueue(w);
        }

        weaponEquipped = element;
        SetAllWeaponAngles();
        weapon = weapons[element].Peek();
    }

    private bool CycleWeaponEquipped()
    {
        int start = (int)weaponEquipped;
        int iterator = start + 1;
        if (iterator == 6)
            iterator = 0;
        while (iterator != start)
        {
            if (weapons[(EElement)iterator].Count != 0)
            {
                weaponEquipped = (EElement)iterator;
                return true;
            }
            iterator++;
            if (iterator == 6)
                iterator = 0;
        }
        weaponEquipped = EElement.none;
        return false;
    }

    private void SetWeaponAngles(EElement e, float xRotation, float yRotation, float radius, float height)
    {
        float angleIncrement = (2f * Mathf.PI) / weapons[e].Count;
        int j = 0;
        foreach (NWeapon w in weapons[e])
        {
            w.SetParameters(angleIncrement * j, xRotation, yRotation, radius, height);
            j++;
        }
    }

    private void SetAllWeaponAngles()
    {
        int elementCount = 0;
        foreach (KeyValuePair<EElement,Queue<NWeapon>> e in weapons)
        {
            if (e.Value.Count != 0)
                elementCount++;

            if (elementCount == 1)
                SetWeaponAngles(e.Key, 1f, 0.25f, 0.8f, 0f);
            else if (elementCount == 2)
                SetWeaponAngles(e.Key, 1f, 0.65f, 0.8f, 0f);
            else if (elementCount == 3)
                SetWeaponAngles(e.Key, 1f, -0.65f, 0.8f, 0f);
            else if (elementCount == 4)
                SetWeaponAngles(e.Key, 0f, 1f, 0.8f, 0f);
            else
                SetWeaponAngles(e.Key, 1f, 0.25f, 0.4f, 1f);
        }

    }

    private IEnumerator DeathByFire()
    {
        float timer = 3f;
        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        TryStateTransition(EState.ashes);
        living = false;
        levelManager.RemovePlayer(gameObject);
    }

    #endregion
}
