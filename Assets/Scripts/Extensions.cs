using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class Extensions : MonoBehaviour
    {
    }

    public enum DirectionType : byte
    {
        Top,
        Bot,
        Left,
        Right
    }

    public enum ChipType : byte
    {
        Apple,
        Avocado,
        Kiwi,
        Banana,
        Orange,
        Peach
    }
}