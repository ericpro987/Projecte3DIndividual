using System.Collections;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class RobotBomb : MonoBehaviour
{
    InputSystem_Actions action;
    CharacterController cc;
    InputAction move;
    InputAction look;
    [SerializeField] private Camera cam;
    [SerializeField] private Player player;
    [SerializeField] private Camera camPlayer;
    [SerializeField] private float _LookVelocity = 100;
    [SerializeField] private UIManager uiManager;
    int timer;
    float gravity = 9.8f;

    [SerializeField]
    LayerMask enemyMask;
    float velY;
    private Vector2 _LookRotation = Vector2.zero;
    int spd = 14;

    private void Awake()
    {
        action = new InputSystem_Actions();

        cc = GetComponent<CharacterController>();
       
        move = action.Robot.Move;

        look = action.Robot.Cam;

        look.started += Look;


    }
    private void OnEnable()
    {
        timer = 20;
        uiManager.ActiveTimer();
        uiManager.ChangeTextTimer(timer);
        StartCoroutine(Explode());
        action.Robot.Enable();

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!cc.isGrounded)
        {
            velY -= gravity * 1f * Time.deltaTime;
        }
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
    IEnumerator Explode()
    {
        uiManager.ChangeTextTimer(timer);
        while (true)
        {
            yield return new WaitForSeconds(1f);
            timer--;
            uiManager.ChangeTextTimer(timer);
            if (timer <= 0)
            {
                uiManager.DesactiveTimer();
                Collider[] c = Physics.OverlapSphere(this.transform.position, 10f, enemyMask);
                if (c.ElementAt(0) != null)
                {
                    c.ElementAt(0).gameObject.SetActive(false);
                }
                this.gameObject.SetActive(false);
                break;
            }
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.tag == "Enemy")
        {
            StopAllCoroutines();
            uiManager.DesactiveTimer();
            collision.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
    } 
    private void OnDisable()
    {
        action.Robot.Disable();
        camPlayer.gameObject.SetActive(true);
        player.action.Player.Enable();
    }
}
