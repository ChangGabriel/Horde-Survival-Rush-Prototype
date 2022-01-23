using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{

    [SerializeField] private Transform attackPoint;

    //Slash attack related
    [SerializeField] private GameObject slashPrefab;
    [SerializeField] private float slashDamage;

    //Axe throw related
    [SerializeField] private GameObject axePrefab;
    [SerializeField] private float axeDamage; //axe rotation speed when thrown
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float axeMaxRange;
    private GameObject axe;
    private Vector3 targetPos; //where we want to the axe to reach
    private bool isThrown;
    private bool canCallBack;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            slashAttack(); //Slash Attack
        }

        if (Input.GetMouseButtonDown(1) && !isThrown) //Axe Throw
        {
            isThrown = true;
            if (!canCallBack)
            {
                targetPos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0); //where mouse is when clicked
                spawnAxe();
            }
        }
        if (isThrown)
        {
            axeThrow();
        }
    }

    private void slashAttack()
    {
        //Handle aiming, shall follow where mousepointer is
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - attackPoint.transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        attackPoint.transform.rotation = Quaternion.Euler(0, 0, rotZ);
        // Handle Slash projection spawn
        GameObject slashSpawn = Instantiate(slashPrefab, attackPoint.transform.position + new Vector3(difference.x, difference.y, 0).normalized, attackPoint.transform.rotation);
        slashSpawn.GetComponent<Slash>().setSlashDamage(slashDamage);
        slashSpawn.transform.parent = attackPoint;

    }

    private void spawnAxe()
    {
        // Handle aiming, shall follow where mousepointer is
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - attackPoint.transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        attackPoint.transform.rotation = Quaternion.Euler(0, 0, rotZ);
        // Handles axe projectile spawn
        axe = Instantiate(axePrefab, attackPoint.transform.position + new Vector3(difference.x, difference.y, 0).normalized, attackPoint.transform.rotation);
        axe.GetComponent<AxeThrow>().setPlayerPos(attackPoint.transform.position);
        axe.GetComponent<AxeThrow>().setAxeDamage(axeDamage);
    }

    // TODO AXE IS CURRENTLY DEALING DAMAGE MULTIPLE TIMES. AXE STOPS WHEN REACHED TARGET OR MAX RANGE
    private void axeThrow()
    {
        // Handle axe movement and booleans
        axe.GetComponent<AxeThrow>().setcanDamage(true);
        axe.GetComponent<AxeThrow>().setisRotating(true);
        if (!canCallBack) //move towards target if axe has not been thrown
        {
            axe.transform.position = Vector2.MoveTowards(axe.transform.position, targetPos, projectileSpeed * Time.deltaTime);
        }
        else
        {
            axe.transform.position = Vector2.MoveTowards(axe.transform.position, attackPoint.transform.position, projectileSpeed * 4 * Time.deltaTime);
        }
        // Handle when axe has reach targeted pos, No damage and rotation
        if (Vector2.Distance(axe.transform.position, targetPos) <= 0.01f)
        {
            isThrown = false;
            canCallBack = true;
            axe.GetComponent<AxeThrow>().setcanKnockback(false);
            axe.GetComponent<AxeThrow>().setisRotating(false);
            axe.GetComponent<AxeThrow>().setcanDamage(false);
        }
        // Handle when axe has reached back to player
        if (Vector2.Distance(axe.transform.position, attackPoint.transform.position) <= 0.01f)
        {
            isThrown = false;
            canCallBack = false;
            Destroy(axe);
        }
    }

}
