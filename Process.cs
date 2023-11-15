using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLStartupFSM
{
    public enum FSMState
    {
        NOTSTART = 0,
        RECOVERING,
        RESTORING,
        RECOVERYPENDING,
        ONLINE,
        SUSPECT,
        EMERGENCY,
        OFFLINE,
        MAXSTATE
    }
    public enum FSMEvent
    {
        STARTUP = 0,
        RECOVERYSUCCESS,
        RECOVERYFAILURE,
        ALERTONLINE,
        ALERTEMERGENCY,
        ALERTOFFLINE,
        RESTOREDATABASE,
        RESTORINGLOG,
        BACKUPWITHNORECOVERY,
        RESTOREWITHRECOVERY,
        RESOURCEERROR,
        MAXEVENT
    }
    public class Process
    {
        class StateTransition
        {
            readonly FSMState CurrentState;
            readonly FSMEvent mEvent;

            public StateTransition(FSMState currentState, FSMEvent evt)
            {
                CurrentState = currentState;
                mEvent = evt;
            }

            public override int GetHashCode()
            {
                return 17 + 31 * CurrentState.GetHashCode() + 31 * mEvent.GetHashCode();
            }

            public override bool Equals(Object? obj)
            {
                if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    StateTransition? other = obj as StateTransition;
                    return other != null && this.CurrentState == other.CurrentState && this.mEvent == other.mEvent;
                }

            }
        }
        Dictionary<StateTransition, FSMState> transitions;
        public FSMState CurrentState { get; private set; }
        public Process()
        {
            CurrentState = FSMState.NOTSTART;
            transitions = new Dictionary<StateTransition, FSMState>
            {
                { new StateTransition(FSMState.NOTSTART, FSMEvent.STARTUP), FSMState.RECOVERING },
                { new StateTransition(FSMState.RECOVERING, FSMEvent.RESOURCEERROR), FSMState.RECOVERYPENDING },
                { new StateTransition(FSMState.RECOVERING, FSMEvent.RECOVERYFAILURE), FSMState.SUSPECT },
                { new StateTransition(FSMState.RECOVERING, FSMEvent.RECOVERYSUCCESS), FSMState.ONLINE },
                { new StateTransition(FSMState.SUSPECT, FSMEvent.RESTOREDATABASE), FSMState.RESTORING },
                { new StateTransition(FSMState.SUSPECT, FSMEvent.ALERTEMERGENCY), FSMState.EMERGENCY },
                { new StateTransition(FSMState.SUSPECT, FSMEvent.ALERTONLINE), FSMState.RECOVERING },
                { new StateTransition(FSMState.SUSPECT, FSMEvent.ALERTOFFLINE), FSMState.OFFLINE },
                { new StateTransition(FSMState.EMERGENCY, FSMEvent.ALERTONLINE), FSMState.RECOVERING },
                { new StateTransition(FSMState.EMERGENCY, FSMEvent.ALERTOFFLINE), FSMState.OFFLINE },
                { new StateTransition(FSMState.EMERGENCY, FSMEvent.RESTOREDATABASE), FSMState.RESTORING },
                { new StateTransition(FSMState.RESTORING, FSMEvent.RESTOREWITHRECOVERY), FSMState.RECOVERING },
                { new StateTransition(FSMState.RESTORING, FSMEvent.RESTORINGLOG), FSMState.RESTORING },
                { new StateTransition(FSMState.OFFLINE, FSMEvent.ALERTEMERGENCY), FSMState.EMERGENCY },
                { new StateTransition(FSMState.OFFLINE, FSMEvent.ALERTONLINE), FSMState.RECOVERING },
                { new StateTransition(FSMState.OFFLINE, FSMEvent.RESTOREDATABASE), FSMState.RESTORING },
                { new StateTransition(FSMState.RECOVERYPENDING, FSMEvent.RESTOREDATABASE), FSMState.RESTORING },
                { new StateTransition(FSMState.RECOVERYPENDING, FSMEvent.ALERTONLINE), FSMState.RECOVERING },
                { new StateTransition(FSMState.ONLINE, FSMEvent.ALERTEMERGENCY), FSMState.OFFLINE },
                { new StateTransition(FSMState.ONLINE, FSMEvent.BACKUPWITHNORECOVERY), FSMState.RESTORING },
                { new StateTransition(FSMState.ONLINE, FSMEvent.RESTOREDATABASE), FSMState.RESTORING }
            };
        }

        public FSMState GetNext(FSMEvent ev)
        {
            StateTransition transition = new StateTransition(CurrentState, ev);
            FSMState nextState;
            if (!transitions.TryGetValue(transition, out nextState))
            {
                Console.WriteLine($"The Event {ev.ToString()} is not a valid event for current State:{CurrentState}.");
                Console.WriteLine($"Keep Current State...{CurrentState}...");
                nextState = CurrentState;
                
            }
            return nextState;
            
        }

        public FSMState MoveNext(FSMEvent ev)
        {
            CurrentState = GetNext(ev);
            return CurrentState;
        }
    }

}
