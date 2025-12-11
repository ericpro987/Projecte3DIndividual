using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    [SerializeField]
    NavMeshAgent agent;
    int hp;
    int dmg;
    [SerializeField]
    LayerMask PlayerMask;
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {        
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        RandomDestination();
        this.hp = 10;
        dmg = 2;

    }
    public void ReceiveDamage(int dmg)
    {
        hp -= dmg;
        Debug.Log("Ay");
        if(hp <= 0)
        {
            Debug.Log("ME MUEROOOOOOOOO!!! AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            this.gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, agent.destination) <= 1)
        {
            RandomDestination();
        }
    }

    void RandomDestination()
    {        
        agent.SetDestination(new Vector3(UnityEngine.Random.Range(-35,40), 0, UnityEngine.Random.Range(-90, 90)));
    }
    public void Perseguir()
    {
        Collider[] cList = Physics.OverlapSphere(this.transform.position, 44f, PlayerMask);
        if (cList.Length > 0)
        {
            GameObject g =  cList[0].gameObject;
            rb.linearVelocity = this.transform.position - g.transform.position;            
        }
        
    }
    public void Atacar()
    {
        Collider[] cList = Physics.OverlapSphere(this.transform.position, 11f, PlayerMask);
        if (cList.Length > 0)
        {
            GameObject g =  cList[0].gameObject;
            if(g.TryGetComponent<Player>(out Player p))
            {
               // p.ReceiveDamage(dmg);
            }           
        }
        
    }
}
