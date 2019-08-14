using System;
using System.Collections.Generic;
using System.Text;
using WoA.Lib.Business.StateObjects;

namespace WoA.Lib.Business
{
    public class ApplicationStateManager : IApplicationStateManager
    {
        public IState StateInfo { get; set; }
        public ApplicationState CurrentState { get; set; }

        public ApplicationStateManager()
        {
            CurrentState = ApplicationState.Neutral;
        }

        public void SetNeutral()
        {
            StateInfo = null;
            CurrentState = ApplicationState.Neutral;
        }

        public void SetState(ApplicationState state, IState stateInfo)
        {
            CurrentState = state;
            StateInfo = stateInfo;
        }
    }
}
