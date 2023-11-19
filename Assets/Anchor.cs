using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    float rope_length = 30.0f;

    bool taut = false;

    [SerializeField]
    float g = 9.81f;

    [SerializeField]
    Rigidbody player1;

    [SerializeField]
    Rigidbody player2;

    [SerializeField]
    Transform anchor2;

    void FixedUpdate()
    {
        float anchor_distance = (transform.position - anchor2.position).magnitude;
        Debug.Assert(anchor_distance <= rope_length);

        Vector3 offset1 = player1.position - transform.position;
        float length1 = offset1.magnitude;
        Vector3 d1 = offset1 / length1;

        Vector3 offset2 = player2.position - anchor2.position;
        float length2 = offset2.magnitude;
        Vector3 d2 = offset2 / length2;

        Vector3 f1 = player1.mass * g * Vector3.down;
        Vector3 f2 = player2.mass * g * Vector3.down;

        if (length1 + length2 + anchor_distance > rope_length)
        {
            if (!taut)
            {
                // Cancel velocity with impulse
                float impulse = (Vector3.Dot(player1.velocity, d1) + Vector3.Dot(player2.velocity, d2))
                    / (1.0f / player1.mass + 1.0f / player2.mass);
                if (impulse >= 0.0f)
                {
                    player1.AddForce(impulse * -d1, ForceMode.Impulse);
                    player2.AddForce(impulse * -d2, ForceMode.Impulse);
                }
            }
            else
            {
                float tension = Vector3.Dot(f1 + player1.GetComponent<PlayerController>().GetForce(), d1) / player1.mass + Vector3.Dot(f2 + player2.GetComponent<PlayerController>().GetForce(), d2) / player2.mass;
                tension /= 1.0f / player1.mass + 1.0f / player2.mass;

                float over_extent = length1 + length2 + anchor_distance - rope_length;
                float elastic_force = over_extent * 50.0f;

                tension += elastic_force;

                Vector3 t1 = tension * -d1;
                Vector3 t2 = tension * -d2;

                f1 += t1;
                f2 += t2;

                // Friction at anchor

                // Only need the sign of this. Positive means rope is moving towards p1
                float rope_speed = 0.5f * Vector3.Dot(player1.velocity, d1) - 0.5f * Vector3.Dot(player2.velocity, d2);
                float normal_force = Vector3.Dot(t1 + t2, Vector3.up);
                float rope_friction = Mathf.Sign(rope_speed) * normal_force * 0.04f;

                f1 += rope_friction * -d1;
                f2 += rope_friction * d2;
            }

            taut = true;
        }
        else
        {
            taut = false;
        }

        player1.AddForce(f1);
        player2.AddForce(f2);
    }


}
