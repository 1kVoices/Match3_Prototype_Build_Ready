using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class TreeManager : MonoBehaviour
    {
        [SerializeField]
        private Animator[] _trees;
        [SerializeField]
        private Animator _blackScreen;
        [SerializeField]
        private Button _next;
        [SerializeField]
        private Button _previous;
        [SerializeField]
        private Button _exit;
        private int _currentTree = 0;
        private static readonly int LeftShow = Animator.StringToHash("leftShow");
        private static readonly int LeftHide = Animator.StringToHash("leftHide");
        private static readonly int RightShow = Animator.StringToHash("rightShow");
        private static readonly int RightHide = Animator.StringToHash("rightHide");
        private static readonly int HalfDarken = Animator.StringToHash("halfDarken");
        private static readonly int Whitening = Animator.StringToHash("whitening");

        public void ShowTrees()
        {
            _trees[_currentTree].SetTrigger(RightShow);
            _blackScreen.SetTrigger(HalfDarken);
            _next.gameObject.SetActive(true);
            _previous.gameObject.SetActive(true);
            _exit.gameObject.SetActive(true);
        }

        public void NextTree()
        {
            if (_currentTree >= _trees.Length - 1) return;
            _trees[_currentTree].SetTrigger(LeftHide);
            _currentTree++;
            _trees[_currentTree].SetTrigger(RightShow);
        }

        public void PreviousTree()
        {
            if (_currentTree == 0) return;
            _trees[_currentTree].SetTrigger(RightHide);
            _currentTree--;
            _trees[_currentTree].SetTrigger(LeftShow);
        }

        public void Exit()
        {
            _next.gameObject.SetActive(false);
            _previous.gameObject.SetActive(false);
            _exit.gameObject.SetActive(false);
            _blackScreen.SetTrigger(Whitening);
            _trees[_currentTree].SetTrigger(LeftHide);
        }
    }
}