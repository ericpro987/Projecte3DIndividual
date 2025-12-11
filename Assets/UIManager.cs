using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI timerBombText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActiveTimer()
    {
        timerBombText.gameObject.SetActive(true);
    }
    public void DesactiveTimer()
    {
        timerBombText.gameObject.SetActive(false);
    }
    public void ChangeTextTimer(int time)
    {
        this.timerBombText.text = time.ToString();
    }
}
