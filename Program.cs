using CallCenter;

public class Program
{
    public static void Main()
    {
        CallCenter.CallCenter callCenter = CallCenterFactory.Create(5, 1);
        _ = callCenter.Start();
        Random random = new Random();
        Console.WriteLine("Press 'c' to add a call or 'q' to quit.");
        while (true)
        {
            ConsoleKey PressedKey = Console.ReadKey(true).Key;
            if (PressedKey == ConsoleKey.Q)
            {
                callCenter.Stop();
                break;
            }
            else if (PressedKey == ConsoleKey.C)
            {
                callCenter.AddCall(random.Next(3, 11));
            }
        }

    }
}

