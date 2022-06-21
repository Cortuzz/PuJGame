using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Character, IObservable
{
    public float speed;
    public float jumpForce;

    public float jumpTicks;
    public float jumpSpeed;
    public float maxJumpSpeed;
    public float holdJumpTime;
    public bool holdingJump = false;
    public bool blockAction = false;
    public bool removingPrimaryBlock = true;

    private bool _inventoryShowing = false;
    private Vector2 _mousePosition;
    public Inventory inventory;

    private List<IObserver> _observers = new List<IObserver>();

    protected override void Awake()
    {
        inventory = GetComponent<Inventory>();
        base.Awake();
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
        bool collision = _collider.CheckBottomCollision();
        if (collision)
            TakeDamage((int)(jumpTicks / 5 - 40)); // TODO: ѕроблема в более быстром / медленном падении
        SetOnGround(collision);

        _collider.CheckTopCollision();
        _collider.CheckSideCollision();
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

    private void HotbarUpdate()
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
        bool triggerLMB = Input.GetMouseButtonDown(0);
        bool triggerRMB = Input.GetMouseButtonDown(1);
        removingPrimaryBlock = triggerLMB;

        if (triggerLMB || triggerRMB)
        {
            blockAction = true;
            Notify();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TileDrop"))
        {
            if (inventory.TryAdd(collision.gameObject.GetComponent<TileDrop>().item))
                Destroy(collision.gameObject);
        }
    }

    private void FixedUpdate()
    {
        blockAction = false;
        Notify();
        CheckCollisions();
        CheckDirection();
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
        HotbarUpdate();
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
