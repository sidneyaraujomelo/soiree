using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartInvestigate : MonoBehaviour
{
    public GameObject button;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnableButton());
    }

    IEnumerator EnableButton()
    {
        yield return new WaitForSeconds(5f);
        button.SetActive(true);
    }
}
