using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private SpriteRenderer renderer;
    private Animator anim;
    public AfterImage ai;
    public GameObject eyelight;

    private float moveSpeed=.11f;

    public GameObject bulletObj;
    private GameObject[] bulletObjs=new GameObject[20];
    private int bulletIdx;
    public SpriteRenderer mask;
    private GameObject revolverObj;
    
    enum State
    {
        Idle=0,
        Moving=1,
        Dead=2
    }

    private State state = State.Idle;
    [SerializeField]
    private bool isDashing;
    [SerializeField]
    private int dashFrames, dashDelay, counterFrames;
    private int conAdjustFrame1, conAdjustFrame2, conAdjustFrame3;
    [SerializeField]
    private int dashDir;
    private int bulletCount;
    private float bulletVelocity = 50f;
    [SerializeField] private int concentration = 2000; //<<데드아이 수치 스테이지 클리어시 증가할 수 있음(UIManager쪽 Fillamound도 같이 수정해야함)
    private bool isInvulnerable = false;
    private bool isConcentrating = false;
    private List<KeyValuePair<GameObject, Vector2>> deadEyeBullets=new List<KeyValuePair<GameObject, Vector2>>();
    
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        
        for (int i = 0; i < 20; i++)
        {
            bulletObjs[i] = Instantiate(bulletObj);
        }
        bulletCount = 12;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetAxisRaw("Horizontal") != 0 && dashDelay != 0 &&
            isDashing == false) Dash();
        
        if (counterFrames > 0) //회피 성공시 (카운터 가능)
        {
            renderer.color=Color.black;
        }

        if (isConcentrating) //데드아이 발동중
        {
            conAdjustFrame1++; //데드아이 timescale(0.1배) 보정용
            conAdjustFrame2++;
            conAdjustFrame3++;

            if (conAdjustFrame1 == 10)
            {
                conAdjustFrame1 = 0;
                counterFrames--;
            }

            if (isDashing)
            {
                if (conAdjustFrame2 == 10)
                {
                    conAdjustFrame2 = 0;
                    dashFrames--;
                }
            }
            else
            {
                if (conAdjustFrame3 == 0)
                    dashDelay--;
            }
            if (dashFrames == 0) EndInvulnerable();
        }

        else
        {
            counterFrames--;
            if (isDashing) dashFrames--;
            else dashDelay--;
            if (dashFrames == 0) EndInvulnerable();
        }

        if (counterFrames > 0)
        {
            eyelight.SetActive(true);
        }
        else
        {
            eyelight.SetActive(false);
        }
        
        if (Input.GetMouseButtonDown(0)) Fire();

        if (Input.GetKeyDown(KeyCode.R)) Reload();
        if (Input.GetKeyDown(KeyCode.V)) DeadEye();
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
    
    private void Move() //이동
    {
        if (state == State.Dead) return;
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

    private void Dash() //대시(무적)
    {
        if (state == State.Dead) return;
        {
            isDashing = true;
            dashDir = (int)Input.GetAxisRaw("Horizontal");
            StartInvulnerable();
            ai.StartCoroutine("GenerateAfterImages", new KeyValuePair<Sprite, int>(renderer.sprite, dashDir)); //잔상
            AudioManager.Instance.PlayDash();
        }
    }

    private void Reload() //재장전
    {
        anim.SetTrigger("Reload");
        AudioManager.Instance.PlayReload();
    }

    public void Fire() //발사
    {
        if (state == State.Dead) return;
        {
            if (bulletCount < 1)
            {
                AudioManager.Instance.PlayEmpty();
                return; //Reload Notice
            }

            if (isConcentrating) //데드아이 사용중(기존이랑 다름)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 charPos = Camera.main.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(transform.position));
                Vector2 trajVector = ((Vector2) mousePos - (Vector2) charPos).normalized;
                float angle = Mathf.Atan2(trajVector.y, trajVector.x) * Mathf.Rad2Deg;
                GameObject obj=bulletObjs[bulletIdx++%20];
                obj.transform.position = transform.position;
                obj.transform.rotation=Quaternion.Euler(new Vector3(0,0,angle));
                obj.GetComponent<SpriteRenderer>().enabled = false;
                deadEyeBullets.Add(new KeyValuePair<GameObject, Vector2>(obj, trajVector));
                UIManager.Instance.MarkedForDeath(Input.mousePosition);
                bulletCount--;
                UIManager.Instance.SetGunChambersUI(bulletCount);
            }
            else //평상시
            {
                if (counterFrames > 0) //카운터 성공시
                {
                    Collider2D[] c2ds = Physics2D.OverlapCircleAll(transform.position, .3f,11);
                    foreach (var col in c2ds)
                    {
                        if (col.tag == "EnemyBullet")
                        {
                            Destroy(col.gameObject); //반경 0.3f내의 적 총알 파괴
                        }
                    }

                    counterFrames = 0; //카운터 종료
                    AudioManager.Instance.PlayCounter();
                    Invoke("Fire",.1f); //한번 더 발사
                }
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 charPos = Camera.main.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(transform.position));
                Vector2 trajVector = ((Vector2) mousePos - (Vector2) charPos).normalized;
                float angle = Mathf.Atan2(trajVector.y, trajVector.x) * Mathf.Rad2Deg;
                GameObject obj=bulletObjs[bulletIdx++%20];
                obj.transform.position = transform.position;
                obj.transform.rotation=Quaternion.Euler(new Vector3(0,0,angle));
                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                rb.AddForce(trajVector * 70f, ForceMode2D.Impulse);
                bulletCount--;
                UIManager.Instance.SetGunChambersUI(bulletCount);
                AudioManager.Instance.PlayGunshot();
            }
        }
    }

    public void Die() //사망
    {
        AudioManager.Instance.PlayDeath();
        anim.SetTrigger("Die");
        state = State.Dead;
    }

    IEnumerator Blackout() //데드아이시 화면 페이드아웃
    {
        for (float f = 0; f < .8; f += .0008f)
        {
            mask.color=new Color(0,0,0,f);
            yield return null;
        }
    }
    
    IEnumerator UnleashBullets() //데드아이시 한번에 총알 발사
    {
        UIManager.Instance.RemoveMarks();
        foreach (var data in deadEyeBullets)
        {
            Rigidbody2D rb = data.Key.GetComponent<Rigidbody2D>();
            rb.AddForce(data.Value*70f,ForceMode2D.Impulse);
            data.Key.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(.1f);
            AudioManager.Instance.PlayGunshot();
        }
        deadEyeBullets.Clear();
    }

    public void ReloadBullet() //재장전
    {
        bulletCount = 12;
        UIManager.Instance.SetGunChambersUI(12);
    }
    
    public void StartInvulnerable()//무적(대시) 시작
    {
        dashFrames = 20;
        renderer.color=new Color(1,1,1,.5f);
        moveSpeed = .25f;
        isInvulnerable = true;
    }

    public void EndInvulnerable()//무적(대시) 종료
    {
        dashDelay = 20;
        isDashing = false;
        dashDir = 0;
        renderer.color=Color.white;
        moveSpeed = .11f;
        isInvulnerable = false;
    }

    void DeadEye()//데드아이
    {
        if (!isConcentrating && concentration > 300)
        {
            SlowTimeScale();
        }
        else
        {
            ResetTimeScale();
        }
    }

    public void SlowTimeScale()//시간 느리게
    {
        isConcentrating = true;
        Time.timeScale = .1f;
        moveSpeed = 1.1f;
        anim.speed = 4.5f;
        StartCoroutine("Blackout");
        AudioManager.Instance.PlayDeadEye();
    }

    public void ResetTimeScale()//시간 정상화
    {
        isConcentrating = false;
        Time.timeScale = 1f;
        moveSpeed = .11f;
        anim.speed = 1f;
        StopCoroutine("Blackout");
        mask.color = Color.clear;
        StartCoroutine("UnleashBullets");
        AudioManager.Instance.StopPlayingDeadEye();
    }
    
    
    private void OnTriggerEnter2D(Collider2D other)//적총알 충돌
    {
        if (other.tag.Equals("EnemyBullet"))
        {
            if (isDashing)
            {
                counterFrames = 20;
            }
            
            else
            {
                Die();
            }
        }
    }
}
