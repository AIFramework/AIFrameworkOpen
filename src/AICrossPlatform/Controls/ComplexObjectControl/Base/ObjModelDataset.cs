using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;

namespace AI.Controls.ComplexObjectControl.Base
{
    [Serializable]
    public class ObjModelDataset
    {
        public List<Vector> States { get; set; } = new List<Vector>();

        public List<Vector> ControlActions { get; set; } = new List<Vector>();


        public void Add(Vector state, Vector action)
        {
            States.Add(state);
            ControlActions.Add(action);
        }
    }
}
