using System.Collections;
using UnityEngine;

public class BulletCounter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] ammoSprites;
    [SerializeField] private float reloadTime;
    [SerializeField] private float reloadCooldown = 2f;
    public bool canFire = true;
        
    public int currentAmmo;
    private readonly int maxAmmo = 5;
    private Coroutine activeCoroutine;

    private void Start()
    {
        spriteRenderer.sprite = ammoSprites[^1];
        spriteRenderer.color = Color.white;
        currentAmmo = maxAmmo;
        activeCoroutine = null;
    }

    public void ReduceAmmo()
    {
        if (!canFire) return;
        currentAmmo--;
        spriteRenderer.sprite = ammoSprites[currentAmmo];
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        activeCoroutine = StartCoroutine(ReloadCooldown());
        if (currentAmmo == 0)
        {
            canFire = false;
            spriteRenderer.color = Color.red;
            StopCoroutine(activeCoroutine);
            StartCoroutine(ReloadCooldown());
        }
    }
    public IEnumerator ReloadCooldown ()
    {
        yield return new WaitForSeconds(reloadCooldown);
        activeCoroutine = StartCoroutine(Reload());
    }
    private IEnumerator Reload()
    {
        for (int i = currentAmmo; i < ammoSprites.Length - 1; i++)
        {
            currentAmmo++;
            spriteRenderer.sprite = ammoSprites[currentAmmo];
            yield return new WaitForSeconds(reloadTime / ammoSprites.Length);
        }
        canFire = true;
        if (spriteRenderer.color != Color.white)
        {
            spriteRenderer.color = Color.white;
        }
    }
}
