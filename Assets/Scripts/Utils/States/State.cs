namespace OnGame.Utils
{
    public enum PlayerStates{Idle, Move, Dash, Guard, Dead}
    public enum EnemyStates{Patrol, Chase, Attack, Dead}
    
    public abstract class State<T> where T : class
    {
        // Start
        public abstract void Enter(T source);
        // Update
        public abstract void Execute(T source);
        // Destroy
        public abstract void Exit(T source);
    }
}
