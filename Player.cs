using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(NetworkManager))]
public class Player : NetworkBehaviour {

    [Header("Player Attributes:")]
    [SerializeField]
    private int maxHealth = 100;
    [SyncVar] //Syncs this variable across all connected clients
    private int currentHealth;
    [SyncVar]
    private bool _isDead = false; //Cannot mark property itself as SyncVar so the prop and private var are seperated
    public bool IsDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }
    [Space]
    [Header("Respawn Mechanics:")]
    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

	public void Setup()
	{
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }
        SetDefaults();
	}

    public void SetDefaults()
    {
        IsDead = false;
        currentHealth = maxHealth;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;
        Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Update()
    {
        //Kill code for testing
        if (!isLocalPlayer)
            return;
        if (Input.GetKeyDown(KeyCode.K))
            RpcTakeDamage(100);
    }

    [ClientRpc] //Server run method that activates on all connected clients
    public void RpcTakeDamage(int _amount)
    {
        if (IsDead)
            return;
        currentHealth -= _amount;
        Debug.Log(String.Format("{0} has taken {1} damage and now has {2} health", this.name, _amount, currentHealth));

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        IsDead = true;
        //Disable Player Components
        Debug.Log(String.Format("{0} has died", this.name));
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;

        Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        rb.useGravity = true;

        //Call Respawn
        StartCoroutine(Respawn());
    }
    
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        SetDefaults();
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        Debug.Log(this.name + " has respawned");
    }
}