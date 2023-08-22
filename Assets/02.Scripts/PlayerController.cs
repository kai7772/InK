using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static bool haveFoundTheKing = false;

    [SerializeField] private LayerMask platformLayerMask;

    public BoxCollider2D groundChecker;

    public delegate void OnStarCollect();

    public OnStarCollect onStarCollect;

    public GameObject kingJellyDeco;

    #region GameEndVariable
    private bool isDead;
    private bool isGameEnd;
    [SerializeField] private float fdt;
    private float gameEndFdt = 2.5f;
    #endregion
   
    private Rigidbody2D rigid;
    private bool _isInSafeZone = false;

    #region PlayerMoveVariable
    private float horInput;
    private Vector3 playerScale;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float safeZonePlayerSpeed;
    private float playerXScale;
    #endregion

    #region PlayerJumpVariable
    private bool isGround;
    [SerializeField] private bool isJump;

    [SerializeField] private float jumpHeight;
    [SerializeField] private int maxJump;
    private float jumpForce;
    [SerializeField] private int jumpCount;
    #endregion

    #region MonoBehaviour Method

    void Start()
    {
        jumpCount = 0;
        rigid = GetComponent<Rigidbody2D>();
        playerXScale = transform.localScale.x;
        isDead = false;
        UpdateKingJelly();
    }

    void Update()
    {        
        isGround = isGrounded();

        if (!isGround && !_isInSafeZone) fdt += Time.deltaTime;
        
        CheckGameOver();
      
        if (!isDead) PlayerAct();
    }

    private void FixedUpdate()
    {
        if (rigid != null)
        {
            if (rigid.velocity.y < -15)
            {
                rigid.velocity = new Vector3(rigid.velocity.x, -15,0f);
            }
        }
    }
    #endregion

    void PlayerAct()
    {   // check Player Act     
        PlayerJump();
        PlayerMove();
    }

    void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJump && fdt < .1f && !_isInSafeZone)
        {
            ColorManager.instance.AutoSwitchMainColoring();
            rigid.velocity = Vector2.zero;
            jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rigid.gravityScale));
            rigid.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            isJump = true;
        }
    }

    void PlayerMove()
    {
        horInput = Input.GetAxisRaw("Horizontal");
        
        playerScale = transform.localScale;
        if (horInput > 0)
        {
            playerScale.x = playerXScale;
        }
        else if(horInput < 0)
        {
            playerScale.x = -playerXScale;
        }
        transform.localScale = playerScale;
        float _finalSpeed = playerSpeed;
        if (_isInSafeZone) _finalSpeed = safeZonePlayerSpeed;
        transform.Translate(Vector2.right * Time.deltaTime * _finalSpeed * horInput);

        if (_isInSafeZone)
        {
            if (horInput == 0)
            {
                rigid.velocity = new Vector2(0.0f, rigid.velocity.y);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    { // check to player Fall, use OnCollisionExit
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJump = true;
            jumpCount++;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            if (!isGround)
            {
                fdt = 0f;
                isJump = false;
                jumpCount = 0;
            }
            other.gameObject.SetActive(false);

            StartCoroutine(ResetItem());

            IEnumerator ResetItem()
            { // item regenerate after 3 sec
                other.gameObject.SetActive(false);
                yield return new WaitForSeconds(3);
                other.gameObject.SetActive(true);
            }
        }

        if (other.gameObject.CompareTag("Spike"))
        {
            ColoredObject _co = other.gameObject.GetComponent<ColoredObject>();
            if (_co.isCollidable)
            {
                if (isDead == false) StartCoroutine(DeathCoroutine());
            }
        }

        if (other.gameObject.CompareTag("Star"))
        {
            if (other.GetComponent<ColoredObject>().isCollidable)
            {
                if (other.GetComponent<ColoredObject>() == GetComponent<JellyShooter>().jelliedObject)
                {
                    GetComponent<JellyShooter>().RetriveJelly();
                }
                other.gameObject.SetActive(false);

                onStarCollect?.Invoke();
            }
        }

        if (other.gameObject.CompareTag("SafeZone"))
        {
            _isInSafeZone = true;
        }

    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Spike"))
        {
            ColoredObject _co = other.gameObject.GetComponent<ColoredObject>();
            if (_co.isCollidable)
            {
                if (isDead == false) StartCoroutine(DeathCoroutine());
            }
        }

        if (other.gameObject.CompareTag("Star"))
        {
            if (other.GetComponent<ColoredObject>().isCollidable)
            {
                if (other.GetComponent<ColoredObject>() == GetComponent<JellyShooter>().jelliedObject)
                {
                    GetComponent<JellyShooter>().RetriveJelly();
                }
                other.gameObject.SetActive(false);

                onStarCollect?.Invoke();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SafeZone"))
        {
            _isInSafeZone = false;
        }
    }

    bool isGrounded()
    {
        float extraHeightText = 0.2f;
        // RaycastHit2D rayCastHit = Physics2D.Raycast(groundChecker.bounds.center, Vector2.down, groundChecker.bounds.extents.y + extraHeightText, platformLayerMask);
        RaycastHit2D[] _allHits = Physics2D.BoxCastAll(groundChecker.bounds.center, groundChecker.bounds.size, 0f, Vector2.down, extraHeightText, platformLayerMask);
        bool _value = false;

        for (int i = 0; i < _allHits.Length; i++)
        {
            if (_allHits[i].collider != null && !_allHits[i].collider.isTrigger)
            {
                fdt = 0f;
                jumpCount = 0;
                _value = true;
            }
        }
        
        if (_value == false && isJump)
        {
            jumpCount++;
            isJump = false;
        }
        _value = false;

        return _value;
    }

    void CheckGameOver()
    {
        if(fdt > gameEndFdt)
        {
            GameOver();
        }
    }

    public IEnumerator DeathCoroutine()
    {
        isDead = true;
        DeathEffect();
        yield return new WaitForSeconds(1f);
        GameOver();
    }

    void DeathEffect()
    {
        float _minPower = 3f;
        float _maxPower = 9f;
        float _torquePower = 9f;

        for (int i = 0; i < transform.childCount; i++)
        {
            Rigidbody2D _rb2d = transform.GetChild(i).AddComponent<Rigidbody2D>();

            float _angle = Random.Range(0f, 360f);
            Vector2 _dir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * _angle), Mathf.Sin(Mathf.Deg2Rad * _angle));
            float _power = Random.Range(_minPower, _maxPower);
            _rb2d.AddForce(_dir * _power, ForceMode2D.Impulse);

            int _rnd = Random.Range(0, 2);
            _rb2d.AddTorque((_rnd * 2f - 1f) * _torquePower,ForceMode2D.Impulse);

            if (transform.GetChild(i).GetComponent<Collider2D>() != null) transform.GetChild(i).GetComponent<Collider2D>().enabled = false;
        }
        transform.DetachChildren();
    }

    void GameOver()
    {
        if (isGameEnd == true) return;

        isGameEnd = true;
        UIManager.instance._isGameEnd = true;
    }

    public void UpdateKingJelly()
    {
        kingJellyDeco.SetActive(haveFoundTheKing);
    }
}
