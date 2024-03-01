using System;
using UnityEngine;

namespace General
{
    public class Grid <TGridObject>
    {

        public EventHandler<OnGridValueChangedArgs> OnGridValueChanged;

        public class OnGridValueChangedArgs : EventArgs
        {
            public int x;
            public int y;
        }
        
        private int _width;
        private int _height;
        private float _cellSize;
        private Vector3 _originPos;
        
        private TGridObject[,] _gridArray;
        
        public Grid(int width, int height, float cellSize, Vector3 originPos, Func<Grid<TGridObject>, int, int, TGridObject> createGridObj, bool showDebug = false)
        {
            _height = height;
            _width = width;
            _cellSize = cellSize;
            _originPos = originPos;
            
            _gridArray = new TGridObject[width, height];

            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < _gridArray.GetLength(1); y++)
                {
                    _gridArray[x, y] = createGridObj(this, x, y);
                }
            }

            if (showDebug)
            {
                for (int x = 0; x < _gridArray.GetLength(0); x++)
                {
                    for (int y = 0; y < _gridArray.GetLength(1); y++)
                    {
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1),Color.white, 100f);
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                        //Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y - 1));
                        //Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x  - 1, y));
                    }
                }
            }
        }

        public int GetWidth()
        {
            return _width;
        }

        public int GetHeight()
        {
            return _height;
        }

        public float GetCellSize()
        {
            return _cellSize;
        }

        private Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y) * _cellSize + _originPos;
        }

        public void GetXY(Vector3 worldPos, out int x, out int y)
        {
            Vector3 pos = worldPos - _originPos;
            x = Mathf.FloorToInt(pos.x / _cellSize);
            y = Mathf.FloorToInt(pos.y / _cellSize);
        } 

        public void SetGridObject(int x, int y, TGridObject value)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                _gridArray[x, y] = value;
                if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedArgs{x = x, y = y});
            }
        }

        public void TriggerOnGridChanged(int x, int y)
        {
            if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedArgs{x = x, y = y});
        }

        public void SetGridObject(Vector3 worldPos, TGridObject value)
        {
            int x, y;
            GetXY(worldPos, out x, out y);
            SetGridObject(x, y, value);
        }

        public TGridObject GetGridObject(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height) return _gridArray[x, y];
            else return default(TGridObject);
        }
        public TGridObject GetGridObject(Vector3 worldPos)
        {
            int x, y;
            GetXY(worldPos, out x, out y);
            return GetGridObject(x, y);
        }
    }
}