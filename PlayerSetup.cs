using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour
{

    #region Inspector Variables
    [Header("Isolating Local Player Settings:")]
    [SerializeField]
    private string remoteLayerName = "RemotePlayer";
    [SerializeField]
    private string cullingLayer = "DontDraw";
    [SerializeField]
    private GameObject cullingGraphic;
    [SerializeField]
    private Behaviour[] componentsToDisable;

    [Header("Crosshair Settings")]
    [SerializeField]
    GameObject playerUIPrefab;
    private GameObject playerUIInstance;

    #endregion

    Camera sceneCam;

    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents(componentsToDisable);
            AssignRemoteLayer();
        }
        else
        {
            DisableSceneCam();
            //Disable armor mesh for local player(blocks view)
            CullGraphicLayer(cullingGraphic, LayerMask.NameToLayer(cullingLayer));
            //Create PlayerUI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;
        }

        GetComponent<Player>().Setup();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        GameManager.RegisterPlayer(this.GetComponent<NetworkIdentity>().netId.ToString(), this.GetComponent<Player>());
    }

    void AssignRemoteLayer()
    {
        this.gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents(Behaviour[] componentsToDisable)
    {
        //If player accessing this script isn't the local player, disable these items
        foreach (var i in componentsToDisable)
        {
            i.enabled = false;
        }
    }

    void DisableSceneCam()
    {
        //If they are local player, disable scene camera
        sceneCam = Camera.main;

        if (sceneCam != null)
        {
            sceneCam.gameObject.SetActive(false);
        }
    }

    void CullGraphicLayer(GameObject obj, int newLayer)
    {
        //Used alternate method to no load certain self graphics from tutorial
        //As I'm already using layers to determine if an object im hitting is myself or remote player
        MeshRenderer render = obj.GetComponent<MeshRenderer>();
        render.enabled = false;
    }

    void OnDisable()
    {
        Destroy(playerUIInstance);

        if (sceneCam != null)
        {
            sceneCam.gameObject.SetActive(true);
        }

        GameManager.UnregisterPlayer(this.transform.name);
    }
}