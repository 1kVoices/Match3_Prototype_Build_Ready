using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class LevelManager : MonoBehaviour
    {
        private int _currentLevel;

        [SerializeField]
        private CellComponent[] _cells;
        private LinkedList<ChipComponent> _chipPool;
        private Controls _controls;
        private Vector2 _startDragMousePos;
        private bool _isReading;
        private CellComponent _currentCell;
        [SerializeField]
        private float _dragSens;

        private void Start()
        {
            _controls = new Controls();
            _controls.Enable();
            _chipPool = new LinkedList<ChipComponent>();
            _startDragMousePos = Vector2.zero;

            foreach (var cell in _cells)
            {
                cell.PoolingEvent += OnPoolingEvent;
                cell.PointerDownEvent += OnCellPointerDownEvent;
                cell.PointerUpEvent += OnCellPointerUpEvent;
            }
        }

        private void OnCellPointerUpEvent(CellComponent cell) => _isReading = false;

        private void OnCellPointerDownEvent(CellComponent cell, Vector2 cellPos)
        {
            _currentCell = cell;
            _isReading = true;
            _startDragMousePos = cellPos;
        }

        private void Update()
        {
            ReadPlayerInput();
        }

        private void ReadPlayerInput()
        {
            if (!_isReading) return;
            Vector2 newMousePos = _controls.MainMap.Mouse.ReadValue<Vector2>();

            if (newMousePos.x - _startDragMousePos.x > _dragSens)
            {
                _isReading = false;
                print("Right");
            }
            else if (newMousePos.x - _startDragMousePos.x < -_dragSens)
            {
                _isReading = false;
                print("Left");
            }
            else if (newMousePos.y - _startDragMousePos.y > _dragSens)
            {
                _isReading = false;
                print("Top");
            }
            else if (newMousePos.y - _startDragMousePos.y < -_dragSens)
            {
                _isReading = false;
                print("Bot");
            }
        }

        private void OnPoolingEvent(ChipComponent chip)
        {
            _chipPool.AddLast(chip);
        }

        private void OnDestroy()
        {
            _controls.Dispose();
        }
    }
}