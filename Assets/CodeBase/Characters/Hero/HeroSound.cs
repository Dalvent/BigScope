using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSound : MonoBehaviour
{
    public AudioSource JumpAudio;
    public HeroMove HeroMove;

    private void Awake()
    {
        HeroMove.Jump += JumpAudio.Play;
        HeroMove.AirJump += JumpAudio.Play;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
