using System;

namespace Match3
{
    public static class MenuEvents
    {
        public static event Action BlackScreenBleached;
        public static event Action PedroAskedHelp;
        public static event Action BlackScreenDarken;

        public static void OnBlackScreenBleached() => BlackScreenBleached?.Invoke();
        public static void OnPedroAskedHelp() => PedroAskedHelp?.Invoke();
        public static void OnBlackScreenDarken() => BlackScreenDarken?.Invoke();
    }
}