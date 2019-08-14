using System;
using System.Collections.Generic;
using System.Text;
using WoA.Lib.Business.StateObjects;

namespace WoA.Lib.Business
{
    public enum ApplicationState
    {
        Neutral,
        RecipeCreation,
    };

    public interface IApplicationStateManager
    {
        ApplicationState CurrentState { get; set; }
        IState StateInfo { get; set; }
        void SetState(ApplicationState state, IState stateInfo);
        void SetNeutral();
    }
}
