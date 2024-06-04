using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitiesSystem;
using System.Linq;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Attributes;
using Unity.VisualScripting;
using AbilitiesSystem.Views;

namespace AbilitiesSystem.ViewModels
{
    [Serializable]
    public class AbilityViewModel : IBindingContext
    {
        #region Binding Properties

        [SerializeField]
        public List<AbilityViewModel> Children;
        [SerializeField]
        public List<AbilityViewModel> Roots;

        [SerializeField]
        public bool IsBase;

        [SerializeField]
        public Transform Transform; 

        public AbilitiesMapViewModel Parent;

        public bool IsLearned
        {
            get { return _isLearned.Value; }
            set
            {
               if (_isLearned.TrySetValue(value))
               {
                    _isLearned.Value = value;
               }
            }
        }

        [Observable(nameof(IsLearned))]
        private readonly IProperty<bool> _isLearned = new Property<bool>();


        public string Text
        {
            get { return _text.Value; }
            set
            {
               if (_text.TrySetValue(value))
                {
                    _text.Value = value;
                }
            }
        }

        [Observable(nameof(Text))]
        private readonly IProperty<string> _text = new Property<string>();



        [SerializeField]
        public uint ReqiredScore;

        [SerializeField]
        public Guid ID;

        [SerializeField]
        public uint Depth;

        #endregion

        public AbilityViewModel(){}
        public AbilityViewModel(uint reqiredScore, uint depth, AbilityViewModel root = null)
        {
            Children = new List<AbilityViewModel>();
            Roots = new List<AbilityViewModel>();
            ReqiredScore = reqiredScore;
            IsBase = root == null;
            Depth = depth;

            if (root != null)
                AddRoot(root);

            ID = Guid.NewGuid();

            IsLearned = IsBase;

            UpdateText(reqiredScore);
        }


        #region public Methods

        public bool IsRemovePossible()
        {
            return !IsBase && IsLearned && CountOfWaysToRoot() > 0 && !IsChildrenLearned();
        }

        public uint RemoveAbility()
        {
            IsLearned = false;
            return ReqiredScore;
        }

        public bool IsLearnPossible(uint currentScore)
        {
            return  !IsLearned && ReqiredScore <= currentScore && CountOfWaysToRoot() > 0;
        }

        public uint LearnAbility()
        {
            IsLearned = true;
            return ReqiredScore;
        }

        public AbilityViewModel AddChild(AbilityViewModel ability)
        {
            Children.Add(ability);
            return ability;
        }

        public void AddRoot(AbilityViewModel root)
        {
            Roots.Add(root);
        }
        public uint ReturnToRootState()
        {
            uint score = 0;

            if (IsBase)
            {
                List<AbilityViewModel> abilities = GetLeanedChildren();

                foreach (AbilityViewModel ability in abilities)
                    score += ability.ReturnToRootState();
            }
            else if (IsLearned)
            {
                IsLearned = false;
                score = ReqiredScore;
            }

            return score;
        }

        public List<AbilityViewModel> GetLeanedChildren()
        {
            List<AbilityViewModel> children = new List<AbilityViewModel>();
            foreach (AbilityViewModel ability in Children.Where(var => var.IsLearned))
            {
                children.Add(ability);
                children.AddRange(ability.GetLeanedChildren());
            }
            return children;
        }

        
        private void UpdateText(uint reqiredScore)
        {
            Text = $@"Стоимость:{reqiredScore}";
        }


        #endregion

        #region private Methods
        private uint CountOfWaysToRoot()
        {
            uint count = 0;

            if (!IsBase)
            {
                foreach (AbilityViewModel ability in Roots)
                {
                    if (ability.IsLearned)
                        count++;
                }
                return count;
            }
            else
            {
                return count;
            }
        }

        private bool IsChildrenLearned()
        {
            if (!IsBase)
            {
                foreach (AbilityViewModel ability in Children)
                {
                    if (ability.IsLearned)
                        return true;
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        internal void Select()
        {
            this.Parent.SelectedAbility = this;
        }
        #endregion
    }
}