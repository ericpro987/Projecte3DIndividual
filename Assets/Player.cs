using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class Player : MonoBehaviour
{
    [SerializeField] private float _LookVelocity = 100;
    private Vector2 _LookRotation = Vector2.zero;

    InputSystem_Actions action;
    CharacterController cc;
    InputAction move;
    InputAction look;
    InputAction run;
    InputAction shoot1;
    InputAction shoot2;
    [SerializeField]
    LayerMask Shoot1Mask;
    [SerializeField]
    LayerMask Shoot2Mask;
    [SerializeField]
    private Camera cam;
    float gravity = 9.8f;
    float velY;
    bool wait = false;

    int spd = 5;

    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        action = new InputSystem_Actions();
        cc = GetComponent<CharacterController>();

        move = action.Player.Move;

        look = action.Player.Look;

        run = action.Player.Sprint;

        shoot1 = action.Player.Shoot1;

        shoot2 = action.Player.Shoot2;

        action.Enable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        look.performed += Look;
        run.started += Sprint;
        run.performed += Sprint;
        run.canceled += CancelSprint;
        shoot1.started += Shoot1;
        shoot2.started += Shoot2;
        shoot2.canceled += ExitShoot2;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Esta tocando suelo? {cc.isGrounded}");
        if (!cc.isGrounded)
        {
            velY -= gravity * Time.deltaTime;
        }
        if (!wait)
            Movement();


    }
    public void Look(InputAction.CallbackContext context)
    {
        Vector2 lookInput = look.ReadValue<Vector2>();
        _LookRotation.y += lookInput.y * _LookVelocity * Time.deltaTime;
        _LookRotation.y = Mathf.Clamp(_LookRotation.y, -60, 60);
        transform.Rotate(0, look.ReadValue<Vector2>().x, 0);
        cam.transform.localRotation = Quaternion.Euler(-_LookRotation.y, 0, 0);
    }
    public void Movement()
    {


        Vector2 movimentInput = move.ReadValue<Vector2>();

        Vector3 moviment2 = transform.right * movimentInput.x + transform.forward * movimentInput.y;
        moviment2.y = velY;
        moviment2 = moviment2.normalized * spd;
        this.cc.Move(moviment2 * Time.deltaTime);
    }

    void Sprint(InputAction.CallbackContext context)
    {
        spd = 10;
    }
    void CancelSprint(InputAction.CallbackContext context)
    {
        spd = 5;
    }

    void Shoot1(InputAction.CallbackContext context)
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 20f, Shoot1Mask))
        {

        }
    }
    void Shoot2(InputAction.CallbackContext context)
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 20f, Shoot2Mask))
        {
            Debug.DrawLine(cam.transform.position, hit.point, Color.red, 4f);
            wait = true;
            Vector3 dir = hit.point - this.transform.position;
            dir = dir.normalized * 7;
            StartCoroutine(HookMove(dir));
        }
    }
    IEnumerator HookMove(Vector3 dir)
    {
        while(wait && this.transform.position.x != dir.x && transform.position.z!=dir.z)
        {
            cc.Move(dir*Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
    }
    void ExitShoot2(InputAction.CallbackContext context)
    {
        wait = false;
        StopAllCoroutines();
    }
}

