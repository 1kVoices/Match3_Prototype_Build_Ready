using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Match3
{
    public class Cell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public StandardChip CurrentChip { get; private set; }
        public StandardChip PreviousChip { get; private set; }
        public StandardChip TemporaryChip { get; private set; }
        private SpecialChip _currentSpecial;
        private int _cellsLayer;
        private int _spawnsLayer;
        public Cell Top { get; private set; }
        public Cell Bot { get; private set; }
        public Cell Left { get; private set; }
        public Cell Right { get; private set; }
        private List<Cell> _poolingNeighbours;
        private bool IsMatch { get; set; }
        private Cell _pulledBy;
        private CellHighlighter _highlighter;
        public bool IsHighlighting { get; private set; }
        public SpawnPoint SpawnPoint { get; private set; }
        public event Action<Cell,Vector2> PointerDownEvent;
        public event Action<Cell> PointerUpEvent;
        public event Action<Cell> PointerClickEvent;
        public event Action<Cell> PointerEnter;
        public event Action<Cell> PointerExit;

        private void Start()
        {
            _poolingNeighbours = new List<Cell>();
            _highlighter = GetComponentInChildren<CellHighlighter>();
            _cellsLayer = LayerMask.GetMask("Level");
            _spawnsLayer = LayerMask.GetMask("Spawn");
            FindNeighbours();
        }

        public void CheckMatches(StandardChip newChip)
        {
            IsMatch = false;
            _poolingNeighbours.Clear();
            SetPreviousChip(CurrentChip);
            SetCurrentChip(newChip);
            if (newChip.Type == ChipType.None && !newChip.IsTransferred)
            {
                _currentSpecial = CurrentChip.GetComponent<SpecialChip>();
                IsMatch = true;
                return;
            }

            Extensions.FindMatches(_poolingNeighbours, this);

            Pulling(_poolingNeighbours.ToArray());
            IsMatch = _poolingNeighbours.Count > 1;
        }

        public void ChipMoved()
        {
            if (!this.HasChip()) return;
            switch (IsMatch)
            {
                case true when _currentSpecial is not null:
                    _currentSpecial.Action();
                    SetPreviousChip(CurrentChip);
                    SetCurrentChip(null);
                    _currentSpecial = null;
                    break;
                case true:
                    CurrentChip.SetPreviousCell(null);
                    Pooling();
                    break;
                case false when CurrentChip.PreviousCell is not null && CurrentChip.PreviousCell.IsMatch == false:
                    CurrentChip.SendBack();
                    SetCurrentChip(PreviousChip);
                    SetPreviousChip(null);
                    break;
                case false:
                    CurrentChip.SetPreviousCell(null);
                    CurrentChip.SetInteractionState(true);
                    CurrentChip.SetTransferState(false);
                    break;
            }
        }
        /// <summary>
        /// Пуллинг - вытягивание.
        /// Этот метод устанавливает всем cells в поле _pulledBy себя. Если у клетки уже есть _pulledBy, то данная клетка
        /// объединяет свой _poolingNeighbours лист с первой. Это позволяет находить редкие комбинации типа зиг-заг и прочие
        /// </summary>
        private void Pulling(Cell[] cells)
        {
            if (!_poolingNeighbours.Contains(this))
                _poolingNeighbours.Add(this);

            foreach (Cell cell in cells)
            {
                if (cell is null || !cell.HasChip()) continue;

                cell.CurrentChip.SetInteractionState(false);

                if (cell._pulledBy is null)
                    cell.SetPulledByCell(this);
                else
                {
                    _poolingNeighbours.AddRange(cell._pulledBy._poolingNeighbours);

                    foreach (Cell pulledCell in _poolingNeighbours)
                        pulledCell.SetPulledByCell(this);
                }
            }
        }
        /// <summary>
        /// Пуллинг - отправка в пул.
        /// В данном случае сначала отправка к менеджеру и потом в пул
        /// </summary>
        private void Pooling()
        {
            if (!this.HasChip()) return;
            if (CurrentChip.Type != ChipType.None)
            {
                if (_pulledBy is not null && _pulledBy != this)
                {
                    _pulledBy._poolingNeighbours.AddRange(_poolingNeighbours);
                    _poolingNeighbours.Clear();
                    return;
                }
            }

            var cleared = _poolingNeighbours.Distinct();
            _poolingNeighbours = cleared.ToList();
            LevelManager.Singleton.DestroyChips(this, _poolingNeighbours.ToArray());
        }

        public void TransferChip(Cell callerCell)
        {
            StartCoroutine(CurrentChip.Transfer(callerCell, LevelManager.Singleton.ChipsFallTime));
            SetCurrentChip(null);
        }

        public void ChipFade()
        {
            if (!this.HasChip()) return;

            CurrentChip.FadeOut(_pulledBy is not null
                ? _pulledBy.HasChip()
                    ? _pulledBy.CurrentChip.GetComponent<SpecialChip>()
                    : _pulledBy.PreviousChip is not null
                        ? _pulledBy.PreviousChip.GetComponent<SpecialChip>()
                        : null
                : null);

            SetPreviousChip(CurrentChip);
            SetCurrentChip(null);
        }

        public IEnumerator SetSpecialChip(SpecialChip specialChip)
        {
            yield return null;
            specialChip.SetPreviousCell(null);
            specialChip.transform.parent = transform;
            specialChip.transform.position = transform.position;
            specialChip.SetCurrentCell(this);
            specialChip.ShowUp();
            SetCurrentChip(specialChip);
        }

        private void FindNeighbours()
        {
            RaycastHit2D topRay = Physics2D.Raycast(transform.position, transform.up, 1f, _cellsLayer);
            RaycastHit2D botRay = Physics2D.Raycast(transform.position, -transform.up, 1f, _cellsLayer);
            RaycastHit2D leftRay = Physics2D.Raycast(transform.position, -transform.right, 1f, _cellsLayer);
            RaycastHit2D rightRay = Physics2D.Raycast(transform.position, transform.right, 1f, _cellsLayer);
            RaycastHit2D spawnRay = Physics2D.Raycast(transform.position, transform.up, 10f, _spawnsLayer);

            if (topRay.collider is not null) Top = topRay.collider.GetComponent<Cell>();
            if (botRay.collider is not null) Bot = botRay.collider.GetComponent<Cell>();
            if (leftRay.collider is not null) Left = leftRay.collider.GetComponent<Cell>();
            if (rightRay.collider is not null) Right = rightRay.collider.GetComponent<Cell>();
            if (spawnRay.collider is not null) SpawnPoint = spawnRay.collider.GetComponent<SpawnPoint>();
        }

        public void Highlight(bool isActivate)
        {
            if (isActivate)
                _highlighter.Activate();
            else
                _highlighter.Deactivate();
        }

        public void SetCurrentChip(StandardChip newChip) => CurrentChip = newChip;
        private void SetPreviousChip(StandardChip newChip) => PreviousChip = newChip;
        public void SetTemporaryChip(StandardChip newChip) => TemporaryChip = newChip;
        public void SetPulledByCell(Cell cell) => _pulledBy = cell;
        public void SetHighlightState(bool state) => IsHighlighting = state;
        public void OnPointerDown(PointerEventData eventData) => PointerDownEvent?.Invoke(this, eventData.position);
        public void OnPointerUp(PointerEventData eventData) => PointerUpEvent?.Invoke(this);
        public void OnPointerEnter(PointerEventData eventData) => PointerEnter?.Invoke(this);
        public void OnPointerExit(PointerEventData eventData) => PointerExit?.Invoke(this);
        public void Redness(bool state) => _highlighter.Redness(state);

        public void OnPointerClick(PointerEventData eventData)
        {
            PointerClickEvent?.Invoke(this);
            if (LevelManager.Singleton.GetInputState() == false) return;
            if (!this.HasChip() || !CurrentChip.IsInteractable || CurrentChip.Type != ChipType.None) return;

            LevelManager.Singleton.DestroyChips(this, this);
        }
    }
}