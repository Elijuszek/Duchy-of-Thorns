namespace DuchyOfThorns;
public enum Team
{
    PLAYER,
    ENEMY,
    NEUTRAL
}

public enum TroopState
{
    PATROL,
    ENGAGE,
    ADVANCE,
    ATTACK,
}

public enum BaseCaptureOrder
{
    FIRST,
    LAST
}

public enum LoadingForm
{
    New,
    Save,
    Load
}

public enum ProjectileType
{
    ARROW,
    FIRE_ARROW
}

public enum LootType
{
    GOLD,
    SILVER,
    BRONZE
}

public enum TroopType
{
    ALLY_ARCHER,
    ALLY_FOOTMAN,

    ENEMY_ARCHER,
    ENEMY_FOOTMAN
}