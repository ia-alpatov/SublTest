
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;

namespace AbilitiesSystem.ViewModels
{

    public class AbilitiesMapViewModel  : IBindingContext
    {
        #region Binding Properties
        public AbilityViewModel RootAbility
        {
            get { return _rootAbility.Value; }
            set
            {
                if (_rootAbility.TrySetValue(value))
                {
                    _rootAbility.Value = value;
                }
            }
        }

        [Observable(nameof(RootAbility))]
        private readonly IProperty<AbilityViewModel> _rootAbility = new Property<AbilityViewModel>();



        public AbilityViewModel SelectedAbility
        {
            get { return _selectedAbility.Value; }
            set
            {
                if (_selectedAbility.TrySetValue(value))
                {
                    _selectedAbility.Value = value;
                }
            }
        }

        [Observable(nameof(SelectedAbility))]
        private readonly IProperty<AbilityViewModel> _selectedAbility = new Property<AbilityViewModel>();



        public uint Score
        {
            get { return _score.Value; }
            set
            {
               if (_score.TrySetValue(value))
                {
                    _score.Value = value;
                }
            }
        }

        [Observable(nameof(Score))]
        private readonly IProperty<uint> _score = new Property<uint>();

        #endregion

        #region public Methods
        public void LearCurrentAbility()
        {
            if(SelectedAbility != null && SelectedAbility.IsLearnPossible(Score))
            {
               Score -= SelectedAbility.LearnAbility();
            }
        }
    
        public void RemoveCurrentAbility()
        {
            if(SelectedAbility != null && SelectedAbility.IsRemovePossible())
            {
               Score += SelectedAbility.RemoveAbility();
            }
        }

        public void AddScore()
        {
            Score++;
        }
    
        
        public void RemoveAllAbilitites()
        {
            Score += RootAbility.ReturnToRootState();
        }
    
        #endregion
    }
}