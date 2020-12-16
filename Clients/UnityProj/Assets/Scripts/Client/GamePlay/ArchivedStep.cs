using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;

public class ArchivedStep
{
    public List<Step> AllGridChanges = new List<Step>();

    public class Step
    {
        public GridPos GP;
        public bool BeforeIsFront;

        public Step(GridPos gp, bool beforeIsFront)
        {
            GP = gp;
            BeforeIsFront = beforeIsFront;
        }
    }
}