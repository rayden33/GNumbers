using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CubesManager : MonoBehaviour
{
    public GameObject goCubes;
    public int countRow = 4;
    public Text txtMovedCount;
    public GameObject go_winPanel;

    private Dictionary<int, pair> dCon = new Dictionary<int, pair>();
    private int countCubes;
    private struct pair 
    {
        public int a;
        public int b;
    }
    private int count_swap = GlobalData.depth_step * 2;
    private int count_pairs;
    private GameObject[] cube;
    private List <pair> pCubes;
    private List <int> randomList;
    
 
 
    public void generateRandomList (int maxNum)
    {
        int q=0;
        int t=0;
        int nxt;
        int randomDiff = 100;
        for(int i=1;i<=maxNum;i++)
            randomList.Add(i);
        for(int i = 1; i <= randomDiff; i++)
        {
            int numSwap = Random.Range(1,maxNum);
            nxt = (t + numSwap)%maxNum;
            q = randomList[t];
            randomList[t] = randomList[nxt];
            randomList[nxt] = q;
        } 
        //Debug.Log("qwe" + randomList.Count + "qwe__" + t);
    }
    private void initCubes()
    {
        pair tp;
        int t=0;
        for (int i=0; i< countRow; i++)
        {
            for(int j=0;j<countRow;j++)
            {
                t++;
                GlobalData.cubNum[i,j] = t;
                tp.a = i;
                tp.b = j;
                dCon[t] = tp;
            }
        }

        for (int i=1; i <= countCubes; i++)
        {
            cube[i] = goCubes.transform.Find(i.ToString()).gameObject;
        }

        for (int i=1; i <= countCubes; i++)
        {
            for(int j=i; j <= countCubes; j++)
            {
                if(i==j)
                    continue;
                tp.a = i;
                tp.b = j;
                pCubes.Add(tp);
            }
        }
    }
    private void goSwap(int a, int b)
    {
        pair pTmpa,pTmpb;
        Vector3 tmp = cube[a].transform.position;
        cube[a].transform.position = cube[b].transform.position;
        cube[b].transform.position = tmp;
        dCon.TryGetValue(a, out pTmpa);
        //Debug.Log(a + "---" + pTmpa.a + ";" + pTmpa.b);
        dCon.TryGetValue(b, out pTmpb);
        //Debug.Log(b + "+++" + pTmpb.a + ";" + pTmpb.b);
        GlobalData.cubNum[pTmpa.a,pTmpa.b] = b;
        GlobalData.cubNum[pTmpb.a,pTmpb.b] = a;
        dCon[a] = pTmpb;
        dCon[b] = pTmpa;
    }
    private void genLvl()
    {
        int pairCount = randomList.Count;
        Debug.Log(count_swap);
        for (int i=0; i<count_swap; i++)
        {
            goSwap(pCubes[randomList[i]-1].a, pCubes[randomList[i]-1].b);
            //Debug.Log("---" + pCubes[randomList[i]-1].a + ";" + pCubes[randomList[i]-1].b);
        }
    }
    private bool checkCanMove(int a, int b, int i, int j)
    {
        int nextY = a + i, nextX = b + j;
        Debug.Log(nextY + "-" + nextX);
        if(nextY >= 0 && nextY < 16 && nextX >= 0 && nextX < 16)
        return (GlobalData.cubNum[nextX,nextY] == 16);
        return false;
    }

    private void showMatrix ()
    {
                                            ////////////////////DEbug.log(matrix)
        string[] s = new string[countRow];
        for (int i = 0; i < countRow; i++)
        {
            for (int j = 0; j < countRow; j++)
            {
                s[i]=s[i] + " " + GlobalData.cubNum[i,j] + "(" + i + ";" + j + ")";
            }
            Debug.Log(s[i]);
        }
        s = new string[countRow];
        foreach (var item in dCon)
        {
            Debug.Log(item.Key + "->" + item.Value.a + ";" + item.Value.b);
        }
    }
    public void restartGame() 
    {
        GlobalData.movedCount = 0;
        SceneManager.LoadScene("MainScene");
    }
    public bool checkWin()
    {
        int t=0;
        for (int i = 0; i < countRow; i++)
        {
            for (int j = 0; j < countRow; j++)
            {
                t++;
                if(GlobalData.cubNum[i,j] != t)
                return false;
                
            }
        }
        return true;
    }
    void Start()
    {
        countCubes = countRow*countRow;
        GlobalData.cubNum = new int[4,4];
        randomList = new List<int> ();
        pCubes = new List<pair> ();
        cube = new GameObject[countCubes+1];
        initCubes();
        generateRandomList(pCubes.Count);
        //genLvl();
        //showMatrix();
    }
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                var hitCol = hitInfo.collider;
                if(hitCol != null)
                {
                    if(hitCol.tag == "cube")
                    {
                        pair tmp;
                        int cubeNum;
                        bool canMove = false;
                        int.TryParse(hitCol.transform.name, out cubeNum);
                        int cubeX=-1, cubeY=-1, pCubeX, pCubeY;
                        //Debug.Log(cubeNum);
                        dCon.TryGetValue(cubeNum, out tmp);
                        cubeY = tmp.a;
                        cubeX = tmp.b;
                        dCon.TryGetValue(16, out tmp);
                        pCubeY = tmp.a;
                        pCubeX = tmp.b;
                        canMove = (Mathf.Abs(cubeY - pCubeY) + Mathf.Abs(cubeX - pCubeX) < 2); //// why didn't check for 0 because tag "cube"
                        //Debug.Log((Mathf.Abs(cubeY - pCubeY) + Mathf.Abs(cubeX - pCubeX)) + "=" + cubeY + "-" + pCubeY + "+" + cubeX + "-" + pCubeX);
                        if(canMove)
                        {
                            GlobalData.movedCount++;
                            txtMovedCount.text = GlobalData.movedCount.ToString();
                            goSwap(cubeNum, 16);
                        }
                        if(checkWin())
                        {
                            go_winPanel.SetActive(true);
                            Time.timeScale = 0;
                        }
                        
                        // for (int i = 0; i < countRow; i++)
                        // {
                        //     for (int j = 0; j < countRow; j++)
                        //     {
                        //         if(GlobalData.cubNum[i,j] == cubeNum)
                        //         {
                        //             cubeX = j;
                        //             cubeY = i;
                        //             break;
                        //         }
                        //     }
                        //     if(cubeX != -1)
                        //     break;
                        // }
                        //Debug.Log(cubeY + "-" + cubeX);
                        // for(int i=-1; i<=1; i+=2)
                        // {
                        //     for(int j=-1; j<=1; j+=2)
                        //     {
                        //         if(i + j < 2 && checkCanMove(cubeY, cubeX, i, j))
                        //         {
                        //             //Debug.Log(cubeY + "+" + i + "-" + cubeX + "+" + j);
                        //         }
                        //     }
                        // }
                        
                    }
                }
            }
        }
    }
}
