using System;
using UnityEngine;
using UnityEngine.Events;

namespace OnGame.Utils
{
  [Serializable]
  public class RangedStat
  {
    public UnityEvent<int> onChanged;

    [SerializeField] [GetSet("Value")] private int value;
    [SerializeField] private Stat<int> max;

#if UNITY_EDITOR
    [SerializeField] [GetSet("Max")] private int maxValue;
#endif

    public RangedStat(int maxValue, int value, StatOperator<int> maxOper = null)
    {
      max = new Stat<int>(maxValue);
      Value = value;
      MaxOper = maxOper;
    }

    public RangedStat(int maxValue) : this(maxValue, maxValue)
    {
    }

    public int Value
    {
      get => value;
      set
      {
        var prevValue = this.value;
        this.value = Math.Max(0, Math.Min(value, max));
        onChanged?.Invoke(prevValue);
      }
    }

    public int Max
    {
      get => max;
      set
      {
        max.baseValue = value;
        Value = this.value;
      }
    }

    public StatOperator<int> MaxOper
    {
      get => max.oper;
      set
      {
        max.oper = value;
        Value = this.value;
      }
    }
  }
}