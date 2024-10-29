using System.Collections;
using UnityEngine;

public class BulletCounter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] ammoSprites;
    [SerializeField] private float reloadTime;
    public bool canFire = true;
    
    private int currentAmmo;
    private readonly int maxAmmo = 5;

    private void Start()
    {
        spriteRenderer.sprite = ammoSprites[^1];
        currentAmmo = maxAmmo;
    }

    public void ReduceAmmo()
    {
        if (!canFire) return;
        currentAmmo--;
        Debug.Log(currentAmmo + " " + spriteRenderer.sprite.name);
        spriteRenderer.sprite = ammoSprites[currentAmmo];
        if (currentAmmo == 0)
        {
            canFire = false;
            StartCoroutine(Reload());
            currentAmmo = maxAmmo;
        }
    }

    private IEnumerator Reload()
    {
        foreach (var sprite in ammoSprites)
        {
            spriteRenderer.sprite = sprite;
            yield return new WaitForSeconds(reloadTime / ammoSprites.Length);
        }
        canFire = true;
    }
}
