using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class BoardManager : MonoBehaviour
{
 
    public int boardRows, boardColumns;
	public int minRoomSize, maxRoomSize;
	public GameObject floorTile;
	public GameObject wallTile;
	public GameObject pathFinder;
	public Transform Player;
	public GameObject enemy;
	public GameObject chiken;
	public GameObject coin;
	public int NumberOfEnemy;

	private GameObject[,] boardPositionsFloor;
	private GameObject[,] boardPositionsUnits;
	private bool playerSetPosition = false;
	


	public class SubDungeon
	{
		public SubDungeon left, right;

		public Rect rect;
		public Rect room = new Rect(-1, -1, 0, 0); // i.e null
		public int debugId;
		public List<Rect> corridors = new List<Rect>();

		private static int debugCounter = 0;

		public SubDungeon(Rect mrect)
		{
			rect = mrect;
			debugId = debugCounter;
			debugCounter++;
		}

		public bool IAmLeaf()
		{
			return left == null && right == null;
		}

		public bool Split(int minRoomSize, int maxRoomSize)
		{
			if (!IAmLeaf())
			{
				return false;
			}

			// choose a vertical or horizontal split depending on the proportions
			// i.e. if too wide split vertically, or too long horizontally, 
			// or if nearly square choose vertical or horizontal at random
			bool splitH;
			if (rect.width / rect.height >= 1.25)
			{
				splitH = false;
			}
			else if (rect.height / rect.width >= 1.25)
			{
				splitH = true;
			}
			else
			{
				splitH = Random.Range(0.0f, 1.0f) > 0.5;
			}

			if (Mathf.Min(rect.height, rect.width) / 2 < minRoomSize)
			{
				//Debug.Log("Sub-dungeon " + debugId + " will be a leaf");
				return false;
			}

			if (splitH)
			{
				// split so that the resulting sub-dungeons widths are not too small
				// (since we are splitting horizontally) 
				int split = Random.Range(minRoomSize, (int)(rect.width - minRoomSize));

				left = new SubDungeon(new Rect(rect.x, rect.y, rect.width, split));
				right = new SubDungeon(
					new Rect(rect.x, rect.y + split, rect.width, rect.height - split));
			}
			else
			{
				int split = Random.Range(minRoomSize, (int)(rect.height - minRoomSize));

				left = new SubDungeon(new Rect(rect.x, rect.y, split, rect.height));
				right = new SubDungeon(
					new Rect(rect.x + split, rect.y, rect.width - split, rect.height));
			}

			return true;
		}

		public void CreateRoom()
		{
			if (left != null)
			{
				left.CreateRoom();
			}
			if (right != null)
			{
				right.CreateRoom();
			}
			if (left != null && right != null)
			{
				CreateCorridorBetween(left, right);
			}
			if (IAmLeaf())
			{
				int roomWidth = (int)Random.Range(rect.width / 2, rect.width - 2);
				int roomHeight = (int)Random.Range(rect.height / 2, rect.height - 2);
				int roomX = (int)Random.Range(1, rect.width - roomWidth - 1);
				int roomY = (int)Random.Range(1, rect.height - roomHeight - 1);

				// room position will be absolute in the board, not relative to the sub-dungeon
				room = new Rect(rect.x + roomX, rect.y + roomY, roomWidth, roomHeight);
				//Debug.Log("Created room " + room + " in sub-dungeon " + debugId + " " + rect);
			}
		}


		public void CreateCorridorBetween(SubDungeon left, SubDungeon right)
		{
			Rect lroom = left.GetRoom();
			Rect rroom = right.GetRoom();

			//Debug.Log("Creating corridor(s) between " + left.debugId + "(" + lroom + ") and " + right.debugId + " (" + rroom + ")");

			// attach the corridor to a random point in each room
			Vector2 lpoint = new Vector2((int)Random.Range(lroom.x + 1, lroom.xMax - 1), (int)Random.Range(lroom.y + 1, lroom.yMax - 1));
			Vector2 rpoint = new Vector2((int)Random.Range(rroom.x + 1, rroom.xMax - 1), (int)Random.Range(rroom.y + 1, rroom.yMax - 1));

			// always be sure that left point is on the left to simplify the code
			if (lpoint.x > rpoint.x)
			{
				Vector2 temp = lpoint;
				lpoint = rpoint;
				rpoint = temp;
			}

			int w = (int)(lpoint.x - rpoint.x);
			int h = (int)(lpoint.y - rpoint.y);

			//Debug.Log("lpoint: " + lpoint + ", rpoint: " + rpoint + ", w: " + w + ", h: " + h);

			// if the points are not aligned horizontally
			if (w != 0)
			{
				// choose at random to go horizontal then vertical or the opposite
				if (Random.Range(0, 1) > 2)
				{
					// add a corridor to the right
					corridors.Add(new Rect(lpoint.x, lpoint.y, Mathf.Abs(w) + 1, 1));

					// if left point is below right point go up
					// otherwise go down
					if (h < 0)
					{
						corridors.Add(new Rect(rpoint.x, lpoint.y, 1, Mathf.Abs(h)));
					}
					else
					{
						corridors.Add(new Rect(rpoint.x, lpoint.y, 1, -Mathf.Abs(h)));
					}
				}
				else
				{
					// go up or down
					if (h < 0)
					{
						corridors.Add(new Rect(lpoint.x, lpoint.y, 1, Mathf.Abs(h)));
					}
					else
					{
						corridors.Add(new Rect(lpoint.x, rpoint.y, 1, Mathf.Abs(h)));
					}

					// then go right
					corridors.Add(new Rect(lpoint.x, rpoint.y, Mathf.Abs(w) + 1, 1));
				}
			}
			else
			{
				// if the points are aligned horizontally
				// go up or down depending on the positions
				if (h < 0)
				{
					corridors.Add(new Rect((int)lpoint.x, (int)lpoint.y, 1, Mathf.Abs(h)));
				}
				else
				{
					corridors.Add(new Rect((int)rpoint.x, (int)rpoint.y, 1, Mathf.Abs(h)));
				}
			}

			//Debug.Log("Corridors: ");
			foreach (Rect corridor in corridors)
			{
				//Debug.Log("corridor: " + corridor);
			}
		}

		public Rect GetRoom()
		{
			if (IAmLeaf())
			{
				return room;
			}
			if (left != null)
			{
				Rect lroom = left.GetRoom();
				if (lroom.x != -1)
				{
					return lroom;
				}
			}
			if (right != null)
			{
				Rect rroom = right.GetRoom();
				if (rroom.x != -1)
				{
					return rroom;
				}
			}

			// workaround non nullable structs
			return new Rect(-1, -1, 0, 0);
		}
	}



	public void CreateBSP(SubDungeon subDungeon)
	{
		//Debug.Log("Splitting sub-dungeon " + subDungeon.debugId + ": " + subDungeon.rect);
		if (subDungeon.IAmLeaf())
		{
			// if the sub-dungeon is too large split it
			if (subDungeon.rect.width > maxRoomSize
				|| subDungeon.rect.height > maxRoomSize
				|| Random.Range(0.0f, 1.0f) > 0.25)
			{

				if (subDungeon.Split(minRoomSize, maxRoomSize))
				{
					//Debug.Log("Splitted sub-dungeon " + subDungeon.debugId + " in "
					//	+ subDungeon.left.debugId + ": " + subDungeon.left.rect + ", "
						//+ subDungeon.right.debugId + ": " + subDungeon.right.rect);

					CreateBSP(subDungeon.left);
					CreateBSP(subDungeon.right);
				}
			}
		}
	}

	public void SetInstance(GameObject tile, int i, int j)
    {

		if (boardPositionsFloor[i, j] == null)
		{
			GameObject instance = Instantiate(tile, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
			instance.transform.SetParent(transform);
			instance.layer = tile.layer; 
			boardPositionsFloor[i, j] = instance;
		}
	}


	public void SetItem(GameObject item,int i,int j)
    {
		GameObject instance = Instantiate(item, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
		instance.transform.SetParent(transform);
		instance.layer = item.layer;
		
	}
	public void SetPlayer(int i, int j)
    {
	
		Player.position = new Vector3(i, j, 0f);
		playerSetPosition = true;
		boardPositionsUnits[i, j] = Player.gameObject;
	}

	public void DrawWallsRooms(SubDungeon subDungeon)
    {
		if (subDungeon == null)
        {
			return;
        }
		if (subDungeon.IAmLeaf())
        {
			int i = (int)subDungeon.room.x-1;
			for (int j = ((int)subDungeon.room.y-1); j < subDungeon.room.yMax+1; j++)
			{
				SetInstance(wallTile, i, j);
			}
			i = (int)subDungeon.room.xMax;
			for (int j = ((int)subDungeon.room.y - 1); j < subDungeon.room.yMax + 1; j++)
			{
				SetInstance(wallTile, i, j);
			}
			i = (int)subDungeon.room.y-1;
			for (int j = (int)subDungeon.room.x; j < subDungeon.room.xMax; j++)
			{
				SetInstance(wallTile, j, i);
			}
			i = (int)subDungeon.room.yMax;
			for (int j = (int)subDungeon.room.x; j < subDungeon.room.xMax; j++)
			{
				SetInstance(wallTile, j, i);
			}


		}
		else
		{
			DrawWallsRooms(subDungeon.left);
			DrawWallsRooms(subDungeon.right);
		}

	}

	public void DrawWallsCorridors(SubDungeon subDungeon)
    {
		if (subDungeon == null)
		{
			return;
		}

		DrawWallsCorridors(subDungeon.left);
		DrawWallsCorridors(subDungeon.right);

		foreach (Rect corridor in subDungeon.corridors)
		{

			int i = (int)corridor.x - 1;
			for (int j = (int)corridor.y-1; j < corridor.yMax+1; j++)
			{
				SetInstance(wallTile, i, j);

			}
			i = (int)corridor.xMax ;
			for (int j = (int)corridor.y-1; j < corridor.yMax+1; j++)
			{
				SetInstance(wallTile, i, j);

			}
			i = (int)corridor.y-1;
			for (int j = (int)corridor.x; j < corridor.xMax; j++)
			{
				SetInstance(wallTile, j, i);

			}
			i = (int)corridor.yMax;
			for (int j = (int)corridor.x; j < corridor.xMax; j++)
			{
				SetInstance(wallTile, j, i);

			}

		}

	}

	public void SetEnemy(GameObject unit, int i, int j)
    {
		
		
			GameObject instance = Instantiate(unit, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
			instance.transform.SetParent(transform);
			instance.layer = unit.layer;
			boardPositionsUnits[i, j] = instance;
			NumberOfEnemy++;
		

	}

	public void DrawRooms(SubDungeon subDungeon)
	{
		if (subDungeon == null)
		{
			return;
		}
		if (subDungeon.IAmLeaf())
		{
			//tworzenie pokoju
			//sprawdzić które punkty to krawędzie i stawić tam inne klocki
			for (int i = (int)subDungeon.room.x; i < subDungeon.room.xMax; i++)
			{
				for (int j = (int)subDungeon.room.y; j < subDungeon.room.yMax; j++)
				{

					SetInstance(floorTile, i, j);
					

				}
			}

			int Randx;
			int Randy;

			if (playerSetPosition)
			{
				
				int x = (int)Random.Range(1, 4);
				for (int i = 0; i < x; i++)
				{
					do
					{
						Randx = (int)Random.Range(subDungeon.room.x, subDungeon.room.xMax);
						Randy = (int)Random.Range(subDungeon.room.y, subDungeon.room.yMax);
					}
					while (boardPositionsUnits[Randx, Randy] != null);

					SetEnemy(enemy, Randx, Randy);
				}
			}
            else
            {
				
				Randx = (int)Random.Range(subDungeon.room.x, subDungeon.room.xMax);
				Randy = (int)Random.Range(subDungeon.room.y, subDungeon.room.yMax);
				
				SetPlayer(Randx, Randy);
			}

			float chance = Random.Range(1, 100);
            if (chance<=50)
            {
				do
				{
					Randx = (int)Random.Range(subDungeon.room.x, subDungeon.room.xMax);
					Randy = (int)Random.Range(subDungeon.room.y, subDungeon.room.yMax);
				}
				while (boardPositionsUnits[Randx, Randy] != null);
				SetItem(chiken, Randx, Randy);
            }

		}
		else
		{
			DrawRooms(subDungeon.left);
			DrawRooms(subDungeon.right);
		}
	}

	void DrawCorridors(SubDungeon subDungeon)
	{
		if (subDungeon == null)
		{
			return;
		}

		DrawCorridors(subDungeon.left);
		DrawCorridors(subDungeon.right);

		foreach (Rect corridor in subDungeon.corridors)
		{
			for (int i = (int)corridor.x; i < corridor.xMax; i++)
			{
				for (int j = (int)corridor.y; j < corridor.yMax; j++)
				{
					SetInstance(floorTile, i, j);

				}
			}
		}
	}

	void CreatePathFinder(SubDungeon subDungeon)
    {
		GameObject instance = Instantiate(pathFinder, new Vector3(49.5f, 50.5f, 0f), Quaternion.identity) as GameObject;
		instance.transform.SetParent(transform);
		
		
	}
	
	public void end()
    {
		SceneManager.LoadScene(0);
    }
	public void EnemyLeft()
    {
		Debug.Log("Zostalo"+NumberOfEnemy);
        if (NumberOfEnemy == 0)
        {
			end();
        }
    }

	void Start()
	{
		EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
		enemyAI.target = Player;
		SubDungeon rootSubDungeon = new SubDungeon(new Rect(0, 0, boardRows, boardColumns));
		CreateBSP(rootSubDungeon);
		rootSubDungeon.CreateRoom();

		boardPositionsFloor = new GameObject[boardRows, boardColumns];
		boardPositionsUnits = new GameObject[boardRows, boardColumns];
		NumberOfEnemy = 0;
		DrawRooms(rootSubDungeon);
		
		DrawCorridors(rootSubDungeon);
		DrawWallsRooms(rootSubDungeon);
		DrawWallsCorridors(rootSubDungeon);
		CreatePathFinder(rootSubDungeon);
		
	}
}