using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//用于寻路计算的节点定义
public class Node : IComparable
{
    public float NodeTotalCost { get; set; }
    public float EstimatedCost { get; set; }//估计耗费
    public GRID_TYPE Type { get; set; }//节点类型
    public Node Parent { get; set; }//父节点
    public Vector3 Position { get; set; }//节点的世界坐标
    public static Vector3 Origin{get;set;}//世界坐标原点

    public Node()
    {
        this.NodeTotalCost = 1.0f;
        this.EstimatedCost = 0.0f;
        this.Parent = null;
        this.Type = GRID_TYPE.NORMAL;                
    }

    public int CompareTo(object o)//比较函数。保存节点的ArrayList根据这个函数进行排序
    {
        Node node = (Node)o;
        if (this.EstimatedCost < node.EstimatedCost)
        {
            return -1;
        }
        else if (this.EstimatedCost > node.EstimatedCost)
        {
            return 1;
        }
        return 0;
    }

    //根据当前位置获取当前网格的中心点
    public static Vector3 GetCenterFromPosition(Vector3 point)
    {
        var gridCellSize = GridManager.Config.GridCellSize;
        Vector3 center = new Vector3();
        //position位于正轴区间加gridCellSize/2；负轴区间减gridCellSize/2。
        center.x = point.x >= 0 ? ((int)(point.x / gridCellSize)) * gridCellSize + gridCellSize / 2 : ((int)(point.x / gridCellSize)) * gridCellSize - gridCellSize / 2;
        center.y = point.y >= 0 ? ((int)(point.y / gridCellSize)) * gridCellSize + gridCellSize / 2 : ((int)(point.y / gridCellSize)) * gridCellSize - gridCellSize / 2;
        center.z = point.z;

        return center;
    }


    //获取节点的中心Screen坐标
    public static Vector3 GetCenterFromIndex(int index)
    {
        var gridCellSize = GridManager.Config.GridCellSize;
        Vector3 cellPosition = GetPositionFromIndex(index);
        cellPosition.x += (gridCellSize / 2.0f);
        cellPosition.y += (gridCellSize / 2.0f);
        return cellPosition;
    }


    //根据索引获取节点的Screen坐标
    public static Vector3 GetPositionFromIndex(int index)
    {
        var gridCellSize = GridManager.Config.GridCellSize;
        int row = GetRowFromIndex(index);
        int column = GetColumnFromIndex(index);
        float xPosInGrid = column * gridCellSize;
        float yPosInGrid = row * gridCellSize;
        return Origin + new Vector3(xPosInGrid, yPosInGrid, 0.0f);
    }

    //根据坐标位置获取网格的索引
    public static int GetIndex(Vector3 pos)
    {
        if (!IsInBound(pos))
        {
            Debug.LogErrorFormat("pos({0} is not in bound!)", pos.ToString());
            return -1;
        }

        var gridCellSize = GridManager.Config.GridCellSize;
        pos -= Origin;
        int column = (int)(pos.x / gridCellSize);
        int row = (int)(pos.y / gridCellSize);
        int index = (row * GridManager.Config.NumOfColumn + column);
        /*Debug.LogFormat("get index({0}) for pos {1}", index, pos.ToString());*/
        return index;
    }

    //判断节点是否在有效范围内
    public static bool IsInBound(Vector3 position)
    {
        var gridCellSize = GridManager.Config.GridCellSize;
        float width = GridManager.Config.NumOfColumn * gridCellSize;
        float height = GridManager.Config.NumOfRow * gridCellSize;

        if (position.x < Origin.x)
        {
            return false;
        }
        else if (position.x > Origin.x + width)
        {
            return false;
        }
        else if (position.y < Origin.y)
        {
            return false;
        }
        else if (position.y > Origin.y + height)
        {
            return false;
        }

        return true;
        /*return (position.x >= Origin.x && position.x <= Origin.x + width && position.y >= Origin.y && position.y <= Origin.y + height);*/
    }

    //获取行数
    public static int GetRowFromPosition(Vector3 position)
    {
        int index = GetIndex(position);
        return index / GridManager.Config.NumOfColumn;
    }

    //获取列号
    public static int GetColumnFromPosition(Vector3 position)
    {
        int index = GetIndex(position);
        return index % GridManager.Config.NumOfColumn;
    }


    //获取行数
    public static int GetRowFromIndex(int index)
    {
        return index / GridManager.Config.NumOfColumn;
    }


    //获取列号
    public static int GetColumnFromIndex(int index)
    {
        return index % GridManager.Config.NumOfColumn;
    }

    //添加上下左右临近节点
    public void GetNeighbours(ArrayList neighbours)
    {
        Vector3 neighbourPos = this.Position;
        int neighbourIndex = GetIndex(neighbourPos);
        int row = GetRowFromIndex(neighbourIndex);
        int column = GetColumnFromIndex(neighbourIndex);
        //bottom
        int nodeRow = row - 1;
        int nodeColumn = column;
        AssignNeighbour(nodeRow, nodeColumn, neighbours);
        //top
        nodeRow = row + 1;
        nodeColumn = column;
        AssignNeighbour(nodeRow, nodeColumn, neighbours);
        //left
        nodeRow = row;
        nodeColumn = column - 1;
        AssignNeighbour(nodeRow, nodeColumn, neighbours);
        //right
        nodeRow = row;
        nodeColumn = column + 1;
        AssignNeighbour(nodeRow, nodeColumn, neighbours);
    }

    //添加临近节点
    void AssignNeighbour(int row, int column, ArrayList neighbours)
    {
        if (row != -1 && column != -1 && row < GridManager.Config.NumOfRow && column < GridManager.Config.NumOfColumn)
        {
            //获取对应格子的节点信息
            Node nodeToAdd = GridManager.Cells[column, row].Node;
            if (!nodeToAdd.IsObstacle())//非阻碍节点，则加入临近节点列表中
            {
                neighbours.Add(nodeToAdd);
            }
        }
    }

    //节点是否是阻碍的
    bool IsObstacle()
    {
        //todo 根据节点的类型(this.Type)断定节点是否是阻碍的
        
        return false;
    }


    public void DebugDrawGrid(Vector3 origin, int numRows, int numColumns, float cellSize, Color color)
    {
        float width = (numColumns * cellSize);
        float height = (numRows * cellSize);
        //draw the horaziontal grid line
        for (int i = 0; i < numRows + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 endPos = startPos + height * new Vector3(1.0f, 0.0f, 0.0f);
            Debug.DrawLine(startPos, endPos, color);
        }

        //draw the vertial grid line
        for (int i = 0; i < numColumns + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 endPos = startPos + width * new Vector3(0.0f, 0.0f, 1.0f);
            Debug.DrawLine(startPos, endPos, color);
        }
    }


}
