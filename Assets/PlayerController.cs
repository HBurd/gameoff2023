using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float jump_speed = 20.0f;

    [SerializeField]
    float movement_speed = 10.0f;

    [SerializeField]
    float air_movement_speed = 10.0f;

    [SerializeField]
    float ground_force = 20.0f;

    [SerializeField]
    float air_force = 5.0f;

    [SerializeField]
    bool controlled = false;

    Vector3 current_force;
    Rigidbody rb;


    bool grounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public Vector3 GetForce()
    {
        return current_force;
    }

    void OnCollisionEnter(Collision collision)
    {
        current_force = collision.impulse / Time.fixedDeltaTime;
    }

    void OnCollisionStay(Collision collision)
    {
        current_force = collision.impulse / Time.fixedDeltaTime;
    }

    void OnCollisionExit(Collision collision)
    {
        current_force = Vector3.zero;//collision.impulse / Time.fixedDeltaTime;
    }

    void Update()
    {
        if (Input.GetButtonDown("Swap"))
        {
            controlled = !controlled;
        }
        if (!controlled)
        {
            return;
        }

        if (grounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity += Vector3.up * (jump_speed - rb.velocity.y);
        }
    }

    void FixedUpdate()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, 1.0f, LayerMask.GetMask("Platform"));

        if (!controlled)
        {
            return;
        }

        float target_speed = grounded ? movement_speed : air_movement_speed;
        float target_vel = target_speed * Input.GetAxis("Horizontal");
        float actual_vel = rb.velocity.x;
        float speed_delta = target_vel - actual_vel;

        if (target_vel * actual_vel > 0.0f && speed_delta * target_vel < 0.0f)
        {
            // Do nothing, don't slow down if player is already moving in target direction
        }
        else
        {
            float force_multiplier = grounded ? ground_force : air_force;
            rb.AddForce(speed_delta * force_multiplier * Vector3.right);
        }
    }
}
