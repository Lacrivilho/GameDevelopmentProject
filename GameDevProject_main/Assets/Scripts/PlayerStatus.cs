using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class PlayerStatus : MonoBehaviour
{
    public int maxHealth = 100;
    public int maxAmmo = 50;
    int health;
    public int ammo;

    public float vignetteScaleInit = 2.244f;
    public float vignetteScaleBig = 3f;

    //UI elements:
    public GameObject ammoObject;
    public GameObject healthBar;
    public GameObject vignette;
    public GameObject splatterMap;
    public Texture[] ammoStates;
    public Texture[] splatterStates;

    Vector3 defaultPosition;
    public Vector3 emptyPosition;

    Coroutine vignetteAnimation = null;


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        ammo = maxAmmo;

        defaultPosition = healthBar.transform.position;
        emptyPosition = emptyPosition + healthBar.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Move Health bar:
        healthBar.transform.position = Vector3.Lerp(emptyPosition, defaultPosition, (float)health / (float)maxHealth);

        //Set ammo state
        if(ammo == 0)
        {
            ammoObject.GetComponent<RawImage>().texture = ammoStates[0];
        }
        else if (ammo < 5)
        {
            ammoObject.GetComponent<RawImage>().texture = ammoStates[1];
        }
        else if(ammo < 10)
        {
            ammoObject.GetComponent<RawImage>().texture = ammoStates[2];
        }
        else if(ammo < 15)
        {
            ammoObject.GetComponent<RawImage>().texture = ammoStates[3];
        }
        else
        {
            ammoObject.GetComponent<RawImage>().texture = ammoStates[4];
        }

        //Set splatter map:
        if(health > 70)
        {
            splatterMap.GetComponent<RawImage>().texture = splatterStates[0];
        }
        else if (health > 30)
        {
            splatterMap.GetComponent<RawImage>().texture = splatterStates[1];
        }
        else if (health > 10)
        {
            splatterMap.GetComponent<RawImage>().texture = splatterStates[2];
        }
        else
        {
            splatterMap.GetComponent<RawImage>().texture = splatterStates[3];
        }
    }

    public void damage(int amount)
    {
        health -= amount;
        if(vignetteAnimation == null)
        {
            vignetteAnimation = StartCoroutine(ScaleObjectOverTime(vignette, vignetteScaleInit, vignetteScaleBig, 0.2f));
        }
        else
        {
            StopCoroutine(vignetteAnimation);
            vignetteAnimation = StartCoroutine(ScaleObjectOverTime(vignette, vignetteScaleInit, vignetteScaleBig, 0.2f));
        }

        if(health < 0)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            GameManager.Instance.win = false;
            GameManager.Instance.gameOver = true;
            SceneManager.LoadScene(0);
        }
        
    }

    public void heal(int ammount)
    {
        health += ammount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public bool bulletsLeft()
    {
        if(ammo > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void shootBullet()
    {
        ammo--;
    }

    public void reload(int ammount)
    {
        ammo += ammount;
        if(ammo > maxAmmo)
        {
            ammo = maxAmmo;
        }
    }

    IEnumerator ScaleObjectOverTime(GameObject obj, float smallScale, float bigScale, float duration)
    {
        // Set the object to bigScale
        obj.transform.localScale = Vector3.one * bigScale;

        // Calculate the scaling speed based on duration
        float scalingSpeed = (bigScale - smallScale) / duration;

        // Scale down to smallScale
        while (obj.transform.localScale.x > smallScale)
        {
            float newScale = obj.transform.localScale.x - scalingSpeed * Time.deltaTime;
            obj.transform.localScale = Vector3.one * Mathf.Max(newScale, smallScale);
            yield return null;
        }

        // Scale up to bigScale
        while (obj.transform.localScale.x < bigScale)
        {
            float newScale = obj.transform.localScale.x + scalingSpeed * Time.deltaTime;
            obj.transform.localScale = Vector3.one * Mathf.Min(newScale, bigScale);
            yield return null;
        }

        // Ensure the object is at bigScale at the end
        obj.transform.localScale = Vector3.one * bigScale;

        vignetteAnimation = null;
    }
}
