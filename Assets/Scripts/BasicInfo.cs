[System.Serializable]
public class BasicInfo
{
    public bool Survival = true;
    public int HP;
    public int AttackPoint;
    public float Speed;

    public void BeAttacked(int AP)
    {
        this.HP -= AP;

        this.CheckSurvive();
    }

    public void CheckSurvive()
    {
        Survival =  HP >= 0 ? true : false;
    }
}
