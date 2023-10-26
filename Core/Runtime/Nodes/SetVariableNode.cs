using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rehawk.Kite
{
    [Category("Variables")]
    public class SetVariableNode : NodeBase
    {
        [SerializeField] private string key;
        [SerializeField] private OperationOperator @operator;
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
      
        [Tooltip("If TRUE the value will be saved to the director as well and will be transferred to later flows.")]
        [SerializeField] private bool persistant = true;

        public override string Summary
        {
            get
            {
                string summary = string.Empty;

                summary += $"'{key}'";
                
                switch (@operator)
                {
                    case OperationOperator.Set:
                        summary += " = ";
                        break;
                    case OperationOperator.Add:
                        summary += " + ";
                        break;
                    case OperationOperator.Subtract:
                        summary += " - ";
                        break;
                    case OperationOperator.Multiply:
                        summary += " * ";
                        break;
                    case OperationOperator.Divide:
                        summary += " / ";
                        break;
                    case OperationOperator.Modulo:
                        summary += " % ";
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

        protected override void OnEnter(Flow flow)
        {
            base.OnEnter(flow);

            switch (dataType)
            {
                case DataType.Bool:
                    HandleBool(flow);
                    break;
                case DataType.Int:
                    HandleInt(flow);
                    break;
                case DataType.Float:
                    HandleFloat(flow);
                    break;
                case DataType.String:
                    HandleString(flow);
                    break;
                case DataType.Object:
                    HandleObject(flow);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Continue(flow);
        }

        private void HandleBool(Flow flow)
        {
            switch (@operator)
            {
                case OperationOperator.Set:
                    flow.SetValue(flow.Director, key, boolValue, persistant);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleInt(Flow flow)
        {
            int previousValue = flow.GetValue<int>(flow.Director, key);
            
            switch (@operator)
            {
                case OperationOperator.Set:
                    flow.SetValue(flow.Director, key, intValue, persistant);
                    break;
                case OperationOperator.Add:
                    flow.SetValue(flow.Director, key, previousValue + intValue, persistant);
                    break;
                case OperationOperator.Subtract:
                    flow.SetValue(flow.Director, key, previousValue - intValue, persistant);
                    break;
                case OperationOperator.Multiply:
                    flow.SetValue(flow.Director, key, previousValue * intValue, persistant);
                    break;
                case OperationOperator.Divide:
                    flow.SetValue(flow.Director, key, previousValue / intValue, persistant);
                    break;
                case OperationOperator.Modulo:
                    flow.SetValue(flow.Director, key, previousValue % intValue, persistant);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleFloat(Flow flow)
        {
            float previousValue = flow.GetValue<float>(flow.Director, key);
            
            switch (@operator)
            {
                case OperationOperator.Set:
                    flow.SetValue(flow.Director, key, floatValue, persistant);
                    break;
                case OperationOperator.Add:
                    flow.SetValue(flow.Director, key, previousValue + floatValue, persistant);
                    break;
                case OperationOperator.Subtract:
                    flow.SetValue(flow.Director, key, previousValue - floatValue, persistant);
                    break;
                case OperationOperator.Multiply:
                    flow.SetValue(flow.Director, key, previousValue * floatValue, persistant);
                    break;
                case OperationOperator.Divide:
                    flow.SetValue(flow.Director, key, previousValue / floatValue, persistant);
                    break;
                case OperationOperator.Modulo:
                    flow.SetValue(flow.Director, key, previousValue % floatValue, persistant);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleString(Flow flow)
        {
            switch (@operator)
            {
                case OperationOperator.Set:
                    flow.SetValue(flow.Director, key, stringValue, persistant);
                    break;
                case OperationOperator.Add:
                    string previousValue = flow.GetValue<string>(flow.Director, key);
                    flow.SetValue(flow.Director, key, previousValue + stringValue, persistant);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void HandleObject(Flow flow)
        {
            switch (@operator)
            {
                case OperationOperator.Set:
                    flow.SetValue(flow.Director, key, objectValue, persistant);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}