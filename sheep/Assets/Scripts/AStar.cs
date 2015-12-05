using UnityEngine;
using System;
using System.Collections;

public class AStar : MonoBehaviour
{

    public static PriorityQueue closeList, openList;

    //计算两个点之间的估计值
    private static float HeursticEstimatedCost(Node curNode, Node goalNode)
    {
        Vector3 vectorCost = curNode.Position - goalNode.Position;
        return vectorCost.magnitude;
    }

    //查找路径主函数
    public static ArrayList FindPath(Node start, Node goal)
    {
        openList = new PriorityQueue();
        openList.Push(start);
        start.NodeTotalCost = 0.0f;//起点的初始耗费为0
        start.EstimatedCost = HeursticEstimatedCost(start, goal);//起点的估计耗费
        closeList = new PriorityQueue();
        Node curNode = null;
        while (openList.Length > 0)
        {
            curNode = openList.First();
            //如果当前节点就是目标节点，则找到路径，直接返回
            if (curNode.Position == goal.Position)
            {
                return CalculatePath(curNode);
            }

            ArrayList neighbours = new ArrayList();
            curNode.GetNeighbours(neighbours);//获取所有的临近节点
            for (int i = 0; i < neighbours.Count; i++)//遍历所有的临近节点，计算H/G
            {
                Node neighbourNode = (Node)(neighbours[i]);
                if (!closeList.Contains(neighbourNode))
                {//该临近节点如果在closeList中，直接跳过
                    float cost = HeursticEstimatedCost(curNode, neighbourNode);//G 表示从起始节点到当前节点的耗费
                    float totalCost = curNode.NodeTotalCost + cost;
                    float neighbourNodeEstCost = HeursticEstimatedCost(neighbourNode, goal);//H 表示从当前节点到目标节点的耗费
                    neighbourNode.NodeTotalCost = totalCost;
                    neighbourNode.Parent = curNode;
                    neighbourNode.EstimatedCost = totalCost + neighbourNodeEstCost;
                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Push(neighbourNode);
                    }

//                     Debug.LogFormat("openList {0}", openList);
//                     Debug.LogFormat("closeList {0}", closeList);
                }
            }

            closeList.Push(curNode);//当前节点加入closeList
            openList.Remove(curNode);//当前节点从openList中删除
        }


        if (curNode.Position != goal.Position)
        {
            Debug.LogError("can't find goal");
            return null;
        }


        return CalculatePath(curNode);
    }

    //计算路径
    public static ArrayList CalculatePath(Node node)
    {
        ArrayList list = new ArrayList();
        while (node != null)
        {
            list.Add(node);
            node = node.Parent;
        }

        list.Reverse();
        return list;
    }
}
