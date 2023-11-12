using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiGlobals : MonoBehaviour
{
    public GameObject hex_progress;

    Grid grid;

    Canvas canvas;

    public static UiGlobals Get()
    {
        return GameObject.Find("/Canvas").GetComponent<UiGlobals>();
    }

    void Start()
    {
        grid = GameObject.Find("/Hive").GetComponent<Grid>();
        canvas = GameObject.Find("/Canvas").GetComponent<Canvas>();
    }


    public Grid GetGrid()
    {
        return grid;
    }

    public Vector2 WorldToCanvas(Vector3 world)
    {
        Vector2 screen_pos = Camera.main.WorldToScreenPoint(world);
        screen_pos.x -= Camera.main.pixelWidth * 0.5f;
        screen_pos.y -= Camera.main.pixelHeight * 0.5f;

        return screen_pos;
    }

    public void SpawnProgressIndicator(BuildJob job)
    {
        HexProgress progress = Instantiate(hex_progress, transform).GetComponent<HexProgress>();
        progress.job = job;
    }
}
