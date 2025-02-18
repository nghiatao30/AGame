using System;
using System.Collections.Generic;

namespace LatteGames.Utils
{
    public class QueryVariable<T>
    {
        private List<ConditionalVariable<T>> variables = new List<ConditionalVariable<T>>();
        public List<ConditionalVariable<T>> Variables => variables;
        private ConditionalVariable<T>.Variable fallbackVariable;
        public ConditionalVariable<T>.Variable FallbackVariable { get => fallbackVariable; set => fallbackVariable = value; }

        public bool StaticCache = false;
        private ConditionalVariable<T>.Variable staticCachedValue = null;

        public T Value
        {
            get{
                if(StaticCache && staticCachedValue != null)
                    return staticCachedValue.Value;
                return GetVariable().Value;
            }
            set{
                if(StaticCache && staticCachedValue != null)
                    staticCachedValue.Value = value;
                else
                    GetVariable().Value = value;
            }
        }

        public void ClearCache()
        {
            staticCachedValue = null;
        }

        public QueryVariable(ConditionalVariable<T>.Variable fallbackValue, IEnumerable<ConditionalVariable<T>> variables)
        {
            this.fallbackVariable = fallbackValue;
            this.variables = new List<ConditionalVariable<T>>(variables);
        }

        public QueryVariable(ConditionalVariable<T>.Variable fallbackValue, params ConditionalVariable<T>[] variables)
        {
            this.fallbackVariable = fallbackValue;
            this.variables = new List<ConditionalVariable<T>>(variables);
        }

        private ConditionalVariable<T>.Variable GetVariable()
        {
            var activeIndex = variables.FindIndex(variable => variable.Condition());
            var selectedVariable = fallbackVariable;
            if(activeIndex != -1)
                selectedVariable = variables[activeIndex].TrueValue;
            staticCachedValue = selectedVariable;
            return selectedVariable;
        }
    }

    public class QueryVariableBuilder<T>
    {
        private QueryVariable<T> queryVariable = new QueryVariable<T>(null);

        public QueryVariable<T> QueryVariable { get => queryVariable; }

        public QueryVariableBuilder<T> AddConditional(T value, Func<bool> condition)
        {
            queryVariable.Variables.Add(new ConditionalVariable<T>(new ConditionalVariable<T>.Variable(value), null, condition));
            return this;
        }
        public QueryVariableBuilder<T> Fallback(T value)
        {
            queryVariable.FallbackVariable = new ConditionalVariable<T>.Variable(value);
            return this;
        }
        public QueryVariableBuilder<T> StaticCache(bool staticCache)
        {
            queryVariable.StaticCache = staticCache;
            return this;
        }
    }

    public class ConditionalVariable<T>
    {
        public class Variable
        {
            private Func<T> GetFunc;
            private Action<T> SetFunc;

            public Variable(Func<T> getFunc, Action<T> setFunc)
            {
                GetFunc = getFunc;
                SetFunc = setFunc;
            }

            public Variable(T value, Action<T> setFunc)
            {
                GetFunc = ()=>value;
                SetFunc = setFunc;
            }

            public Variable(T value)
            {
                GetFunc = ()=>value;
                SetFunc = _=>{};
            }

            public T Value
            {
                get => GetFunc();
                set => SetFunc(value);
            }
        }
    
        private Variable trueValue;
        private Variable fallbackValue;
        private Func<bool> condition;

        public T Value
        {
            get {
                if(condition == null || condition())
                    return trueValue.Value;
                else
                    return fallbackValue.Value;
            }
            set {
                if(condition == null || condition())
                    trueValue.Value = value;
                else
                    fallbackValue.Value = value;
            }
        }

        public Func<bool> Condition { get => condition; }
        public Variable FallbackValue { get => fallbackValue; set => fallbackValue = value; }
        public Variable TrueValue { get => trueValue; set => trueValue = value; }

        public ConditionalVariable(Variable trueValue, Variable fallbackValue, Func<bool> condition = null)
        {
            this.trueValue = trueValue;
            this.fallbackValue = fallbackValue;
            this.condition = condition;
        }

        public ConditionalVariable(T trueValue, T fallbackValue, Func<bool> condition = null)
        {
            this.trueValue = new Variable(trueValue);
            this.fallbackValue = new Variable(fallbackValue);
            this.condition = condition;
        }
    }
}