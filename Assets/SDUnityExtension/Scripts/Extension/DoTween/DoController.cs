using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    namespace Tweening
    {
        public class DoController : MonoBehaviour
        {
            [SerializeField] private List<DoBase> _doBases;

            public void Play(float delay = 0)
            {
                StartCoroutine(CO_Play(delay));
            }

            private IEnumerator CO_Play(float delay)
            {
                yield return new WaitForSeconds(delay);
                for (int i = 0; i < _doBases.Count; i++) _doBases[i].DOPlay();
            }

            public void PlayReverse(float delay = 0)
            {
                StartCoroutine(CO_PlayReverse(delay));
            }

            private IEnumerator CO_PlayReverse(float delay)
            {
                yield return new WaitForSeconds(delay);
                for (int i = 0; i < _doBases.Count; i++) _doBases[i].DoPlayReverse();
            }
        }

    }
}