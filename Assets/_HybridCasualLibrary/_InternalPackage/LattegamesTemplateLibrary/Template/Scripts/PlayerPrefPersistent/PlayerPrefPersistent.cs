using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
    public static class PlayerPrefPersistent
    {
        public abstract class PersitstentValue<T>
        {
            protected Func<string> getKey;
            protected Func<T> getDefaultValue;

            public PersitstentValue(string key, T defaultValue)
            {
                getKey = ()=> key;
                getDefaultValue = ()=>defaultValue;
            }
            public PersitstentValue(string key, Func<T> getDefaultValue)
            {
                getKey = ()=>key;
                this.getDefaultValue = getDefaultValue;
            }
            public PersitstentValue(Func<string> getKey, T defaultValue)
            {
                this.getKey = getKey;
                getDefaultValue = ()=>defaultValue;
            }
            public PersitstentValue(Func<string> getKey, Func<T> getDefaultValue)
            {
                this.getKey = getKey;
                this.getDefaultValue = getDefaultValue;
            }

            public T Value{
                get => GetValue();
                set => SetValue(value);
            }

            protected abstract void SetValue(T value);
            protected abstract T GetValue();
        }

        public class Float : PersitstentValue<float>
        {
            public Float(string key, float defaultValue) : base(key, defaultValue)
            {
            }

            public Float(Func<string> getKey, float defaultValue) : base(getKey, defaultValue)
            {
            }

            public Float(string key, Func<float> getDefaultValue) : base(key, getDefaultValue)
            {
            }

            public Float(Func<string> getKey, Func<float> getDefaultValue) : base(getKey, getDefaultValue)
            {
            }

            protected override float GetValue()
            {
                return PlayerPrefs.GetFloat(getKey(), getDefaultValue());
            }

            protected override void SetValue(float value)
            {
                PlayerPrefs.SetFloat(getKey(), value);
            }
        }

        public class Int : PersitstentValue<int>
        {
            public Int(string key, int defaultValue) : base(key, defaultValue)
            {
            }

            public Int(string key, Func<int> getDefaultValue) : base(key, getDefaultValue)
            {
            }

            public Int(Func<string> getKey, int defaultValue) : base(getKey, defaultValue)
            {
            }

            public Int(Func<string> getKey, Func<int> getDefaultValue) : base(getKey, getDefaultValue)
            {
            }

            protected override int GetValue()
            {
                return PlayerPrefs.GetInt(getKey(), getDefaultValue());
            }

            protected override void SetValue(int value)
            {
                PlayerPrefs.SetInt(getKey(), value);
            }
        }

        public class String : PersitstentValue<string>
        {
            public String(string key, string defaultValue) : base(key, defaultValue)
            {
            }

            public String(string key, Func<string> getDefaultValue) : base(key, getDefaultValue)
            {
            }

            public String(Func<string> getKey, string defaultValue) : base(getKey, defaultValue)
            {
            }

            public String(Func<string> getKey, Func<string> getDefaultValue) : base(getKey, getDefaultValue)
            {
            }

            protected override string GetValue()
            {
                return PlayerPrefs.GetString(getKey(), getDefaultValue());
            }

            protected override void SetValue(string value)
            {
                PlayerPrefs.SetString(getKey(), value);
            }
        }

        public class Boolean : PersitstentValue<bool>
        {
            public Boolean(string key, bool defaultValue) : base(key, defaultValue)
            {
            }

            public Boolean(string key, Func<bool> getDefaultValue) : base(key, getDefaultValue)
            {
            }

            public Boolean(Func<string> getKey, bool defaultValue) : base(getKey, defaultValue)
            {
            }

            public Boolean(Func<string> getKey, Func<bool> getDefaultValue) : base(getKey, getDefaultValue)
            {
            }

            protected override bool GetValue()
            {
                return PlayerPrefs.GetInt(getKey(), getDefaultValue()?1:0)!=0;
            }

            protected override void SetValue(bool value)
            {
                PlayerPrefs.SetInt(getKey(), value?1:0);
            }
        }
    }    
}
