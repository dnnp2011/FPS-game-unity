using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    #region Inspector Variables
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask mask;
    #endregion

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShoot: No camera referenced!");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
    }

    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                ShootWeapon();
            } 
        }
        else
        {
            if(Input.GetButtonDown("Fire1"))
            {
                //Repeatedly call X method, after Y wait time, at Z rate
                InvokeRepeating("ShootWeapon", 0f, (60f / currentWeapon.fireRate));
            }
            else if(Input.GetButtonUp("Fire1"))
            {
                //Cancel calling of X method
                CancelInvoke("ShootWeapon");
            }
        }
    }

    [Client]
    void ShootWeapon()
    {
        //Prevents remote players from controlling local shoot function
        if(!isLocalPlayer)
        {
            return;
        }
        //Server call
        CmdOnShoot();
        //Start raycast to find targets
        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))//if hit something
        {
            //Debug.Log(this.name + " fired their weapon");
            if (_hit.collider.gameObject.layer == LayerMask.NameToLayer("RemotePlayer"))
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
            }

            CmdOnHit(_hit.point, _hit.normal);
        }
    }

    //Local Player shoots, activating this server method
    [Command]
    void CmdOnShoot()
    {
        RpcDisplayMuzzleFlash();
    }
    [Command]
    void CmdOnHit(Vector3 _pos, Vector3 _normal)
    {
        RpcDisplayHitEffect(_pos, _normal);
    }

    //Server then calls this method on all clients to display local shooter's muzzleFlash
    [ClientRpc]
    void RpcDisplayMuzzleFlash()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }
    [ClientRpc]
    void RpcDisplayHitEffect(Vector3 _pos, Vector3 _normal)
    {
        GameObject _hitIns = (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffect, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitIns, 2f);
    }

    [Command]
    void CmdPlayerShot(string _PID, int weaponDamage)
    {
        Debug.Log(_PID + " has been shot by " + this.name);

        Player _player = GameManager.GetPlayer(_PID);
        _player.RpcTakeDamage(weaponDamage);
    }
}
