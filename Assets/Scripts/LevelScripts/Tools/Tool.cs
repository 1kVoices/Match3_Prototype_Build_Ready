﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public abstract class Tool : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _amountText;
        [SerializeField]
        private TextMeshProUGUI _cdText;
        [SerializeField]
        private Button _button;
        [SerializeField]
        private Tool _brother;
        private bool _isUsed;
        private bool _isBreak;
        public bool IsInteractable { get; private set; }
        protected bool IsInput;
        protected int UseAmount;
        protected int HitAmount;
        private int _baseHitAmount;
        private float _cooldown;
        private ColorBlock _colors;
        private bool _isCd;
        private TimeSpan _timeSpan;
        public event Action<Tool> ToolActive;
        public event Action ToolUsed;

        private void Start()
        {
            LevelManager.Singleton.OnPlayerClick += PlayerClicked;
            LevelManager.Singleton.CellPointerEnter += OnCellPointerEnter;
            LevelManager.Singleton.CellPointerExit += OnCellPointerExit;
            if (UseAmount < 1) _button.interactable = false;
            _amountText.text = UseAmount.ToString();
            _button.interactable = false;
            IsInteractable = true;
            _colors = _button.colors;
            _cdText.text = "";
        }

        public void OnClick()
        {
            if (LevelManager.Singleton.InputState == false) return;
            _brother.DisableButton();
            ToolActive?.Invoke(this);
            LevelManager.Singleton.SetToolState(true);
            LevelManager.Singleton.DeactivateHint();
            IsInput = true;
            _colors.normalColor = Color.red;
            _button.colors = _colors;
            _isUsed = false;
            _isBreak = false;
        }

        protected virtual void PlayerClicked(Cell cell)
        {
            ToolUsed?.Invoke();
            _isUsed = true;
            HitAmount--;
            _amountText.text = UseAmount.ToString();
        }

        protected abstract void OnCellPointerEnter(Cell cell);
        protected abstract void OnCellPointerExit(Cell cell);

        protected void UpdateState()
        {
            LevelManager.Singleton.SetToolState(false);
            LevelManager.Singleton.ActivateHint();
            IsInput = false;
            _colors.normalColor = Color.black;
            _button.colors = _colors;
            _amountText.text = UseAmount.ToString();

            if (_isBreak)
            {
                EnableButton();
                if (_brother.IsInteractable)
                    _brother.EnableButton();
                return;
            }

            IsInteractable = false;
            if (_brother.IsInteractable)
                _brother.EnableButton();
            if (UseAmount > 0 && _isUsed) _isCd = true;
        }

        public void EnableButton() => _button.interactable = true;
        public void DisableButton() => _button.interactable = false;

        public void BreakTool()
        {
            _isBreak = true;
            UpdateState();
        }

        private void Update()
        {
            if (!_isCd) return;
            _cooldown -= Time.deltaTime;
            _timeSpan = TimeSpan.FromSeconds(_cooldown);
            _cdText.text = _timeSpan.ToString(@"mm\:ss");
            if (_cooldown >= 0) return;
            _isCd = false;
            _button.interactable = true;
            HitAmount = _baseHitAmount;
            _cdText.text = "";
            IsInteractable = true;
        }

        private void OnDestroy()
        {
            LevelManager.Singleton.OnPlayerClick -= PlayerClicked;
            LevelManager.Singleton.CellPointerEnter -= OnCellPointerEnter;
            LevelManager.Singleton.CellPointerExit -= OnCellPointerExit;
        }

        public void SetStats(int useAmount, int hitAmount, int cooldown)
        {
            UseAmount = useAmount;
            HitAmount = hitAmount;
            _baseHitAmount = hitAmount;
            _cooldown = cooldown * 60;
        }
    }
}