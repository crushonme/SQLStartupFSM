// See https://aka.ms/new-console-template for more information
using SQLStartupFSM;
using System.Threading;
var FSMProcess = new Process();
Console.WriteLine("SQLStartupFSM Starting...");
Console.WriteLine($"Current State:{FSMProcess.CurrentState.ToString()}");
Console.WriteLine("Press any key to Start up SQL...");
FSMProcess.MoveNext(FSMEvent.STARTUP);
Console.ReadLine();
while (true)
{
    Console.WriteLine($"Current State:{FSMProcess.CurrentState.ToString()}");
    Console.WriteLine("---------------------------");
    Console.WriteLine($"Please input a event number less than {((int)FSMEvent.MAXEVENT)}");
    Console.WriteLine("And you also can press q or esc to leave SQLStartupFSM");
    foreach (FSMEvent evt in (FSMEvent[])Enum.GetValues(typeof(FSMEvent)))
    {
        Console.WriteLine($" {(int)evt}  \t{evt.ToString()}");
    }
    Console.WriteLine("---------------------------");
    FSMEvent ev;
    var x = Console.ReadLine();
    if (Enum.TryParse(x, out ev))
    {
        Console.WriteLine($"Your input is Event {ev.ToString()}");
    }
    else if (x.StartsWith("q") | x.StartsWith("esc"))
    {
        Console.WriteLine("Leave the SQLSartupFSM");
        break;
    }
    else

    {
        Console.WriteLine("Your input is not a valid Event");
    }
    FSMProcess.MoveNext(ev);
}
