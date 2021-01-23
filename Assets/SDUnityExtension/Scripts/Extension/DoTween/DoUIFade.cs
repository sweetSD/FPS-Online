using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    namespace Tweening
    {
        [RequireComponent(typeof(CanvasGroup))]
        public class DoUIFade : DoBase
        {
            [SerializeField] protected CanvasGroup _canvasGroup;

            private void Awake()
            {
                if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
            }

            public override Tween GetTween()
            {
                return _canvasGroup.DOFade(_destValue.x, _duration).SetEase(_ease);
            }

            public override Tween GetReversedTween()
            {
                return _canvasGroup.DOFade(_originValue.x, _duration).SetEase(_ease);
            }

            public override void ResetToStart()
            {
                _canvasGroup.alpha = _originValue.x;
            }

            public override void ResetToEnd()
            {
                _canvasGroup.alpha = _destValue.x;
            }
        }
    }
}