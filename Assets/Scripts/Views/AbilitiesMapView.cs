
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AbilitiesSystem.ViewModels;
using Random = UnityEngine.Random;
using UnityMvvmToolkit.UGUI;
using TMPro;
using Unity.VisualScripting;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core;
using static UnityEngine.UI.Button;
using System.Linq;
using System;
using UnityEngine.UI.Extensions;

namespace AbilitiesSystem.Views
{
    [RequireComponent(typeof(UILineRenderer))]
    public class AbilitiesMapView : CanvasView<AbilitiesMapViewModel>
    {
        [SerializeField]
        private Button EarnButton;
        [SerializeField]
        private Button ForgotButton;
        [SerializeField]
        private Button ForgotAllButton;
        [SerializeField]
        private Button LearnButton;
        [SerializeField]
        private TextMeshProUGUI ScoreText;

        

        void Start()
        {
            var text = this.AddComponent<BindableLabelUint>();
            text._bindingTextPath = "Score";
            text._label = ScoreText;
            text.SetBindingContext(this.BindingContext, GetObjectProvider());

            LearnButton.onClick.AddListener(this.BindingContext.LearCurrentAbility);
            EarnButton.onClick.AddListener(this.BindingContext.AddScore);
            ForgotAllButton.onClick.AddListener(this.BindingContext.RemoveAllAbilitites);
            ForgotButton.onClick.AddListener(this.BindingContext.RemoveCurrentAbility);

            GenerateAblitiesMap();

            var LR = GetComponent<UILineRenderer>();
            LR.color = Color.blue;
            LR.Segments = new List<Vector2[]>();
            var children = this.GetComponentsInChildren<AbilityView>();
            foreach (var child in children)
                LR.Segments.AddRange(child.GetPoints().Select(p=> new Vector2[]{child.transform.localPosition, p}));
            LR.SetAllDirty();
        }


        private void GenerateAblitiesMap()
        {
            var baseAbility = CreateAbilityView(0,0);
            BindingContext.RootAbility = baseAbility;
            var baseEdges = Random.Range(3, 5);

            List<AbilityViewModel> allAblities = new List<AbilityViewModel>();

            for (int i = 0; i < baseEdges; i++)
            {
                var score = (uint)Random.Range(1, 4);
                var child = baseAbility.AddChild(CreateAbilityView(score, baseAbility.Depth + 1, baseAbility));
                child.AddRoot(baseAbility);
                allAblities.Add(child);
            }

            for (int i = 0; i < allAblities.Count; i++)
            {
                var depth = Random.Range(1, 3);
                List<AbilityViewModel> generatedAbilities = GenerateAbilityPaths(allAblities[i], depth);

                foreach(var ability in generatedAbilities)
                    allAblities[i].AddChild(ability);
            }

            var startOfConnections = Random.Range(0, 4);
            ConnectAbilities(allAblities, startOfConnections);
        }

        
        private AbilityViewModel CreateAbilityView(uint reqiredScore, uint depth, AbilityViewModel root = null)
        {
            GameObject view = new GameObject();

            view.AddComponent<CanvasRenderer>();
            var rect = view.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(80, 20);
        
            view.transform.SetParent(this.transform, true);
            
            var tmp = view.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = 12;
            

            var abilityViewModel = new AbilityViewModel(reqiredScore, depth, root)
            {
                Parent = this.BindingContext,
                Transform = view.transform
            };

            var abilityView = view.AddComponent<AbilityView>();
            abilityView.ResetBindingContext(GetObjectProvider());
            abilityView.SetBindingContext(abilityViewModel, GetObjectProvider());
            
            var text = view.AddComponent<BindableLabelString>();
            text._bindingTextPath = "Text";
            text._label = tmp;
            text.SetBindingContext(abilityViewModel, GetObjectProvider());

            var color = view.AddComponent<BindableColor>();
            color._bindingColorPath = "IsLearned";
            color._label = tmp;
            color.SetBindingContext(abilityViewModel, GetObjectProvider()); 
            
    
            rect.anchoredPosition = Random.insideUnitCircle.normalized * depth * 100;


            return abilityViewModel;
        }


        private List<AbilityViewModel> GenerateAbilityPaths(AbilityViewModel baseAbility, int depth)
        {
            if (depth == 0)
            {
                var baseEdges = Random.Range(1, 3);

                List<AbilityViewModel> allAblities = new List<AbilityViewModel>();

                for (int i = 0; i < baseEdges; i++)
                {
                    var score = (uint)Random.Range(1, 4);
                    allAblities.Add(CreateAbilityView(score, baseAbility.Depth + 1, baseAbility));
                }

                return allAblities;
            }
            else
            {
                var baseEdges = Random.Range(1, 3);

                List<AbilityViewModel> allAblities = new List<AbilityViewModel>();

                for (int i = 0; i < baseEdges; i++)
                {
                    var score = (uint)Random.Range(1, 4);
                    allAblities.Add(CreateAbilityView(score, baseAbility.Depth + 1, baseAbility));
                }

                for (int i = 0; i < allAblities.Count; i++)
                {
                    List<AbilityViewModel> generatedAbilities = GenerateAbilityPaths(allAblities[i], depth - 1);

                    foreach (var ability in generatedAbilities)
                        allAblities[i].AddChild(ability);
                }

                return allAblities;
            }
        }

        private static void ConnectAbilities(List<AbilityViewModel> rootAblities, int startOfConnections)
        {
            if (startOfConnections == 0)
            {
                List<AbilityViewModel> allAbilities = new List<AbilityViewModel>();

                foreach (var ability in rootAblities)
                {
                    allAbilities.Add(ability);
                    allAbilities.AddRange(ability.Children);
                }

                foreach (var ability in allAbilities)
                {
                    var isConnect = Random.Range(1, 3) == 2;

                    if (isConnect)
                    {
                        var toPick = Random.Range(0, allAbilities.Count);
                        if (allAbilities[toPick] != ability)
                        {
                            if (allAbilities[toPick].Depth > ability.Depth)
                            {
                                allAbilities[toPick].AddRoot(ability);
                            }
                            else if (allAbilities[toPick].Depth < ability.Depth)
                            {
                                ability.AddRoot(allAbilities[toPick]);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var ability in rootAblities)
                {
                    ConnectAbilities(ability.Children, startOfConnections - 1);
                }
            }
        }
    }
}