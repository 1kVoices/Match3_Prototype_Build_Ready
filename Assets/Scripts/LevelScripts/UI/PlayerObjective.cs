﻿using System;
using Match3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PlayerObjective : MonoBehaviour
{
    private Image _objectiveChip;
    private TextMeshProUGUI _count;
    public int CurrentCount { get; private set; }
    [SerializeField]
    private Sprite _apple;
    [SerializeField]
    private Sprite _avocado;
    [SerializeField]
    private Sprite _kiwi;
    [SerializeField]
    private Sprite _banana;
    [SerializeField]
    private Sprite _orange;
    [SerializeField]
    private Sprite _peach;

    private void Start()
    {
        _objectiveChip = GetComponent<Image>();
        _count = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateCount()
    {
        CurrentCount = int.Parse(_count.text);
        CurrentCount--;

        _count.text = CurrentCount.ToString();
    }

    public void SetCurrentObjective(ChipType type, int count)
    {
        CurrentCount = count;
        _count.text = CurrentCount.ToString();

        switch (type)
        {
            case ChipType.Apple:
                _objectiveChip.sprite = _apple;
                break;
            case ChipType.Avocado:
                _objectiveChip.sprite = _avocado;
                break;
            case ChipType.Kiwi:
                _objectiveChip.sprite = _kiwi;
                break;
            case ChipType.Banana:
                _objectiveChip.sprite = _banana;
                break;
            case ChipType.Orange:
                _objectiveChip.sprite = _orange;
                break;
            case ChipType.Peach:
                _objectiveChip.sprite = _peach;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}
