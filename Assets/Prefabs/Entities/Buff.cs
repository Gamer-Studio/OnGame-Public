using System;

namespace OnGame.Prefabs.Entities
{
  [Serializable]
  public class Buff
  {
    public string name;
    public string description;
    public Action<Entity> subscriber, unsubscriber;
  }
}