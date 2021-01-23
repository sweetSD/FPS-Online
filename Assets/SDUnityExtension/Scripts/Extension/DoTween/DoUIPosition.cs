using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    namespace Tweening
    {
        public class DoUIPosition : DoBase
        {
            private RectTransform _rectTransform;

            private void Awake()
            {
                if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            }

            public override Tween GetTween()
            {
                return _rectTransform.DOAnchorPos(_destValue, _duration).SetEase(_ease);
            }

            public override Tween GetReversedTween()
            {
                return _rectTransform.DOAnchorPos(_originValue, _duration).SetEase(_ease);
            }

            public override void ResetToStart()
            {
                _rectTransform.anchoredPosition = _originValue;
            }

            public override void ResetToEnd()
            {
                _rectTransform.anchoredPosition = _destValue;
            }
        }
    }
}