using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    public static bool IsPlayerInvisible = false;

    private readonly Vector2 moveLeft = new Vector2(-2, 0);
    public List<List<Fish>> boids;

    private Vector2 direction;
    public int fishTypesCount = 5;
    public int maxBoidsVisible = 8;

    private MinesManager _minesManager;

    public Transform player;
    
    private Vector2 _pos;
    private Vector2 _newVelocity;
    private Vector2 _res;
    private Vector2 _sum;
    private Vector2 _sumPos;
    private Vector2 _sumVel;
    
    public float yMin = -10, yMax, viewDst;

    public void Awake()
    {
        boids = new List<List<Fish>>();
        for (var i = 0; i < fishTypesCount; i++) boids.Add(new List<Fish>());

        _minesManager = GameObject.Find("MinesManager").GetComponent<MinesManager>();
    }

    public void AddFish(Fish fish)
    {
        boids[fish.type].Add(fish);
    }

    private void FixedUpdate()
    {
        for (var i = 0; i < fishTypesCount; i++)
            ProcessBoids(i);
    }

    private void ProcessBoids(int type)
    {
        for (var i = 0; i < boids[type].Count; i++) ProcessBoid(type, i);
    }

    private void ProcessBoid(int type, int num)
    {
        if (boids[type][num].isDead) return;
        _newVelocity = boids[type][num].body.velocity;
        _newVelocity += Rule1(type, num);
        _newVelocity += Rule2(type, num);
        _newVelocity += Rule3(type, num);
        _newVelocity += Rule4(type, num);
        _newVelocity += AvoidPlayer(type, num);
        _newVelocity += AvoidMines(type, num);
        _newVelocity += MoveLeft();

        boids[type][num].UpdateVelocity(_newVelocity);
    }

    private Vector2 Rule1(int type, int num)
    {
        if (boids[type][num].isDead) return Vector2.zero;

        _sumPos = Vector2.zero;
        var count = 0;

        foreach (var boid in boids[type][num].neighbours)
        {
            if (boid.isDead) continue;
            if (boid != boids[type][num])
            {
                _sumPos += boid.body.position;
                count++;
                if (count >= maxBoidsVisible) break;
            }
        }

        if (count == 0) return Vector2.zero;

        return (_sumPos / count - boids[type][num].body.position) / 4f;
    }

    private Vector2 Rule2(int type, int num)
    {
        if (boids[type][num].isDead) return Vector2.zero;

        _sum = Vector2.zero;
        var count = 0;

        foreach (var boid in boids[type][num].neighbours)
        {
            if (boid.isDead) continue;
            if (boid != boids[type][num])
            {
                ++count;
                var sqrDst = (boids[type][num].body.position - boid.body.position).sqrMagnitude;
                if (sqrDst < 0.3f) _sum += boids[type][num].body.position - boid.body.position;

                if (count >= maxBoidsVisible) break;
            }
        }

        return _sum / 1.5f;
    }

    private Vector2 Rule3(int type, int num)
    {
        if (boids[type][num].isDead) return Vector2.zero;

        _sumVel = Vector2.zero;
        var count = 0;

        foreach (var boid in boids[type][num].neighbours)
        {
            if (boid.isDead) continue;
            if (boid != boids[type][num])
            {
                _sumVel += boid.body.velocity;
                count++;

                if (count >= maxBoidsVisible) break;
            }
        }

        if (count == 0) return Vector2.zero;

        _sumVel /= count;
        return (_sumVel - boids[type][num].body.velocity) / 4f;
    }

    private Vector2 Rule4(int type, int num)
    {
        if (boids[type][num].isDead) return Vector2.zero;
        _res = Vector2.zero;
        _pos = boids[type][num].body.position;
        if (_pos.y > yMax) _res.y = -3;
        if (_pos.y < yMin) _res.y = 3;

        return _res;
    }

    private Vector2 MoveLeft()
    {
        return moveLeft;
    }

    private Vector2 AvoidPlayer(int type, int num)
    {
        if (boids[type][num].isDead || IsPlayerInvisible) return Vector2.zero;
        _res = boids[type][num].body.position - (Vector2) player.position;
        if (_res.sqrMagnitude < 40f) return _res * (1f / _res.sqrMagnitude * 7f);
        return Vector2.zero;
    }

    private Vector2 AvoidMines(int type, int num)
    {
        if (boids[type][num].isDead) return Vector2.zero;
        _res = Vector2.zero;
        foreach (var mine in _minesManager.mines)
        {
            direction = boids[type][num].body.position - mine.GetCenter();
            if (direction.sqrMagnitude < 12f) _res += direction * (1f / direction.sqrMagnitude * 4f);
        }

        return _res;
    }
}