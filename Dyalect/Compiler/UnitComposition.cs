﻿using Dyalect.Runtime;
using Dyalect.Runtime.Types;
using System;
using System.Collections.Generic;

namespace Dyalect.Compiler
{
    public sealed class UnitComposition
    {
        public Guid Id = Guid.NewGuid();

        public UnitComposition(List<Unit> units)
        {
            Units = units;
            Types = StandardType.All.Clone();
        }

        public List<Unit> Units { get; }

        public FastList<DyTypeInfo> Types { get; }
    }    
}
