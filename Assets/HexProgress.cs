using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexProgress : MonoBehaviour
{
    public BuildJob job;

    Image image;

    UiGlobals ui;

    void Start()
    {
        image = GetComponent<Image>();
        ui = UiGlobals.Get();
    }

    void Update()
    {
        if (job.done)
        {
            Destroy(gameObject);
            return;
        }

        image.fillAmount = job.progress;

        Vector3 job_pos_world = ui.GetGrid().CellToWorld(job.pos);
        image.GetComponent<RectTransform>().anchoredPosition = ui.WorldToCanvas(job_pos_world);
    }
}
