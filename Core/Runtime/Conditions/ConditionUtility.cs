using System;

namespace Rehawk.Kite
{
    public static class ConditionUtility
    {
        public static string CreateSummary(ConditionBase[] conditions, ConditionEvaluationMode mode, bool invert)
        {
            string summary = string.Empty;

            if (conditions != null && conditions.Length > 0)
            {
                if (invert)
                {
                    summary += "NOT ( ";
                }
            
                switch (mode)
                {
                    case ConditionEvaluationMode.AllTrue:
                        for (int i = 0; i < conditions.Length; i++)
                        {
                            if (conditions[i] == null)
                                continue;
                        
                            if (i > 0)
                            {
                                summary += " AND ";
                            }
                        
                            summary += $"{conditions[i].Summary}";
                        }
                    
                        break;
                    case ConditionEvaluationMode.OneTrue:
                        for (int i = 0; i < conditions.Length; i++)
                        {
                            if (conditions[i] == null)
                                continue;

                            if (i > 0)
                            {
                                summary += " OR ";
                            }
                        
                            summary += $"{conditions[i].Summary}";
                        }
                    
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (invert)
                {
                    summary += " )";
                }
            }
            else
            {
                if (invert)
                {
                    summary += "NEVER";
                }
                else
                {
                    summary += "ALWAYS";
                }
            }

            return summary;
        }
        
        public static bool Evaluate(Flow flow, ConditionBase[] conditions, ConditionEvaluationMode mode, bool invert)
        {
            bool result = true;

            if (conditions != null)
            {
                for (int i = 0; i < conditions.Length; i++)
                {
                    bool tmpResult = conditions[i].EvaluateCondition(flow);
                
                    if (mode == ConditionEvaluationMode.AllTrue && !tmpResult)
                    {
                        result = false;
                        break;
                    }
                
                    if (mode == ConditionEvaluationMode.OneTrue && tmpResult)
                    {
                        break;
                    }
                }
            }

            return invert || result;
        }
    }

    public enum ConditionEvaluationMode
    {
        AllTrue,
        OneTrue
    }
}