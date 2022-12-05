using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceTrigger : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))

        {

            SceneManager.LoadScene("Race");
        }
    }
}
        
    

