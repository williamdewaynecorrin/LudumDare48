using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSystem : MonoBehaviour
{
    [SerializeField]
    private Rigidbody ropebase;
    [SerializeField]
    private List<RopeSystemNode> nodes;

    private Vector3 systemtop;
    private Vector3 systembot;

    void Awake()
    {
        for (int i = 0; i < nodes.Count; ++i)
        {
            if (i == 0)
                nodes[i].SetupNode(i, null, nodes[i + 1]);
            else if (i == nodes.Count - 1)
                nodes[i].SetupNode(i, nodes[i - 1], null);
            else
                nodes[i].SetupNode(i, nodes[i - 1], nodes[i + 1]);
        }
    }

    public void AddForceAtPosition(Vector3 pos, Vector3 force)
    {
        nodes[0].GetComponent<Rigidbody>().AddForce(force);
    }

    public Vector3 GetPosition(float ratio)
    {
        int numnodes = nodes.Count;
        float div = 1f / (float)numnodes;

        int botnodeidx = Mathf.Clamp(Mathf.FloorToInt(ratio * numnodes), 0, numnodes - 1);
        Debug.LogWarning("System rope index:" + botnodeidx);
        Vector3 curpos = nodes[botnodeidx].GetPosition();
        Vector3 uppos = nodes[botnodeidx].UpLinkPos();

        float l1 = div * botnodeidx;
        float l2 = div * (botnodeidx + 1);
        float lerp = Mathf.InverseLerp(l1, l2, ratio);

        return Vector3.Lerp(uppos, curpos, lerp);
    }

    public float GetValueFromNode(RopeSystemNode node)
    {
        float div = 1f / (float)nodes.Count;
        int nodeidx = node.NodeIndex();

        return div * nodeidx;
    }

    public bool AnyNodeTouchingPlayer()
    {
        for(int i = 0; i < nodes.Count; ++i)
        {
            if (nodes[i].TouchingPlayer())
                return true;
        }

        return false;
    }
}
