using UnityEngine;
using System.Collections;

public enum GRID_TYPE
{
    UNKOWN,//未知格子类型
    NORMAL,//普通格子类型
    WATER,//水格子类型
    BARRIER,//栅栏格子类型
    TARGET//目标格子类型
}

//错误码定义
public enum ERROR_CODE
{
    NOERROR//没有错误
}

//格子定义
public class Grid
{
    //格子的索引属性
    //public int Index{get;set;}

    //节点信息，用于寻路计算
    public Node Node;

    //实体属性
    public GameObject Object;

    public Grid()
    {
        Node = new Node();
        Object = new GameObject();
    }
}


public struct GridConfig
{
    public int NumOfColumn { get; set; }//网格列数
    public int NumOfRow { get; set; }//网格行数
    public float GridCellSize { get; set; }//网格边长
    public bool ShowGrid { get; set; }//是否显示网格

}

public class GridManager
{
    //配置信息
    public static GridConfig Config { get; set; }

    //单元格数组
    public static Grid[,] Cells;

    //实例对象
    public static GridManager Instance{set;get;}

    //初始化函数
    public static ERROR_CODE Init(GridConfig config)
    {
        //如果实例为null，new一下
        if (Instance == null)
        {
            Instance = new GridManager();
        }

        //初始化配置项
        Config = config;

        //初始化网格数组
        Cells = new Grid[Config.NumOfColumn, Config.NumOfRow];
        for (int i = 0; i < Config.NumOfRow; i++)
        {
            for (int j = 0; j < Config.NumOfColumn; j++)
            {
                Cells[j, i] = new Grid();
//                 Cells[j, i].Object = new GameObject();
//                 Cells[j, i].Node = new Node();
            }
        }

        Node.Origin =new Vector3(-(Config.NumOfColumn / 2) * Config.GridCellSize, -(Config.NumOfRow / 2) * Config.GridCellSize, 0);

        return ERROR_CODE.NOERROR;
    }

    //打印网格
    public static void OnDrawGizmos()
    {
        if (Config.ShowGrid)
        {
            DebugDrawGrid(new Vector3(-2.4f, -4.8f, 0), Config.NumOfRow, Config.NumOfColumn, Config.GridCellSize, Color.green);
        }
    }

    public static void DebugDrawGrid(Vector3 origin, int numRows, int numColumns, float cellSize, Color color)
    {
        float width = (numColumns * cellSize);
        float height = (numRows * cellSize);
        //draw the horaziontal grid line
        for (int i = 0; i < numRows + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * new Vector3(0.0f, 1.0f, 0f);
            Vector3 endPos = startPos + width * new Vector3(1.0f, 0.0f, 0.0f);
            if (i == numRows/2)
            {
                Debug.DrawLine(startPos, endPos, Color.red);
            }
            else
            {
                Debug.DrawLine(startPos, endPos, color);
            }
            
        }

        //draw the vertial grid line
        for (int i = 0; i < numColumns + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 endPos = startPos + height * new Vector3(0.0f, 1.0f, 0.0f);
            if (i == numColumns/2)
            {
                Debug.DrawLine(startPos, endPos, Color.red);
            }
            else
            {
                Debug.DrawLine(startPos, endPos, color);
            }
        }
    }


}
