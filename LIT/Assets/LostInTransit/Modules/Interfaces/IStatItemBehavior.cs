﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostInTransit.Modules
{
    public interface IStatItemBehavior
    {
        void RecalcStatsStart();
        void RecalcStatsEnd();
    }
}
