using TMPro;
using UnityEngine;

public class TestDebug : MonoBehaviour
{


    [SerializeField] private TextMeshProUGUI txtLog;

    public static TestDebug Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void AddLog(string log)
    {
               txtLog.text += log + "\n";
    }

    // Update is called once per frame
    void Update()
    {


    }
}
