namespace denModels;

public class Idname
{
    public Idname(int id, string name)
    {
        Id=id;
        Name=name;
    }

    public Idname()
    {

    }

    public int Id { get; set; }
    public string Name { get; set; }
}