public interface FractureAfterDeathFilter : UnitEffectFilter
{
    /**
     * @return ture, 则碎裂; 反之产生ragdoll
     */
    bool EnableBroken(GameUnit victim);
}