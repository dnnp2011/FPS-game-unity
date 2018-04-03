using UnityEngine;

[System.Serializable]
public class PlayerWeapon {

    public string name = "Sci-Fi Automatic";
    public int damage = 25;
    public float range = 150f;
    public float fireRate = 0f;
    public GameObject graphics;

    #region Future Idea
    interface IWeaponInterface
    {
        string WeaponName { get; set; }
        int WeaponDamage { get; set; }
        float WeaponRange { get; set; }

        //GFX
        //Sounds, ETC
    }

    [System.Serializable]
    protected class SF_Auto : IWeaponInterface
    {
        //Get rid of constructor, artificially set prop in set field using serializable variable
        public string WeaponName { get; set; }
        public int WeaponDamage { get; set; }
        public float WeaponRange { get; set; }

        SF_Auto()
        {
            this.WeaponName = "Sci-Fi Automatic";
            this.WeaponDamage = 15;
            this.WeaponRange = 150f;
        }
    }

    [System.Serializable]
    protected class Kin_Pistol : IWeaponInterface
    {
        public string WeaponName { get; set; }
        public int WeaponDamage { get; set; }
        public float WeaponRange { get; set; }

        Kin_Pistol()
        {
            this.WeaponName = "Kinetic Pistol";
            this.WeaponDamage = 25;
            this.WeaponRange = 75f;
        }
    } 
    #endregion
}
