﻿using ES.Engine.Models;

namespace ES.Engine.Mutation
{
    public interface IMutator
    {
        Solution Mutate(Solution solution);
    }
}
