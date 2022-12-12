using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiNonTrigger : MonoBehaviour
{
    public GameObject RockUp;
    public GameObject RockDown;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "AICar")
        {
            RockDown.SetActive(false);
            RockUp.SetActive(true);
        }
    }
}
