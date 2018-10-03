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
    public NStateSO bd;

    public NStateInfo(NPlayerController p, Rigidbody2D r, BoxCollider2D b, Player j, NStateSO bd)
    {
        this.p = p;
        this.r = r;
        this.b = b;
        this.j = j;
        this.bd = bd;
    }
}

public class NPlayerController : MonoBehaviour {

    #region Members
    /// Private Members
    
    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;
    private Animator[] animators;
    private float bodySync;
    private SpriteRenderer[] spriteRenderers;
    [SerializeField] private NStateSO balanceData;
    [SerializeField] private int playerNumber;
    [SerializeField] private float weaponCooldownTime;

    [Tooltip("air, fire, water, earth, plasma")]
    [SerializeField] private int[] weaponCounts;
    [SerializeField] private Transform airPrefab;
    [SerializeField] private Transform frozenPrefab;
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private ParticleSystem bloodParticles;
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
    protected NWeaponPlasma plasmaPull;
    private float totemDist;

    private NState[] movementStates;
    private NState movementState;
    private Dictionary<string, bool> movementBools;
    public EState state;

    private bool grabButton;
    private bool dischargeCooldown;
    private bool swapButton;
    private Vector2 rightStick;

    public Animator armMachine;
    public Animator legMachine;
    #endregion

    #region Monobehaviour
    void Start() {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animators = new Animator[4];
        animators[0] = transform.GetChild(2).GetComponent<Animator>();
        animators[1] = transform.GetChild(2).GetChild(0).GetComponent<Animator>();
        animators[2] = transform.GetChild(2).GetChild(1).GetComponent<Animator>();
        animators[3] = transform.GetChild(2).GetChild(2).GetComponent<Animator>();
        spriteRenderers = new SpriteRenderer[4];
        spriteRenderers[0] = transform.GetChild(2).GetComponent<SpriteRenderer>();
        spriteRenderers[1] = transform.GetChild(2).GetChild(0).GetComponent<SpriteRenderer>();
        spriteRenderers[2] = transform.GetChild(2).GetChild(1).GetComponent<SpriteRenderer>();
        spriteRenderers[3] = transform.GetChild(2).GetChild(2).GetComponent<SpriteRenderer>();

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
        movementBools["active"] = false;
        movementBools["dodged"] = false;
        movementBools["boosted"] = false;
        NStateInfo info = new NStateInfo(this, rigidBody, boxCollider, joystick, balanceData);
        movementStates = new NState[16];
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
        movementStates[12] = new NStateInactive(info, EState.inactive);
        movementStates[13] = new NStateAirDodge(info, EState.airDodge);
        movementStates[14] = new NStateSuspended(info, EState.suspended);
        movementStates[15] = new NStateFireBoost(info, EState.fireBoost);
        movementState = movementStates[12];

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
        SetBodySyncTime();

        if (movementState != null)
        {
            movementState.StateUpdate();
        }

        if (living)
        {
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
                if (onFire != null && totem.GetElement() == EElement.water)
                {
                    Extinguish();
                    Destroy(weapons[EElement.water].Dequeue().gameObject);
                    SetWeaponAngles(weaponEquipped, Mathf.Infinity, Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
                    weapon = weapons[weaponEquipped].Peek();
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
    }

    void FixedUpdate()
    {
        if (movementState != null)
        {
            movementState.StateFixedUpdate();
        }
        //if (!(weapon != null))
        //{
        //    SetAnimatorBools("casting", false);
        //}
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        NPlayerController otherPlayer = col.gameObject.GetComponent<NPlayerController>();
        if (otherPlayer != null && onFire != null)
            otherPlayer.HitByFire(Vector2.zero);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        NWeaponPlasma plasma = collider.transform.GetComponentInParent<NWeaponPlasma>();
        if (plasma != null)
            plasmaPull = plasma;
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        NWeaponPlasma plasma = collider.transform.GetComponentInParent<NWeaponPlasma>();
        if (plasma != null)
            plasmaPull = null; ;
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

    public NWeaponPlasma GetPlasmaPull()
    {
        return plasmaPull;
    }

    public Rigidbody2D GetRigidbody()
    {
        return rigidBody;
    }

    public Vector2 GetNextFramePosition()
    {
        return rigidBody.position + (rigidBody.velocity * Time.fixedDeltaTime);
    }

    public bool GetWeaponCooled()
    {
        return !dischargeCooldown;
    }

    public void SetAnimators(RuntimeAnimatorController[] controllers)
    {
        for (int i = 0; i < 4; i++)
            animators[i].runtimeAnimatorController = controllers[i];
    }

    public void SetPants(Color p)
    {
        spriteRenderers[3].color = p;
    }

    public void SpriteFlipX(bool flip)
    {
        foreach (SpriteRenderer sr in spriteRenderers)
            sr.flipX = flip;
    }

    public bool GetFlipX()
    {
        return spriteRenderers[0].flipX;
    }

    public void SetAnimatorBools(string param, bool b)
    {
        foreach (Animator ac in animators)
            ac.SetBool(param, b);
    }

    public void SetAnimatorFloats(string param, float f)
    {
        foreach (Animator ac in animators)
            ac.SetFloat(param, f);
    }

    public void SetAnimatorTriggers(string param)
    {
        foreach (Animator ac in animators)
            ac.SetTrigger(param);
    }

    public void Bounce()
    {
        movementState.SetBool("bounced", true);
    }

    public void WeaponUsed()
    {
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
                weaponEquipped = EElement.air;
            }
        }
        dischargeCooldown = true;
        StartCoroutine("WeaponCooldown");
    }

    public void HitByAir(Vector2 direction)
    {
        NStatePushed pushed = (NStatePushed)movementStates[6];
        pushed.SetPushedDirection(direction);
        pushed.SetPushedVelocity(7.5f);
        SetMovementBool("pushed", true);
    }

    public void AirMobility()
    {
        NStateAirDodge dodged = (NStateAirDodge)movementStates[13];
        dodged.SetDashDirection(new Vector2(joystick.GetAxis("Move Horizontal"), joystick.GetAxis("Move Vertical")));
        SetMovementBool("dodged", true);
    }

    public void HitByEarth(Vector2 direction)
    {
        living = false;
        rigidBody.constraints = RigidbodyConstraints2D.None;
        NStateSpiked spiked = (NStateSpiked)movementStates[8];
        spiked.SetSpikedDirection(direction);
        movementState.SetBool("spiked", true);
        bloodParticles.Play();
        ReleaseAllWeapons();
        levelManager.RemovePlayer(gameObject);
    }

    public void HitByFire(Vector2 direction)
    {
        if (living)
        {
            if (frozen != null)
            {
                StopCoroutine(frozen);
                frozen = null;
            }
            movementState.SetBool("frozen", false);
            SetAnimatorsPlaying(true);

            if (direction != Vector2.zero)
            {
                NStatePushed pushed = (NStatePushed)movementStates[6];
                pushed.SetPushedDirection(direction);
                pushed.SetPushedVelocity(10f);
                movementState.SetBool("pushed", true);
            }
            
            fireParticles.Play();
            if (!(onFire != null) && !movementState.GetBool("ashed"))
                onFire = StartCoroutine(DeathByFire());
        }
    }

    public float FireMobility()
    {
        NStateSuspended suspended = (NStateSuspended)movementStates[14];
        NStateFireBoost boosted = (NStateFireBoost)movementStates[15];
        Vector2 boostDirection = new Vector2(joystick.GetAxis("Move Horizontal"), joystick.GetAxis("Move Vertical"));
        if (boostDirection == Vector2.zero)
            boostDirection = GetFlipX() ? new Vector2(-1f, 0f) : new Vector2(1f, 0f);
        boosted.SetBoostDirection(boostDirection);
        SetMovementBool("boosted", true);
        return suspended.GetSuspensionTime();
    }

    private IEnumerator DelayedFireMobility(float suspensionTime, Vector2 boostDirection)
    {
        yield return new WaitForSeconds(suspensionTime);
        // TO DO:
        // Use this function to universalize the delayed fire boost between the movement state machine,
        // the animator, and the weapon itself
    }

    public void FireMobilityCollision(Vector2 direction)
    {
        NStatePushed pushed = (NStatePushed)movementStates[6];
        pushed.SetPushedDirection(direction);
        pushed.SetPushedVelocity(10f);
        SetMovementBool("pushed", true);
    }

    public void HitByWater()
    {
        if (living)
        {
            if (onFire != null)
            {
                Extinguish();
            }
            movementState.SetBool("frozen", true);
            SetAnimatorsPlaying(false);
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

    public void Extinguish()
    {
        StopCoroutine(onFire);
        onFire = null;
        fireParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
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
        ReleaseAllWeapons();
        levelManager.RemovePlayer(gameObject);
    }

    public void LevelManagerInitialize(NLevelManager levelManager)
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

    public void Vibrate(int motorIndex, float motorLevel, float duration)
    {
        if (joystick != null)
            joystick.SetVibration(motorIndex, motorLevel, duration);
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

    private void ReleaseAllWeapons()
    {
        foreach (KeyValuePair<EElement, Queue<NWeapon>> e in weapons)
        {
            while (e.Value.Count != 0)
                e.Value.Dequeue().WielderDied();
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
        Transform frozenEffect = Instantiate(frozenPrefab);
        frozenEffect.SetParent(transform);
        frozenEffect.localPosition = new Vector3(0f,0f,0f);
        yield return new WaitForSeconds(2f);
        Destroy(frozenEffect.gameObject);
        movementState.SetBool("frozen", false);
        SetAnimatorsPlaying(true);
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
        ReleaseAllWeapons();
    }

    private IEnumerator WeaponCooldown()
    {
        yield return new WaitForSeconds(weaponCooldownTime);
        dischargeCooldown = false;
    }

    private void SetBodySyncTime()
    {
        float speedX = animators[2].GetFloat("speedX");
        float maxSpeed = balanceData.normalMaxLateralVelocity;
        float minSpeed = 0;
        float ratio = (speedX - minSpeed) / (maxSpeed - minSpeed);
        float maxMultiplier = 2;
        float minMultiplier = 1;
        float multiplier = (ratio * (maxMultiplier - minMultiplier)) + minMultiplier;
        multiplier = multiplier == minMultiplier ? 0.35f : multiplier;
        bodySync += Time.deltaTime * multiplier;
        bodySync = bodySync > 1f ? bodySync - 1f : bodySync;
        SetAnimatorFloats("bodySync", bodySync);
    }

    private void SetAnimatorsPlaying(bool p)
    {
        foreach (Animator animator in animators)
            animator.enabled = p;
    }
    #endregion
}
