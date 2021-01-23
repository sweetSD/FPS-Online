using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DG
{
    namespace Tweening
    {
        public class DoUIFill : DoBase
        {
            [SerializeField] protected Image _image;

            private void Awake()
            {
                if (_image == null) _image = GetComponent<Image>();
            }

            public override Tween GetTween()
            {
                return _image.DOFillAmount(Mathf.Clamp(_destValue.x, 0, 1), _duration).SetEase(_ease);
            }

            public override Tween GetReversedTween()
            {
                return _image.DOFillAmount(Mathf.Clamp(_originValue.x, 0, 1), _duration).SetEase(_ease);
            }

            public override void ResetToStart()
            {
                _image.fillAmount = Mathf.Clamp(_originValue.x, 0, 1);
            }

            public override void ResetToEnd()
            {
                _image.fillAmount = Mathf.Clamp(_destValue.x, 0, 1);
            }
        }
    }
}