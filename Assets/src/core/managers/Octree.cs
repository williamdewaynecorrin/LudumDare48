using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COctree<T>
{
    private COctreeNode<T> root;
    private int depth;

    public COctree(Vector3 center, float size, int depth)
    {
        this.depth = depth;
        root = new COctreeNode<T>(center, size, depth);
        root.SliceTree(depth);
    }

    public class COctreeNode<T>
    {
        public Vector3 center;
        public float size;
        public int depth;

        private COctreeNode<T>[] nodes;
        private List<T> data;

        public COctreeNode(Vector3 center, float size, int depth)
        {
            this.center = center;
            this.size = size;
            this.depth = depth;
        }

        public void SliceTree(int curdepth)
        {
            nodes = new COctreeNode<T>[8];
            for (int i = 0; i < nodes.Length; ++i)
            {
                // -- this cast is redundant, but helps to understand what is happening
                EOctreeIndex index = (EOctreeIndex)i;
                Vector3 offsetdir = slicedirections[(int)index];
                Vector3 offset = offsetdir * size * 0.25f;

                Vector3 childcenter = center + offset;
                nodes[i] = new COctreeNode<T>(childcenter, size / 2f, curdepth);

                if (curdepth > 0)
                  nodes[i].SliceTree(curdepth - 1);
            }
        }

        public bool Leaf()
        {
            return nodes == null;
        }

        public COctreeNode<T>[] Nodes()
        {
            return nodes;
        }
    }

    private enum EOctreeIndex
    {
        TOP_LEFT_FRONT = 0,     //000,
        TOP_LEFT_BACK = 1,      //001,
        TOP_RIGHT_FRONT = 2,    //010,
        TOP_RIGHT_BACK = 3,     //011,
        BOT_LEFT_FRONT = 4,     //100,
        BOT_LEFT_BACK = 5,      //101,
        BOT_RIGHT_FRONT = 6,    //110,
        BOT_RIGHT_BACK = 7      //111,
    }

    private static Vector3[] slicedirections =
    {
        new Vector3(-1, 1, 1),
        new Vector3(-1, 1, -1),
        new Vector3(1, 1, 1),
        new Vector3(1, 1, -1),
        new Vector3(-1, -1, 1),
        new Vector3(-1, -1, -1),
        new Vector3(1, -1, 1),
        new Vector3(1, -1, -1)
    };

    private int GetIndex(Vector3 objectposition, Vector3 nodeposition)
    {
        int index = 0;

        int topbot = objectposition.y > nodeposition.y ? 0 : 4;
        int leftright = objectposition.x > nodeposition.x ? 2 : 0;
        int frontback = objectposition.z > nodeposition.z ? 0 : 1;


        return index;
    }

    public COctreeNode<T> Root()
    {
        return root;
    }
}