using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK;
using AbilitiesSystem.ViewModels;
using UnityMvvmToolkit.UGUI;
using TMPro;
using UnityEngine.EventSystems;

namespace AbilitiesSystem.Views
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    [RequireComponent(typeof(Selectable))]
    public class AbilityView : CanvasView<AbilityViewModel>, ISelectHandler
    {
        private TextMeshProUGUI _text;

        public TextMeshProUGUI UIText
        {
            get
            {
                if (_text == null)
                    _text = GetComponent<TextMeshProUGUI>();
                if (_text == null)
                    _text = gameObject.AddComponent<TextMeshProUGUI>();
                return _text;
            }
        }


        public void OnSelect(BaseEventData eventData)
        {
            this.BindingContext.Select();
        }

        public List<Vector2> GetPoints()
        {
            List<Vector2> points = new List<Vector2>();
            foreach (var child in BindingContext.Children)
                points.Add(child.Transform.localPosition);
            foreach (var child in BindingContext.Roots)
                points.Add(child.Transform.localPosition);
            return points;
        }
    }
}