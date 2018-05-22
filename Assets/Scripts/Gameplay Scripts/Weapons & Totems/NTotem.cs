using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NTotem : MonoBehaviour {

    [SerializeField] protected Transform weapon;
    [SerializeField] protected int weaponCount;
    [SerializeField] protected float cooldown;
    [SerializeField] protected EElement element;
    private bool cooling;
    private Animator ac;

    //TODO: Have GameManager pass each totem a list of players, then have a dictionary cache true/false for in-range/not-in-range
    protected Dictionary<GameObject, NPlayerController> playersInRange;

    protected void Start()
    {
        playersInRange = new Dictionary<GameObject, NPlayerController>();
        cooling = false;
        ac = GetComponent<Animator>();
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        NPlayerController np = collider.gameObject.GetComponent<NPlayerController>();
        if (np != null)
        {
            playersInRange[collider.gameObject] = np;
            np.TryAddTotem(this, (np.transform.position - collider.transform.position).magnitude);
        }
    }

    protected void OnTriggerExit2D(Collider2D collider)
    {
        NPlayerController np = collider.gameObject.GetComponent<NPlayerController>();
        if (np != null)
        {
            playersInRange.Remove(collider.gameObject);
            np.RemoveTotem();
        }
    }

    public EElement GetElement()
    {
        return element;
    }

    public Transform GetWeapon()
    {
        return weapon;
    }

    public int GetWeaponCount()
    {
        return weaponCount;
    }

    public bool GetCooledDown()
    {
        return !cooling;
    }

    public void StartCooldown()
    {
        cooling = true;
        ac.SetBool("cooling", cooling);
        StartCoroutine("Cooldown");
    }

    private IEnumerator Cooldown()
    {
        float timer = 0f;
        while (timer <= cooldown)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        cooling = false;
        ac.SetBool("cooling", cooling);
    }
}
