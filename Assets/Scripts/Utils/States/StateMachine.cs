namespace OnGame.Utils.States
{
    public class StateMachine<T> where T : class
    {
        T ownerEntity;
        State<T> currentState;

        public void SetUp(T _owner, State<T> entryState)
        {
            ownerEntity = _owner;
            currentState = entryState;
        }

        public void Execute() { currentState?.Execute(ownerEntity); }

        public void ChangeState(State<T> _newState)
        {
            if (_newState == null) return;

            currentState?.Exit(ownerEntity);

            currentState = _newState;
            currentState.Enter(ownerEntity);
        }
    }
}
