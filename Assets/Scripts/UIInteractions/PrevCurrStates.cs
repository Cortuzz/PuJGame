using UnityEngine;

namespace UIInteractions
{
    public static class PrevCurrStates
    {
        public static GameObject PrevState;
        public static GameObject CurrState;

        public static void SetStates(GameObject prev, GameObject curr)
        {
            PrevState = prev;
            CurrState = curr;
        }
    }
}