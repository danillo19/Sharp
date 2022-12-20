namespace Lab2;

public class Simulator
{
    public event EventHandler<int>? SimulateNewAttemptsEvent;
    public event EventHandler<int>? SimulateExistingAttemptEvent; 

    public void Simulate(int attemptNumber)
    {
        if (SimulateNewAttemptsEvent != null)
        {
            SimulateNewAttemptsEvent(this, attemptNumber);
        }
    }

    public void SimulateSingleEvent(int attemptNumber)
    {
        if (SimulateExistingAttemptEvent != null)
        {
            SimulateExistingAttemptEvent(this, attemptNumber);
        }
    }
}