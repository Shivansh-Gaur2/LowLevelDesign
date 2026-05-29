using System;

// === Template Method — Employee Onboarding ===

public abstract class EmployeeOnboarding
{
    public void OnBoard()
    {
        CollectDocuments();
        SetupWorkstation();
        AssignTeam();
        GrantAccess();
    }

    private void CollectDocuments() => Console.WriteLine("  Collecting ID, PAN, address proof...");
    protected abstract void SetupWorkstation();
    private void AssignTeam() => Console.WriteLine("  Assigning to team...");
    protected abstract void GrantAccess();
}

public class DeveloperOnboarding : EmployeeOnboarding
{
    protected override void SetupWorkstation() => Console.WriteLine("  Setting up MacBook with VS Code and Docker...");
    protected override void GrantAccess() => Console.WriteLine("  Granting access to GitHub, AWS Console, and CI/CD pipeline...");
}

public class ManagerOnboarding : EmployeeOnboarding
{
    protected override void SetupWorkstation() => Console.WriteLine("  Setting up ThinkPad with MS Office and Teams...");
    protected override void GrantAccess() => Console.WriteLine("  Granting access to Jira, Confluence, and budget dashboards...");
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Template Method Pattern ===\n");
        Console.WriteLine("Developer Onboarding:");
        new DeveloperOnboarding().OnBoard();

        Console.WriteLine("\nManager Onboarding:");
        new ManagerOnboarding().OnBoard();
    }
}
