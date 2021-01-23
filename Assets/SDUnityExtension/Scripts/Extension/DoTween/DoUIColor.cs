using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    namespace Tweening
    {
        public class DoUIColor : DoBase
        {
            [SerializeField] protected Graphic _graphic;
            [SerializeField] protected Color _originColor = Color.white;
            [SerializeField] protected Color _destColor = Color.white;

            private void Awake()
            {
                if (_graphic == null) _graphic = GetComponent<Graphic>();
            }

            public override Tween GetTween()
            {
                return _graphic.DOColor(_destColor, _duration).SetEase(_ease);
            }

            public override Tween GetReversedTween()
            {
                return _graphic.DOColor(_originColor, _duration).SetEase(_ease);
            }

            public override void ResetToStart()
            {
                _graphic.color = _originColor;
            }

            public override void ResetToEnd()
            {
                _graphic.color = _destColor;
            }
        }
    }
}