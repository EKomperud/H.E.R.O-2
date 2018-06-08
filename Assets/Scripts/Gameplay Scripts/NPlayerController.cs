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
    
    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private ParticleSystem fireParticles;
    private ParticleSystem bloodParticles;
    [SerializeField] private NStateSO balanceData;
    [SerializeField] private int playerNumber;

    [Tooltip("air, fire, water, earth, plasma")]
    [SerializeField] private int[] weaponCounts;
    [SerializeField] private Transform airPrefab;
    private NLevelManager levelManager;
    private bool living;
    private Player joystick;
    private SortedList<EElement, Queue<NWeapon>> weapons;
    private bool avatar;
    private Coroutine frozen;
    private Coroutine onFire;
    private EElement weaponEquipped;
    private NWeapon weapon;
    private NTotem totem;
    private float totemDist;

    private NState[] movementStates;
    private NState movementState;
    private Dictionary<string, bool> movementBools;
    public EState state;

    private bool grabButton;
    private bool dischargeButton;
    private bool swapButton;
    #endregion

    #region Monobehaviour
    void Start() {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        fireParticles = transform.GetChild(0).GetComponent<ParticleSystem>();
        bloodParticles = transform.GetChild(1).GetComponent<ParticleSystem>();

        joystick = ReInput.players.GetPlayer(playerNumber);
        movementBools = new Dictionary<string, bool>();
        movementBools["pushed"] = false;
        movementBools["succed"] = false;
        movementBools["bounced"] = false;
        movementBools["frozen"] = false;
        movementBools["ashed"] = false;
        movementBools["spiked"] = false;
        movementBools["slipped"] = false;
        movementBools["doubled"] = false;
        NStateInfo info = new NStateInfo(this, rigidBody, boxCollider, joystick, animator, spriteRenderer, balanceData);
        movementStates = new NState[12];
        movementStates[0] = new NStateNormal(info, EState.normal);
        movementStates[1] = new NStateJump1(info, EState.jump1);
        movementStates[2] = new NStateJump2(info, EState.jump2);
        movementStates[3] = new NStateAshes(info, EState.ashes);
        movementStates[4] = new NStateAirborne(info, EState.airborne);
        movementStates[5] = new NStateSucc(info, EState.succ, transform.position);
        movementStates[6] = new NStatePushed(info, EState.pushed, Vector2.zero);
        movementStates[7] = new NStateBounced(info, EState.bounced, info.bd.bouncedInitialVelocity, info.bd.bouncedTransitionLockoutFrames);
        movementStates[8] = new NStateSpiked(info, EState.spiked, Vector2.zero);
        movementStates[9] = new NStateSlipped(info, EState.slipped);
        movementStates[10] = new NStateSlam(info, EState.slam);
        movementStates[11] = new NStateBounced(info, EState.boinked, info.bd.boinkedInitialVelocity, info.bd.boinkedTransitionLockoutFrames);
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
                else if (!avatar && (!(weapon != null) || totem.GetElement() == weaponEquipped || weaponEquipped == EElement.air))
                    SpawnWeapons();
            }   
        }

        dischargeButton = joystick.GetButtonDown("Discharge Weapon");
        if (dischargeButton && weapon != null)
        {
            Vector2 angle = new Vector2(joystick.GetAxis("Aim Horizontal"), joystick.GetAxis("Aim Vertical")).normalized;
            if (angle == Vector2.zero)
            {
                angle = spriteRenderer.flipX ? new Vector2(-1f, 0f) : new Vector2(1f, 0f);
                weapon.SetSpriteDirection(spriteRenderer.flipX);
            }
            weapon.Discharge(angle, boxCollider);
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
                    if (weaponEquipped != EElement.air)
                    {
                        weaponEquipped = EElement.air;
                        TrySpawnAir();
                    }
                    else
                        weaponEquipped = EElement.air;
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

    void OnCollisionEnter2D(Collision2D col)
    {
        NPlayerController otherPlayer = col.gameObject.GetComponent<NPlayerController>();
        if (otherPlayer != null && onFire != null)
            otherPlayer.HitByFire();
    }
    #endregion

    #region Public Methods

    public bool StateTransition(EState state)
    {
        movementState.ExitState();
        movementState = movementStates[(int)state];
        movementState.EnterState();
        this.state = state;
        return true;
    }

    public void SetMovementBool(string s, bool b)
    {
        movementBools[s] = b;
    }

    public bool GetMovementBool(string s)
    {
        bool b;
        if (movementBools.TryGetValue(s, out b))
            return b;
        else
            throw new KeyNotFoundException();
    }

    public void TryAddTotem(NTotem totem, float dist)
    {
        if (totem.Equals(this.totem))
        {
            totemDist = dist;
        }
        else if (dist < totemDist || !this.totem.GetCooledDown())
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

    public Collider2D GetCollider()
    {
        return boxCollider;
    }

    public void Bounce()
    {
        movementState.SetBool("bounced", true);
    }

    public void HitByAir(Vector2 direction)
    {
        NStatePushed pushed = (NStatePushed)movementStates[6];
        pushed.SetPushedDirection(direction);
        movementState.SetBool("pushed", true);
    }

    public void HitByEarth(Vector2 direction)
    {
        living = false;
        rigidBody.constraints = RigidbodyConstraints2D.None;
        NStateSpiked spiked = (NStateSpiked)movementStates[8];
        spiked.SetSpikedDirection(direction);
        movementState.SetBool("spiked", true);
        bloodParticles.Play();
        levelManager.RemovePlayer(gameObject);
    }

    public void HitByFire()
    {
        if (living)
        {
            if (frozen != null)
            {
                StopCoroutine(frozen);
                frozen = null;
            }
            movementState.SetBool("frozen", false);
            animator.SetBool("frozen", false);
            fireParticles.Play();
            if (!(onFire != null) && !movementState.GetBool("ashed"))
                onFire = StartCoroutine(DeathByFire());
        }
    }

    public void HitByWater()
    {
        if (living)
        {
            if (onFire != null)
            {
                StopCoroutine(onFire);
                fireParticles.Stop();
                onFire = null;
            }
            movementState.SetBool("frozen", true);
            animator.SetBool("frozen", true);
            if (!(frozen != null))
                frozen = StartCoroutine(Frozen());
        }
    }

    public void HitByPlasma(Vector3 blackHole)
    {
        rigidBody.simulated = false;
        NStateSucc succed = (NStateSucc)movementStates[5];
        succed.SetBlackHolePosition(blackHole);
        living = false;
        if (!movementState.GetBool("succed"))
            levelManager.RemovePlayer(gameObject);
        movementState.SetBool("succed", true);
    }

    public void DeathByFalling()
    {
        living = false;
        levelManager.RemovePlayer(gameObject);
    }

    public void DeathBySpikes(bool h)
    {
        living = false;
        rigidBody.constraints = RigidbodyConstraints2D.None;
        NStateSpiked spiked = (NStateSpiked) movementStates[8];
        Vector2 direction = h ? new Vector2(rigidBody.velocity.x, -rigidBody.velocity.y * 0.5f) : new Vector2(-rigidBody.velocity.x, rigidBody.velocity.y);
        spiked.SetSpikedDirection(direction);
        movementState.SetBool("spiked", true);
        bloodParticles.Play();
        levelManager.RemovePlayer(gameObject);
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
        if (totem.Pickup())
        {
            int weaponsCount = totem.GetWeaponCount();
            EElement element = totem.GetElement();
            int originalWeaponCount = weapons[element].Count;
            foreach (NWeapon air in weapons[EElement.air])
                Destroy(air.gameObject);
            weapons[EElement.air].Clear();

            for (int i = 0; i < weaponsCount && weapons[element].Count < weaponCounts[(int)element]; i++)
            {
                Transform weaponTransform;
                NWeapon w;
                if (totem.GetElement() != EElement.earth)
                {
                    weaponTransform = Instantiate(totem.GetWeapon());
                    w = weaponTransform.GetComponent<NWeapon>();
                }
                else
                {
                    weaponTransform = totem.GetWeapon();
                    w = weaponTransform.GetComponent<NWeaponEarth>();
                    
                }
                w.SetWielder(this, joystick);
                weaponTransform.SetParent(transform);
                weapons[element].Enqueue(w);
            }

            weaponEquipped = element;
            SetAllWeaponAngles();
            weapon = weapons[element].Peek();
        }
    }

    public void TrySpawnAir()
    {
        if (weaponEquipped == EElement.air && weapons[EElement.air].Count < weaponCounts[(int)EElement.air])
        {
            Transform airTransform = Instantiate(airPrefab);
            NWeapon w = airTransform.GetComponent<NWeapon>();
            w.SetWielder(this, joystick);
            airTransform.SetParent(transform);
            weapons[EElement.air].Enqueue(w);
            weaponEquipped = EElement.air;
            SetAllWeaponAngles();
            weapon = weapons[EElement.air].Peek();
        }
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

    public void SetWeaponSpriteDirections(bool flipX)
    {
        foreach (KeyValuePair<EElement,Queue<NWeapon>> e in weapons)
        {
            foreach (NWeapon w in e.Value)
                w.SetSpriteDirection(flipX);
        }
    }

    private IEnumerator Frozen()
    {
        yield return new WaitForSeconds(2f);
        animator.SetBool("frozen", false);
        movementState.SetBool("frozen", false);
        frozen = null;
    }

    private IEnumerator DeathByFire()
    {
        yield return new WaitForSeconds(3f);
        living = false;
        movementState.SetBool("ashed", true);
        fireParticles.Stop();
        levelManager.RemovePlayer(gameObject);
        onFire = null;
    }

    #endregion
}
