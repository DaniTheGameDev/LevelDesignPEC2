using System.Collections;
using UnityEngine;

public class Sokoban : MonoBehaviour
{

    [HideInInspector] public int targetCount = 0;
    private float zOffset = -1;
    private float step = 1;
    private float speed = 0.005f;
    private Transform player, moved;
    public static int target { get; set; }
    private Vector3 direction, targetPos;
    private bool isMove;
    private GameObject VictoryCanvas;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = 0;
        targetCount = GameObject.FindGameObjectsWithTag("Target").Length;
        if (player != null)
        {
            player.position = new Vector3(player.position.x, player.position.y, zOffset);
            targetPos = player.position;
        }
        VictoryCanvas = GameObject.Find("VictoryCanvas");
        VictoryCanvas.SetActive(false);
    }

    void Complete()
    {
        Debug.Log("! You Win !");
        VictoryCanvas.SetActive(true);
    }

    void Update()
    {
        if (player != null){Control();}
    }

    Transform GetTransform(Vector2 point)
    {
        RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);
        if (hit.collider != null){return hit.transform;}
        return null;
    }

    bool CanMove()
    {
        moved = null;
        
        Transform t1 = GetTransform(new Vector2(player.position.x + step * direction.x, player.position.y + step * direction.y));
        Transform t2 = GetTransform(new Vector2(player.position.x + step * direction.x * 2f, player.position.y + step * direction.y * 2f));
        
        if (t1 != null && t1.name.CompareTo("Box") == 0) moved = t1;
        if (t1 == null || t1.name.CompareTo("Wall") == 0 || moved != null && t2 != null && t2.name.CompareTo("Box") == 0 ||
            moved != null && t2 != null && t2.name.CompareTo("Wall") == 0) return false;

        isMove = true;
        if (moved != null) moved.position = new Vector3(moved.position.x, moved.position.y, zOffset);
        return true;
    }

    void Move()
    {
        if (!CanMove()) return;
        targetPos = new Vector3(player.position.x + step * direction.x, player.position.y + step * direction.y, player.position.z);
    }

    Vector3 GetRoundPos(Vector3 val)
    {
        val.x = Mathf.Round(val.x * 100f) / 100f;
        val.y = Mathf.Round(val.y * 100f) / 100f;
        val.z = Mathf.Round(val.z * 100f) / 100f;
        return val;
    }

    void Control()
    {
        if (isMove)
        {
            player.position = Vector3.MoveTowards(player.position, targetPos, speed);
            if (moved != null) moved.position = Vector3.MoveTowards(moved.position, targetPos + direction * step, speed);
            if (targetPos == GetRoundPos(player.position))
            {
                isMove = false;
                player.position = GetRoundPos(player.position);
                if (moved != null) moved.position = GetRoundPos(moved.position);
                if (target == targetCount)
                {
                    Complete();
                    enabled = false;
                }
            }
            return;
        }

        if (Input.GetKey(KeyCode.D)){direction = Vector3.right;}
        else if (Input.GetKey(KeyCode.A)){direction = Vector3.left;}
        else if (Input.GetKey(KeyCode.W)){direction = Vector3.up;}
        else if (Input.GetKey(KeyCode.S)){direction = Vector3.down;}
        else{direction = Vector3.zero;}

        if (direction.magnitude != 0)
        {
            Move();
        }
    }
}