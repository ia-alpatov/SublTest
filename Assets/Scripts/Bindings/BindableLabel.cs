

using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;

namespace AbilitiesSystem.Views
{
    public class BindableLabelUint :BindableLabel<uint>
    {

    }
    public class BindableLabelString :BindableLabel<string>
    {

    }
    public abstract class BindableLabel<T> : MonoBehaviour, IBindableElement
    {
        [SerializeField] public TMP_Text _label;
        [SerializeField] public string _bindingTextPath;

        private IReadOnlyProperty<T> _textProperty;
        private PropertyBindingData _propertyBindingData;

        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            _propertyBindingData ??= _bindingTextPath.ToPropertyBindingData();

            _textProperty = objectProvider.RentReadOnlyProperty<T>(context, _propertyBindingData);
            _textProperty.ValueChanged += OnPropertyValueChanged;

            UpdateControlText(_textProperty.Value);
        }

        public void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_textProperty == null)
            {
                return;
            }

            _textProperty.ValueChanged -= OnPropertyValueChanged;

            objectProvider.ReturnReadOnlyProperty(_textProperty);

            _textProperty = null;

            UpdateControlText(default);
        }

        private void OnPropertyValueChanged(object sender, T newText)
        {
            UpdateControlText(newText);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void UpdateControlText(T newText)
        {
            _label.text = newText.ToString();
        }
    }
}