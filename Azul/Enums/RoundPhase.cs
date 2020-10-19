namespace Azul
{
    public enum RoundPhase
    {
        RoundStart = 1,
        FactoryOffer = 2,
        WallTiling = 3,
        NextRoundPreparation = 4,
        GameEnd = 5
    }
    public enum RoundPhaseStatus 
    {
        FactoryDisplaysEmpty,
        FactoryDisplaysNonEmpty,
        AllPlayersScored,
        NotAllPlayersScored,
        GameEndConditionMet,
        GameEndConditionNotMet,
        NoCondition
    }

}