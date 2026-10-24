// Anything that can take damage implements this (PlayerHealth, VirusAi, etc.).
// Attackers just look for IDamageable and call TakeDamage(), so they don't need
// to know what kind of object they hit.
public interface IDamageable
{
    void TakeDamage(float amount);
}
