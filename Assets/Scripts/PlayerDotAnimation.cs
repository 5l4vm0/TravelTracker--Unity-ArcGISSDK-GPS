using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using UnityEngine.Playables;

public class PlayerDotAnimation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer PlayerImage;
    private float oldXPosition;
    [SerializeField] private PlayableDirector timeline;
    [SerializeField] private PlayableAsset WalkingLeft;
    [SerializeField] private PlayableAsset WalkingRight;
    [SerializeField] private PlayableAsset Idle;
    Stopwatch timer;

    // Start is called before the first frame update
    void Start()
    {
        oldXPosition = transform.position.x;
        timer = new Stopwatch();
        timer.Start();
    }

    private void Update()
    {
        if(timer.Elapsed.TotalSeconds >=3)
        {
            if ((transform.position.x - oldXPosition) >= 1) //moving to the right
            {
                timeline.Stop();
                oldXPosition = transform.position.x;
                timeline.Play(WalkingRight,DirectorWrapMode.Loop);
                PlayerImage.flipX = false;
            }
            else if ((oldXPosition - transform.position.x) >= 1) //moving to the left
            {
                timeline.Stop();
                oldXPosition = transform.position.x;
                timeline.Play(WalkingLeft, DirectorWrapMode.Loop);
                PlayerImage.flipX = true;
            }
            else  //not moving to the left or right 
            {
                oldXPosition = transform.position.x;
                StopAnimation();
            }
            timer.Restart();
        }
    }

    private void StopAnimation()
    {
        timeline.Stop();
        timeline.Play(Idle, DirectorWrapMode.Loop);
    }
}