using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    Collider2D col;

    public List<Collider2D> detectedColliders = new List<Collider2D>();
    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
         detectedColliders.Add(collision);

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
         detectedColliders.Remove(collision);
    }
}
