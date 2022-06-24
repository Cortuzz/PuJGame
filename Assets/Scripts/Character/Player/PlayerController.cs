using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Character.Character, IObservable
{
    public float speed;
    public float jumpForce;

    public float jumpTicks;
    public float jumpSpeed;
    public float maxJumpSpeed;
    public float holdJumpTime;
    public bool holdingJump;
    public bool blockAction;
    public bool removingPrimaryBlock = true;

    private bool _inventoryShowing = false;
    private Vector2 _mousePosition;
    public Inventory inventory;
    public HealthBarController hpController;
    public GameObject arrowPrefab;
    public Item arrowItem;
    
    private Item _activeItem;
    private GameObject _activeItemObject;
    private bool _charging;

    private readonly List<IObserver> _observers = new List<IObserver>();

    private int _healthUpdateCounter = 0;
    public int _attackTimeout = 200;
    private int _attackCounter;

    protected override void Awake()
    {
        inventory = GetComponent<Inventory>();
        base.Awake();
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        if (other as EdgeCollider2D)
        {
            TakeDamage(other.GetComponent<Executioner>().weaponDamage);
            return;
        }
        TakeDamage(other.GetComponent<Enemy>().bodyDamage);
    }

    public void Attack()
    {
        if (!(_activeItem && _activeItem.isWeapon))
            return;

        if (_activeItem.name == "Bow" && inventory.IsContains(arrowItem) && _attackCounter <= 0)
        {
            _attackCounter = _attackTimeout;
            inventory.Remove(arrowItem);
            _charging = true;
            _animator.Play("Idle");
            _activeItemObject.GetComponent<SpriteRenderer>().sprite = _activeItem.additionalSprite;
            _activeItemObject.transform.localScale = new Vector3(-0.75f, 0.75f, 1);
            _activeItemObject.transform.localPosition = new Vector3(0, -0.2f, 1);
            _activeItemObject.transform.rotation = new Quaternion(50f, 1f, 0f, 0f);
            //_activeItemObject.transform.localPosition = new Vector3(-0.35f, -0.3f, 0);
        }
    }

    public void SpawnArrow()
    {
        var arrow = Instantiate(arrowPrefab, transform.parent);
        var position = transform.position;
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var positionDif = mousePos - position;
        var hyp = Mathf.Sqrt(Mathf.Pow(positionDif.x, 2) + Mathf.Pow(positionDif.y, 2));
        var trigonometrical = positionDif / hyp;
        var sign = Mathf.Sign(transform.localScale.x);

        position.x -= Mathf.Sign(sign);
        arrow.transform.position = position;
        
        var rb = arrow.GetComponent<Rigidbody2D>();
        rb.velocity = 20 * trigonometrical;
    }

    public void CheckActiveItemDown()
    {
        if (!_activeItem || _activeItem.name != "Bow" || !_charging)
            return;
        
        _animator.Play("Bow Charging");
        _activeItemObject.GetComponent<SpriteRenderer>().sprite = _activeItem.sprite;
        _activeItemObject.transform.localScale = new Vector3(0.35f, 0.35f, 1);
        _activeItemObject.transform.localPosition = new Vector3(-0.25f, -0.2f, 0);
        
        _charging = false;
        SpawnArrow();
    }

    public void SetActiveItem(Item item, GameObject itemObject)
    {
        if (_activeItem == item)
            return;
        
        Destroy(_activeItemObject);
        if (!item)
        {
            _activeItem = null;
            _activeItemObject = null;
            return;
        }

        _activeItem = item;
        _activeItemObject = new GameObject(name = "Active tile");
        _activeItemObject.transform.parent = transform.GetChild(0);
        _activeItemObject.AddComponent<SpriteRenderer>().sprite =
            itemObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite;

        _activeItemObject.transform.rotation = Quaternion.identity;
        _activeItemObject.transform.localScale = new Vector3(0.35f, 0.35f, 1);
        _activeItemObject.transform.localPosition = new Vector3(-0.25f, -0.2f, 0);

        //_activeItemObject = Instantiate(itemObject.transform.GetChild(0).gameObject, transform);
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    public override void SetOnGround(bool onGround)
    {
        if (onGround)
            jumpTicks = 0;
        this.onGround = onGround;
    }

    public override void Die()
    {
        Spawn();
    }

    protected override Collider GetCollider()
    {
        return new(transform, _rb);
    }

    public override void CheckCollisions()
    {
        var collision = _collider.CheckBottomCollision();
        var fallDamage = (int)(jumpTicks / 5 - 10);
        if (collision && fallDamage > 0)
            TakeDamage(fallDamage); // TODO: �������� � ����� ������� / ��������� �������
        SetOnGround(collision);

        _collider.CheckTopCollision();
        _collider.CheckSideCollision();
    }

    private void RegenerateUpdate()
    {
        if (_regeneration > 0)
            return;
        
        _healthUpdateCounter++;
        
        if (health >= maxHealth)
            return;

        if (_healthUpdateCounter < 50) 
            return;

        _healthUpdateCounter = 0;
        health++;
    }

    public override void MoveUpdate()
    {
        horizontalSpeed = Input.GetAxis("Horizontal");

        Vector2 movement = new(horizontalSpeed * speed, Mathf.Max(_rb.velocity.y, -50));

        if (onGround)
        {
            if (Input.GetKeyDown(KeyCode.Space) && holdingJump == false)
            {
                holdJumpTime = Time.time;
                holdingJump = true;
            }

            if (Input.GetKeyUp(KeyCode.Space) && holdingJump)
            {
                holdingJump = false;
                SetOnGround(false);
                movement.y = Mathf.Clamp(jumpForce * (Time.time - holdJumpTime), 5, maxJumpSpeed);
            }
        }

        if (CheckFall())
            ++jumpTicks;
        else
            jumpTicks = 0;

        _rb.velocity = movement;
    }

    public Item GetItem()
    {
        return inventory.GetActiveItem();
    }

    public bool CheckFall()
    {
        return _rb.velocity.y < 0 && !onGround;
    }

    public Vector2 GetMousePos()
    {
        return _mousePosition;
    }

    private void InventoryUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            _inventoryShowing = !_inventoryShowing;
        }

        inventory.ChangeVisibility(_inventoryShowing);
    }

    private void HotBarUpdate()
    {
        for (int i = 49; i < 57; i++)
        {
            if (Event.current.Equals(Event.KeyboardEvent(i.ToString())))
            {
                inventory.activeHotbarSlot = i - 49;
            }
        }
    }

    private void MouseUpdate()
    {
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool triggerLmb = Input.GetMouseButtonDown(0);
        bool triggerRmb = Input.GetMouseButtonDown(1);
        bool triggerUpLmb = Input.GetMouseButtonUp(0);
        removingPrimaryBlock = triggerLmb;
        var item = inventory.GetActiveItem();

        if (triggerUpLmb)
        {
            CheckActiveItemDown();
            return;
        }
        
        if (triggerLmb && item && item.isWeapon)
        {
            Attack();
            return;
        }

        if (!triggerLmb && !triggerRmb) return;
        blockAction = true;
        Notify();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("TileDrop")) return;
        if (inventory.TryAdd(collision.gameObject.GetComponent<TileDrop>().item))
            Destroy(collision.gameObject);
    }

    private void FixedUpdate()
    {
        blockAction = false;
        Notify();
        CheckCollisions();
        CheckDirection();
        RegenerateUpdate();
        _invulnerability = Mathf.Max(_invulnerability - 1, 0);
        _regeneration = Mathf.Max(_regeneration - 1, 0);
        _attackCounter = Mathf.Max(_attackCounter - 1, 0);
    }

    private void Update()
    {
        MoveUpdate();
        if (!World.CanPlay())
            return;

        MouseUpdate();
        InventoryUpdate();
    }

    private void OnGUI()
    {
        HotBarUpdate();
        hpController.UpdateUI(health);
    }

    public void Attach(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void Detach(IObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify()
    {
        foreach (var observer in _observers)
        {
            observer.ObserverUpdate(this);
        }
    }
}