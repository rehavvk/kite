﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rehawk.Kite
{
    [Category("Flow")]
    [Description("Picks one of it's indented Case nodes based on the set mode.")]
    public class RandomNode : NodeBase, IBreakableNode
    {
        [Tooltip("Random = Totally random\nOnce = Random but without duplicates until all cases were used.")]
        [SerializeField] private Mode mode;

        [HideInInspector] 
        [SerializeField] private int[] optionIndices;

        public sealed override IncreaseIndentLevelMode IncreasesIndentLevel
        {
            get { return IncreaseIndentLevelMode.Next; }
        }

        public override string Title
        {
            get
            {
                switch (mode)
                {
                    case Mode.Random:
                        return "RANDOM";
                    case Mode.Once:
                        return "RANDOM ONCE";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override void OnEnter(Flow flow)
        {
            base.OnEnter(flow);

            int randomIndex = 0;

            if (mode == Mode.Random)
            {
                randomIndex = optionIndices[Random.Range(0, optionIndices.Length)];
            }
            else if (mode == Mode.Once)
            {
                if (TryGetNodeValue<List<int>>(flow, "available_options", out List<int> options) == false || options.Count <= 0)
                {
                    options = new List<int>(optionIndices);
                }

                randomIndex = options[Random.Range(0, options.Count)];
                options.Remove(randomIndex);

                SetNodeValue(flow, "available_options", options, true);
            }

            ContinueWithIndex(flow, randomIndex);
        }

        protected override void OnValidate(Sequence sequence)
        {
            base.OnValidate(sequence);

            var optionIndices = new List<int>();

            for (int i = Index; i < sequence.Count; i++)
            {
                if (sequence[i].IndentLevel == IndentLevel + 1 && sequence[i].GetType() == typeof(CaseNode))
                {
                    optionIndices.Add(i);
                }

                if (sequence[i].IndentLevel == IndentLevel && sequence[i].GetType() == typeof(EndNode))
                {
                    break;
                }
            }

            this.optionIndices = optionIndices.ToArray();
        }
        
        void IBreakableNode.Break(Flow flow)
        {
            ContinueOnIndentLevel(flow, IndentLevel);
        }

        public enum Mode
        {
            Random,
            Once
        }
    }
}