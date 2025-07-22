namespace DataServices.Adapter;

public interface ICanBeMappedToDomain<out T>
{
    T ToDomainModel();
}