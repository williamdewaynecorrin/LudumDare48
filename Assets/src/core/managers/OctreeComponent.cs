using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctreeComponent : MonoBehaviour
{
    [SerializeField]
    private float size = 1f;
    [SerializeField]
    private int depth = 3;

    COctree<int> octree;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        octree = new COctree<int>(this.transform.position, size, depth);
        Color startcolor = Color.green;
        DrawNode(octree.Root(), startcolor);
    }

    private void DrawNode(COctree<int>.COctreeNode<int> node, Color color)
    {
        if (!node.Leaf())
        {
            foreach (COctree<int>.COctreeNode<int> n in node.Nodes())
            {
                DrawNode(n, new Color(color.r, color.g, color.b, color.a * 0.5f));
            }
        }

        Gizmos.color = color;
        Gizmos.DrawWireCube(node.center, Vector3.one * node.size);
    }
}
