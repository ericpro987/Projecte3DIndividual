using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class Player : MonoBehaviour
{
    [SerializeField] private float _LookVelocity = 100;
    private Vector2 _LookRotation = Vector2.zero;

    public InputSystem_Actions action;
    CharacterController cc;
    InputAction move;
    InputAction look;
    InputAction run;
    InputAction shoot1;
    InputAction shoot2;
    InputAction jump;
    InputAction robot;
    [SerializeField]
    LayerMask Shoot1Mask;
    [SerializeField]
    LayerMask Shoot2Mask;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    RobotBomb robotBomb;
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

        jump = action.Player.Jump;

        robot = action.Player.RobotBomb;

        action.Player.Enable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        jump.started += Jump;
        look.performed += Look;
        run.started += Sprint;
        run.performed += Sprint;
        run.canceled += CancelSprint;
        shoot1.started += Shoot1;
        shoot2.started += Shoot2;
        shoot2.canceled += ExitShoot2;
        robot.started += ActivateRobot;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Esta tocando suelo? {cc.isGrounded}");
        if (!cc.isGrounded)
        {
            velY -= gravity * 1f * Time.deltaTime;
        }
        if (!wait)
            Movement();


    }
    public void Look(InputAction.CallbackContext context)
    {
        Vector2 lookInput = look.ReadValue<Vector2>();
        _LookRotation.y += lookInput.y * _LookVelocity * Time.deltaTime;
        _LookRotation.y = Mathf.Clamp(_LookRotation.y, -60, 60);
        transform.Rotate(0, lookInput.x, 0);
        cam.transform.localRotation = Quaternion.Euler(-_LookRotation.y, 0, 0);
    }
    public void Movement()
    {


        Vector2 movimentInput = move.ReadValue<Vector2>();

        Vector3 moviment2 = (transform.right * movimentInput.x + transform.forward * movimentInput.y).normalized;
        moviment2.y = velY;
        moviment2 = moviment2 * spd;
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
            Debug.DrawLine(cam.transform.position, hit.point, Color.red, 4f);

            if (hit.transform.TryGetComponent<EnemyScript>(out EnemyScript e))
             {
                 e.ReceiveDamage(2);
             }
        }
    }
   /* public void ReceiveDamage(int dmg)
    {

    }*/
    float dist;
    Vector3 hitPoint;
    void Shoot2(InputAction.CallbackContext context)
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 20f, Shoot2Mask))
        {
             dist = Vector3.Distance(hit.point, this.transform.position);
            Debug.DrawLine(cam.transform.position, hit.point, Color.red, 4f);
            wait = true;
            Vector3 dir = hit.point - this.transform.position;
            hitPoint = hit.point;
            dir = dir.normalized * 7;
            StartCoroutine(HookMove(dir));

        }
    }
    IEnumerator HookMove(Vector3 dir)
    {
        while(wait &&  Mathf.Abs(Vector3.Distance(hitPoint, this.transform.position))>1f)
        {
            cc.Move(dir*Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
    }
    void Jump(InputAction.CallbackContext context)
    {
        /*if (cc.isGrounded)
        {
            float u = 5 / this.GetComponent<Rigidbody>().mass;
            float t = 2 * u / Physics.gravity.magnitude;
            Vector3 AB = (cam.transform.forward + cam.transform.forward * 0) - cam.transform.forward;
            Vector3 h = AB / t;
            Vector3 H = h * this.GetComponent<Rigidbody>().mass;
            Vector3 F = H + 0 * Vector3.up;
            F.Normalize();
            cc.velocity.Set(F.x, F.y, F.z);
            cc.Move(cc.velocity*Time.deltaTime);

        }*/
        if (cc.isGrounded)
        {
            Debug.Log("Estamos en el suelo.");

            velY = 5f;
        }
    }
    void ActivateRobot(InputAction.CallbackContext context)
    {
        robotBomb.transform.position = this.transform.position + this.transform.forward * 2;
        cam.gameObject.SetActive(false);
        robotBomb.gameObject.SetActive(true);
        action.Player.Disable();
    }
    void ExitShoot2Func()
    {
        wait = false;
        StopAllCoroutines();
    }
    void ExitShoot2(InputAction.CallbackContext context)
    {
        ExitShoot2Func();
    }
}

