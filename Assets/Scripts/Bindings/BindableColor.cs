

using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;

namespace AbilitiesSystem.Views
{
    [RequireComponent(typeof(TMP_Text))]
    public class BindableColor : MonoBehaviour, IBindableElement
    {
        [SerializeField] public TMP_Text _label;
        [SerializeField] public string _bindingColorPath;

        private IReadOnlyProperty<bool> _boolProperty;
        private PropertyBindingData _propertyBindingData;

        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            _propertyBindingData ??= _bindingColorPath.ToPropertyBindingData();

            _boolProperty = objectProvider.RentReadOnlyProperty<bool>(context, _propertyBindingData);
            _boolProperty.ValueChanged += OnPropertyValueChanged;

            UpdateControlText(_boolProperty.Value);
        }

        public void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_boolProperty == null)
            {
                return;
            }

            _boolProperty.ValueChanged -= OnPropertyValueChanged;

            objectProvider.ReturnReadOnlyProperty(_boolProperty);

            _boolProperty = null;

            UpdateControlText(default);
        }

        private void OnPropertyValueChanged(object sender, bool newColor)
        {
            UpdateControlText(newColor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void UpdateControlText(bool newColor)
        {
            _label.color = newColor ? Color.green : Color.white;
        }
    }
}