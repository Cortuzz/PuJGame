using UnityEngine;

namespace UIInteractions
{
    public class BackButton: MonoBehaviour
    {
        public void ReturnToPrevState()
        {
            PrevCurrStates.CurrState.SetActive(false);
            PrevCurrStates.PrevState.SetActive(true);
        }   
    }
}