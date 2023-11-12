using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildJob
{
    public Vector3Int pos;
    public CellType type;
    public float progress;
    public bool done;
}

struct WorkerGroup
{
    public int worker_count;
    public BuildJob job;
}

public class HiveEditor : MonoBehaviour
{
    Grid grid;

    [SerializeField]
    GameObject base_cell;

    [SerializeField]
    GameObject resevoir_cell;

    [SerializeField]
    GameObject hatchery_cell;

    UiGlobals ui;

    Dictionary<CellType, GameObject> cell_prefabs = new Dictionary<CellType, GameObject>();

    Dictionary<Vector3Int, Cell> cells = new Dictionary<Vector3Int, Cell>();

    List<BuildJob> build_jobs = new List<BuildJob>();

    List<WorkerGroup> workers = new List<WorkerGroup>();

    double next_tick = 0.0;

    [SerializeField]
    double tick_interval = 0.2f;

    [SerializeField]
    int idle_workers = 10;

    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponent<Grid>();
        ui = UiGlobals.Get();

        cell_prefabs[CellType.Basic] = base_cell;
        cell_prefabs[CellType.Resevoir] = resevoir_cell;
        cell_prefabs[CellType.Hatchery] = hatchery_cell;

        Cell new_cell = AddCell(new Vector3Int (0, 0), CellType.Resevoir);
        Resevoir resevoir = new_cell.GetComponent<Resevoir>();
        resevoir.resource_type = ResourceType.Honey;
        resevoir.resource_amount = 100.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse_pos_world = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward);

            // Project onto z=0
            Vector3 camera_delta = mouse_pos_world - Camera.main.transform.position;
            mouse_pos_world += -(mouse_pos_world.z / camera_delta.z) * camera_delta;

            Vector3Int cell_pos = grid.WorldToCell(mouse_pos_world);
            CreateBuildJob(cell_pos);
        }

        if (Time.timeAsDouble > next_tick)
        {
            Tick();
            next_tick = Time.timeAsDouble + tick_interval;
        }
    }

    void Tick()
    {
        UpdateWorkers();
    }

    void UpdateWorkers()
    {
        for (int i = 0; i < workers.Count; ++i)
        {
            BuildJob job = workers[i].job;
            job.progress += 0.1f * workers[i].worker_count;

            if (job.progress >= 1.0f)
            {
                // TODO: don't set done until we know tile has placed
                job.done = true;
                build_jobs.Remove(job);

                idle_workers += workers[i].worker_count;
                workers.RemoveAt(i);
                i -= 1;
                
                AddCell(job.pos, job.type);
            }
        }
    }

    void CreateBuildJob(Vector3Int cell_pos, CellType cell_type = CellType.Basic)
    {
        if (!CanQueueJob(cell_pos))
        {
            return;
        }

        BuildJob job = new BuildJob { pos = cell_pos, type = cell_type, progress = 0.0f, done = false };
        build_jobs.Add(job);
        if (idle_workers > 0)
        {
            idle_workers -= 1;
            workers.Add(new WorkerGroup { job = job, worker_count = 1 });
        }

        ui.SpawnProgressIndicator(job);
    }

    Cell AddCell(Vector3Int cell_pos, CellType cell_type = CellType.Basic)
    {
        if (!CanPlaceCell(cell_pos))
        {
            return null;
        }

        Vector3 cell_pos_world = grid.CellToWorld(cell_pos);
        GameObject new_cell_obj = Instantiate(cell_prefabs[cell_type], cell_pos_world, Quaternion.identity);

        Cell new_cell = new_cell_obj.GetComponent<Cell>();
        cells[cell_pos] = new_cell;
        return new_cell;
    }

    bool CanPlaceCell(Vector3Int pos)
    {
        return pos.z == 0 && !cells.ContainsKey(pos);
    }

    bool CanQueueJob(Vector3Int pos)
    {
        if (!CanPlaceCell(pos))
        {
            return false;
        }

        if (cells.Count == 0)
        {
            return true;
        }

        Vector3Int[] adjacent;
        if (pos.y % 2 == 0)
        {
            adjacent = new Vector3Int[]
            {
                pos + Vector3Int.right, pos + Vector3Int.left, pos + Vector3Int.up, pos + Vector3Int.up + Vector3Int.left, pos + Vector3Int.down, pos + Vector3Int.down + Vector3Int.left
            };
        }
        else
        {
            adjacent = new Vector3Int[]
            {
                pos + Vector3Int.right, pos + Vector3Int.left, pos + Vector3Int.up, pos + Vector3Int.up + Vector3Int.right, pos + Vector3Int.down, pos + Vector3Int.down + Vector3Int.right
            };
        }

        foreach (Vector3Int adjacent_pos in adjacent)
        {
            if (cells.ContainsKey(adjacent_pos))
            {
                return true;
            }
        }

        return false;
    }
}
