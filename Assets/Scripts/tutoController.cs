using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutoController : MonoBehaviour
{
    [SerializeField] private GameObject UIObject;
    [SerializeField] private GameObject UIObjectWater;
    [SerializeField] private GameObject UIObjectMud;
    [SerializeField] private GameObject UIObjectFinish;
    [SerializeField] private GameObject UIObjectFire;

    // Start is called before the first frame update
    void Start()
    {
        UIObject.SetActive(false);
        UIObjectWater.SetActive(false);
        UIObjectMud.SetActive(false);
        UIObjectFinish.SetActive(false);
        UIObjectFire.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Tuto"))
        {
            UIObject.SetActive(true);
        }

        if (collision.gameObject.CompareTag("WaterZone"))
        {
            UIObjectWater.SetActive(true);
        }
        if (collision.gameObject.CompareTag("MudZone")) 
        {
            UIObjectMud.SetActive(true);
        }
        if (collision.gameObject.CompareTag("FinishZone"))
        {
            UIObjectFinish.SetActive(true);
        }
        if (collision.gameObject.CompareTag("FireZone"))
        {
            UIObjectFire.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Tuto"))
        {
            UIObject.SetActive(false);
        }
        if (collision.gameObject.CompareTag("WaterZone"))
        {
            UIObjectWater.SetActive(false);
        }
        if (collision.gameObject.CompareTag("MudZone"))
        {
            UIObjectMud.SetActive(false);
        }
        if (collision.gameObject.CompareTag("FinishZone"))
        {
            UIObjectFinish.SetActive(false);
        }
        if (collision.gameObject.CompareTag("FireZone"))
        {
            UIObjectFire.SetActive(false);
        }
    }
}
