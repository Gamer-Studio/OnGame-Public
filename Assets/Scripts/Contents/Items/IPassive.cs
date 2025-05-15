using OnGame.Scenes.World;

namespace OnGame.Contents.Items
{
  public interface IPassive
  {
    void Apply(Player player);
    void Remove(Player player);
    bool TryGetValue <T>(string key, out T value);
  }
}