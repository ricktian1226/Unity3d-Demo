using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour
{
    //一个栅栏实例
    private GameObject barrier;

    //
    public float GridSize = 0.48f;

    //游戏启动前执行
    void Awake()
    {
        GridConfig config = new GridConfig()
        {
            NumOfColumn = 10,
            NumOfRow = 20,
            ShowGrid = true,
            GridCellSize = 0.48f
        };

        //初始化GridManager
        ERROR_CODE errCode = GridManager.Init(config);
        if (errCode != ERROR_CODE.NOERROR)
        {
            //todo
        }
    }


    //游戏启动时执行
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GridManager.OnDrawGizmos();

        barrier = GameObject.FindGameObjectWithTag("Barrier");
        barrier.transform.position = new Vector3(-0.96f, 0, 0);

//         if (barrier == null)
//         {
//             barrier = Instantiate(GameObject.FindGameObjectWithTag("Barrier"), new Vector3(0.48f, 0.48f, 0), Quaternion.identity) as GameObject;
//         }

        Vector3 mousePositionOnScreen = Input.mousePosition;
        Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePositionOnScreen);
        mousePositionInWorld.z = 0;
        barrier.transform.position = Node.GetCenterFromPosition(mousePositionInWorld);
        if (Input.GetButtonDown("Fire1"))//如果点击鼠标左键，则生成一个栅栏对象
        {
            var position = barrier.transform.position;
            var row = Node.GetRowFromPosition(position);
            var column = Node.GetColumnFromPosition(position);
            if (null != GridManager.Cells[column, row].Object)//如果已经存在一个Object，则先销毁它
            {
                Destroy(GridManager.Cells[column, row].Object);
            }

            //重新设置一下对象和类型
            GridManager.Cells[column, row].Object = Instantiate(GameObject.FindGameObjectWithTag("Barrier"), position, Quaternion.identity) as GameObject;
            GridManager.Cells[column, row].Node.Type = GRID_TYPE.BARRIER;
        }
    }
}
