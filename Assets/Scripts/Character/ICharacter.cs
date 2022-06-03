public interface ICharacter
{
    public void CheckCollisions();

    public void CheckDirection();

    public void MoveUpdate();

    public void Spawn();

    public void Spawn(int x, int y);
}
