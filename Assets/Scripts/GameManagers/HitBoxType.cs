using UnityEngine;
public enum Hitbox
{
    Body,
    Head,
    Shield
}
public class HitboxType : MonoBehaviour 
{
    public Hitbox hitbox;

    public void OnHit()
    {
        gameObject.SetActive(false);
    }
}
