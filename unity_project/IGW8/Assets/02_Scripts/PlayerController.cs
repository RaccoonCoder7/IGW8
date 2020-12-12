using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private SpriteRenderer renderer;
    private Animator anim;
    public AfterImage ai;

    private float moveSpeed=.11f;

    public GameObject bulletObj;
    public SpriteRenderer mask;
    private GameObject revolverObj;
    
    enum State
    {
        Idle=0,
        Moving=1,
        Dead=2
    }

    private State state = State.Idle;

    private bool isDashing;
    private int dashFrames, dashDelay;
    [SerializeField]
    private int dashDir;
    private int bulletCount;
    private float bulletVelocity = 70f;
    [SerializeField]
    private int concentration=2000, constreak;
    private bool isInvulnerable = false;
    private bool isConcentrating = false;
    private List<KeyValuePair<GameObject, Vector2>> deadEyeBullets=new List<KeyValuePair<GameObject, Vector2>>();
    
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        bulletCount = 12;
    }

    void Update()
    {
        Die();
        Dash();
        Reload();
        Fire();
        DeadEye();
        SetUI();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    private void SetUI()
    {
        if (isConcentrating)
        {
            if (concentration > 0) concentration --;
            else ResetTimeScale();
        }
        else
        {
            if (concentration < 2000) concentration++;
        }
        UIManager.Instance.SetConcentrationUI(concentration);
    }
    
    private void Move()
    {
        if (Input.GetAxisRaw("Horizontal") < 0 && dashDir * Input.GetAxisRaw("Horizontal") >= 0)
        {
            renderer.flipX = true;
            anim.SetBool("isMoving",true);
            state = State.Moving;
        }
        
        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            anim.SetBool("isMoving",false);
            state = State.Idle;
        }
        
        if (Input.GetAxisRaw("Horizontal") > 0 && dashDir * Input.GetAxisRaw("Horizontal") >= 0)
        {
            renderer.flipX = false;
            anim.SetBool("isMoving",true);
            state = State.Moving;
        }

        transform.Translate((isDashing ? dashDir : Input.GetAxisRaw("Horizontal"))* moveSpeed, 0, 0);
    }

    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetAxisRaw("Horizontal")!=0 && dashDelay!=0 && isDashing==false)
        {
            isDashing = true;
            dashDir = (int)Input.GetAxisRaw("Horizontal");
            StartInvulnerable();
            ai.StartCoroutine("GenerateAfterImages", new KeyValuePair<Sprite, int>(renderer.sprite, dashDir));
            //anim.SetTrigger("Dash");
            //StartInvulnerable();
        }

        if (isDashing) dashFrames--;
        else dashDelay--;
        if (dashFrames == 0) EndInvulnerable();
    }

    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            anim.SetTrigger("Reload");
        }
    }

    public void Fire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isConcentrating)
            {
                if (bulletCount < 1) return; //Reload Notice
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Debug.Log(mousePos);
                Vector3 charPos = Camera.main.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(transform.position));
                Vector2 trajVector = ((Vector2) mousePos - (Vector2) charPos).normalized;
                float angle = Mathf.Atan2(trajVector.y, trajVector.x) * Mathf.Rad2Deg;
                GameObject obj = Instantiate(bulletObj, transform.position, Quaternion.Euler(new Vector3(0, 0, angle)));
                deadEyeBullets.Add(new KeyValuePair<GameObject, Vector2>(obj,trajVector));
                UIManager.Instance.MarkedForDeath(Input.mousePosition);
                //bulletCount--;
                UIManager.Instance.SetGunChambersUI(bulletCount);
            }
            else
            {
                if (bulletCount < 1) return; //Reload Notice
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Debug.Log(mousePos);
                Vector3 charPos = Camera.main.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(transform.position));
                Vector2 trajVector = ((Vector2) mousePos - (Vector2) charPos).normalized;
                float angle = Mathf.Atan2(trajVector.y, trajVector.x) * Mathf.Rad2Deg;
                GameObject obj = Instantiate(bulletObj, transform.position, Quaternion.Euler(new Vector3(0, 0, angle)));
                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                rb.AddForce(trajVector * 70f, ForceMode2D.Impulse);
                //bulletCount--;
                UIManager.Instance.SetGunChambersUI(bulletCount);
            }

        }
    }

    public void Die()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            anim.SetTrigger("Die");
        state = State.Dead;
    }

    IEnumerator Blackout()
    {
        for (float f = 0; f < .8; f += .0008f)
        {
            mask.color=new Color(0,0,0,f);
            yield return null;
        }
    }
    
    IEnumerator UnleashBullets()
    {
        foreach (var data in deadEyeBullets)
        {
            Rigidbody2D rb = data.Key.GetComponent<Rigidbody2D>();
            rb.AddForce(data.Value*70f,ForceMode2D.Impulse);
            yield return new WaitForSeconds(.1f);
        }
        deadEyeBullets.Clear();
        Invoke("RemoveMarks", .3f);
    }

    public void ReloadBullet()
    {
        bulletCount = 12;
        UIManager.Instance.SetGunChambersUI(12);
    }
    
    public void StartInvulnerable()
    {
        dashFrames = 20;
        renderer.color=new Color(1,1,1,.5f);
        moveSpeed = .25f;
        isInvulnerable = true;
    }

    public void EndInvulnerable()
    {
        dashDelay = 20;
        isDashing = false;
        dashDir = 0;
        renderer.color=Color.white;
        moveSpeed = .11f;
        isInvulnerable = false;
    }

    void DeadEye()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (!isConcentrating && concentration>300)
            {
                AudioManager.Instance.PlayDeadEye();
                SlowTimeScale();
                Invoke("ResetTimeScale",.75f);
            }
            else
            {
                ResetTimeScale();
            }
        }
    }

    public void SlowTimeScale()
    {
        isConcentrating = true;
        Time.timeScale = .1f;
        moveSpeed = 1.1f;
        anim.speed = 4.5f;
        StartCoroutine("Blackout");
    }

    public void ResetTimeScale()
    {
        isConcentrating = false;
        Time.timeScale = 1f;
        moveSpeed = .11f;
        anim.speed = 1f;
        StopCoroutine("Blackout");
        mask.color = Color.clear;
        StartCoroutine("UnleashBullets");
    }
    
    void RemoveMarks()
    {
        UIManager.Instance.RemoveMarks();
    }
}
