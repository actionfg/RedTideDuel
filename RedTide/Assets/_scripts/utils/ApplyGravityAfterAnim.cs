using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyGravityAfterAnim : MonoBehaviour
{
    private float ySpeed;
    private bool onGround = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        ySpeed += Physics.gravity.y * Time.fixedDeltaTime;
    }

    /** TODO 需注意使用场合
     ** 如本身是个跳跃动画, 引入重力计算后, 会导致跳不起来现象
    */
    void OnAnimatorMove()
    {
        Animator animator = GetComponent<Animator>();

        if (animator)
        {
            if (onGround)
            {
                transform.position = animator.rootPosition;
            }
            else
            {
                Vector3 newPosition = animator.rootPosition;
                newPosition.y += ySpeed * Time.deltaTime;
                transform.position = newPosition;
            }
        }
    }
}
