using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rehawk.Kite
{
    [Serializable]
    [AddTypeMenu("Variable")]
    public class VariableCondition : ConditionBase
    {
        [SerializeField] private string key;
        [SerializeField] private ComparisonOperator @operator;
        [SerializeField] private DataType dataType;

        [ShowIf("dataType", DataType.Bool)]
        [SerializeField] private bool boolValue;
        [ShowIf("dataType", DataType.Int)]
        [SerializeField] private int intValue;
        [ShowIf("dataType", DataType.Float)]
        [SerializeField] private float floatValue;
        [ShowIf("dataType", DataType.String)]
        [SerializeField] private string stringValue;
        [ShowIf("dataType", DataType.Object)]
        [SerializeReference] private Object objectValue;

        public override string Summary
        {
            get
            {
                string summary = string.Empty;

                summary += $"VAR( {key} )";
                
                switch (@operator)
                {
                    case ComparisonOperator.Equal:
                        summary += " == ";
                        break;
                    case ComparisonOperator.NotEqual:
                        summary += " != ";
                        break;
                    case ComparisonOperator.Less:
                        summary += " < ";
                        break;
                    case ComparisonOperator.LessOrEqual:
                        summary += " <= ";
                        break;
                    case ComparisonOperator.Greater:
                        summary += " > ";
                        break;
                    case ComparisonOperator.GreaterOrEqual:
                        summary += " >= ";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (dataType)
                {
                    case DataType.Bool:
                        summary += boolValue.ToStringAdvanced();
                        break;
                    case DataType.Int:
                        summary += intValue.ToStringAdvanced();
                        break;
                    case DataType.Float:
                        summary += floatValue.ToStringAdvanced();
                        break;
                    case DataType.String:
                        summary += stringValue.ToStringAdvanced();
                        break;
                    case DataType.Object:
                        summary += objectValue.ToStringAdvanced();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                return summary;
            }
        }

        public override bool EvaluateCondition(Flow flow)
        {
            switch (dataType)
            {
                case DataType.Bool:
                    return EvaluateBool(flow);
                case DataType.Int:
                    return EvaluateInt(flow);
                case DataType.Float:
                    return EvaluateFloat(flow);
                case DataType.String:
                    return EvaluateString(flow);
                case DataType.Object:
                    return EvaluateObject(flow);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool EvaluateBool(Flow flow)
        {
            bool value = flow.GetValue<bool>(key);

            switch (@operator)
            {
                case ComparisonOperator.Equal:
                    return value == boolValue;
                case ComparisonOperator.NotEqual:
                    return value != boolValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private bool EvaluateInt(Flow flow)
        {
            int value = flow.GetValue<int>(key);

            switch (@operator)
            {
                case ComparisonOperator.Equal:
                    return value == intValue;
                case ComparisonOperator.NotEqual:
                    return value != intValue;
                case ComparisonOperator.Less:
                    return value < intValue;
                case ComparisonOperator.LessOrEqual:
                    return value <= intValue;
                case ComparisonOperator.Greater:
                    return value > intValue;
                case ComparisonOperator.GreaterOrEqual:
                    return value >= intValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private bool EvaluateFloat(Flow flow)
        {
            float value = flow.GetValue<float>(key);

            switch (@operator)
            {
                case ComparisonOperator.Equal:
                    return value == floatValue;
                case ComparisonOperator.NotEqual:
                    return value != floatValue;
                case ComparisonOperator.Less:
                    return value < floatValue;
                case ComparisonOperator.LessOrEqual:
                    return value <= floatValue;
                case ComparisonOperator.Greater:
                    return value > floatValue;
                case ComparisonOperator.GreaterOrEqual:
                    return value >= floatValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private bool EvaluateString(Flow flow)
        {
            string value = flow.GetValue<string>(key);

            switch (@operator)
            {
                case ComparisonOperator.Equal:
                    return value == stringValue;
                case ComparisonOperator.NotEqual:
                    return value != stringValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private bool EvaluateObject(Flow flow)
        {
            Object value = flow.GetValue<Object>(key);

            switch (@operator)
            {
                case ComparisonOperator.Equal:
                    return value == objectValue;
                case ComparisonOperator.NotEqual:
                    return value != objectValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}