
// 捡取金币时的Filter
public interface GoldPickUpFilter : UnitEffectFilter
{
    // 正值, 则为增加; 反之, 则减少
    float OnPickUpGold(GameUnit unit, float gold);
}