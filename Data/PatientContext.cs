﻿using System.Collections.Generic;
using System.Linq;

namespace BackupCalcReport.Data
{
    internal class PatientContext
    {
        public PatientContext()
        {
            PlanningItemsInScope = new List<PlanningItem>();
        }

        public string PatientId { get; set; }
        public PlanningItem ActivePlanSetup { get; set; }
        public List<PlanningItem> PlanningItemsInScope { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as PatientContext;

            if (other != null)
            {
                return PatientId == other.PatientId
                    && ActivePlanSetup == other.ActivePlanSetup
                    && PlanningItemsInScope.SequenceEqual(other.PlanningItemsInScope);
            }

            return false;
        }

        // Do not bother with calculating a hashcode,
        // but overriding it stops a compiler warning
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
