using UnityEngine;

public class Water : MonoBehaviour
{
    [HideInInspector] public float[] accelerations;

    //The properties of our water
    private float baseheight;
    private float bottom;
    private GameObject collider;
    public float damping = 0.04f;
    private float left;
    [HideInInspector] public Mesh mesh;

    private GameObject meshobject;
    [HideInInspector] public Water nextBlock;

    [HideInInspector] public Water prevBlock;

    //Our particle system
    public GameObject splash;
    public float spread = 0.05f;

    //All our constants
    public float spring = 0.02f;
    [HideInInspector] public float[] velocities;

    private Vector3[] vertices;

    //The GameObject we're using for a mesh
    public GameObject watermesh;
    public float wind = 1f;
    private float[] xpositions;
    [HideInInspector] public float[] ypositions;
    public float z = 1f;

    public void Splash(float xpos, Vector2 velocity)
    {
        //If the position is within the bounds of the water:
        if (xpos >= xpositions[0] && xpos <= xpositions[xpositions.Length - 1])
        {
            //Offset the x position to be the distance from the left side
            xpos -= xpositions[0];

            //Find which spring we're touching
            var index = Mathf.RoundToInt((xpositions.Length - 1) *
                                         (xpos / (xpositions[xpositions.Length - 1] - xpositions[0])));

            //Add the velocity of the falling object to the spring
            velocities[index] += velocity.y;

            //Set the correct position of the particle system.
            var position = new Vector3(xpositions[index], ypositions[index] - 0.35f, 10);

            //This line aims the splash towards the middle. Only use for small bodies of water:
            if (velocity.y > 0) velocity = -velocity;
            var rotation = Quaternion.LookRotation(-velocity);

            //Create the splash and tell it to destroy itself.

            var newSplash = Instantiate(splash, position, rotation);
            var system = newSplash.GetComponent<ParticleSystem>().main;
            var speed = velocity.magnitude;
            system.startSpeed = new ParticleSystem.MinMaxCurve(4 + speed * 12, 5 + speed * 12);
            system.startLifetime = new ParticleSystem.MinMaxCurve(0.3f + speed, 0.7f + speed);
        }
    }

    public void SpawnWater(float Left, float Width, float Top, float Bottom)
    {
        //Bonus exercise: Add a box collider to the water that will allow things to float in it.
        // gameObject.AddComponent<BoxCollider2D>();
        // gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(Left + Width / 2, (Top + Bottom) / 2);
        // gameObject.GetComponent<BoxCollider2D>().size = new Vector2(Width, Top - Bottom);
        // gameObject.GetComponent<BoxCollider2D>().isTrigger = true;


        //Calculating the number of edges and nodes we have
        var edgecount = Mathf.RoundToInt(Width) * 5;
        var nodecount = edgecount + 1;

        //Declare our physics arrays
        xpositions = new float[nodecount];
        ypositions = new float[nodecount];
        velocities = new float[nodecount];
        accelerations = new float[nodecount];
        mesh = new Mesh();

        //Set our variables
        baseheight = Top;
        bottom = Bottom;
        left = Left;

        //For each node, set the line renderer and our physics arrays
        for (var i = 0; i < nodecount; i++)
        {
            ypositions[i] = Top;
            xpositions[i] = Left + Width * i / edgecount;
            accelerations[i] = 0;
            velocities[i] = 0;
        }

        //Setting the meshes now:
        vertices = new Vector3[nodecount * 2];

        for (var i = 0; i < nodecount; ++i) vertices[i] = new Vector3(xpositions[nodecount - i - 1], bottom, z);

        for (var i = 0; i < nodecount; i++) vertices[i + nodecount] = new Vector3(xpositions[i], ypositions[i], z);

        var tris = new int[edgecount * 6];
        for (var i = 0; i < edgecount; i++)
        {
            tris[6 * i] = nodecount + i;
            tris[6 * i + 1] = nodecount + i + 1;
            tris[6 * i + 2] = nodecount - i - 2;
            tris[6 * i + 3] = nodecount - i - 2;
            tris[6 * i + 4] = nodecount - i - 1;
            tris[6 * i + 5] = nodecount + i;
        }

        mesh.vertices = vertices;
        mesh.triangles = tris;

        var uv = new Vector2[vertices.Length];
        for (var i = 0; i < vertices.Length; ++i) uv[i] = Vector2.zero;

        mesh.uv = uv;

        meshobject = Instantiate(watermesh, Vector3.zero, Quaternion.identity);
        meshobject.GetComponent<MeshFilter>().mesh = mesh;
        meshobject.transform.parent = transform;

        collider = new GameObject();
        collider.name = "Water";
        collider.AddComponent<BoxCollider2D>();
        collider.transform.parent = transform;

        collider.transform.position = new Vector3(Left + Width * 0.5f, Top, 0);
        collider.transform.localScale = new Vector3(Width, 0.2f, 1);
        collider.GetComponent<BoxCollider2D>().isTrigger = true;
        collider.AddComponent<WaterDetector>();
    }

    //Same as the code from in the meshes before, set the new mesh positions
    private void UpdateMesh()
    {
        var nodecount = xpositions.Length;
        for (var i = 0; i < nodecount; i++) vertices[i + nodecount].y = ypositions[i];

        mesh.vertices = vertices;
    }

    private void OverrideParams()
    {
        if (prevBlock)
        {
            float size = ypositions.Length - 1;
            ypositions[0] = prevBlock.ypositions[ypositions.Length - 1];
            accelerations[0] = prevBlock.accelerations[ypositions.Length - 1];
            velocities[0] = prevBlock.velocities[ypositions.Length - 1];
        }
    }

    private void FixedUpdate()
    {
        OverrideParams();

        for (var i = 0; i < xpositions.Length; i++)
        {
            var force = spring * (ypositions[i] - baseheight) + velocities[i] * damping;
            accelerations[i] = -force;
            ypositions[i] += velocities[i];
            velocities[i] += accelerations[i];
            velocities[i] += wind * (Mathf.Cos(xpositions[i]) * Random.Range(0f, 1f));
        }

        OverrideParams();

        var leftDeltas = new float[xpositions.Length];
        var rightDeltas = new float[xpositions.Length];

        for (var j = 0; j < 8; j++)
        {
            for (var i = 0; i < xpositions.Length; i++)
            {
                if (i > 0)
                {
                    leftDeltas[i] = spread * (ypositions[i] - ypositions[i - 1]);
                    velocities[i - 1] += leftDeltas[i];
                }

                if (i < xpositions.Length - 1)
                {
                    rightDeltas[i] = spread * (ypositions[i] - ypositions[i + 1]);
                    velocities[i + 1] += rightDeltas[i];
                }
            }

            for (var i = 0; i < xpositions.Length; i++)
            {
                if (i > 0)
                    ypositions[i - 1] += leftDeltas[i];
                if (i < xpositions.Length - 1)
                    ypositions[i + 1] += rightDeltas[i];
            }
        }

        UpdateMesh();
    }
}