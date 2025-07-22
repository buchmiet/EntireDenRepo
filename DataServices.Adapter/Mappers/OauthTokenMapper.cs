using System.Buffers;
using System.Runtime.CompilerServices;


namespace DataServices.Adapter.Mappers;

public static class OauthTokenMapper
{



    /// <summary>
    /// Mapuje pojedynczy token OAuth z DataService.Models do denModels.Persistent
    /// Metoda jest zoptymalizowana pod kątem wydajności i oznaczona jako inline
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static denModels.Persistent.OauthToken ToDomainModel(this DataServicesNET80.Models.OauthToken source)
    {
        if (source == null)
            return null;

        return new denModels.Persistent.OauthToken
        {
            OauthTokenId = source.OauthTokenId,
            LocationId = source.LocationId,
            ServiceId = source.ServiceId,
            AccessToken = source.AccessToken,
            RefreshToken = source.RefreshToken,
            ClientId = source.ClientId,
            ClientSecret = source.ClientSecret,
            AppId = source.AppId,
            CertId = source.CertId,
            DevId = source.DevId,
            AdditionalData = source.AdditionalData,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    /// <summary>
    /// Mapuje kolekcję tokenów OAuth z DataService.Models do denModels.Persistent
    /// Wykorzystuje ArrayPool dla zminimalizowania alokacji pamięci na stercie
    /// </summary>
    public static IEnumerable<denModels.Persistent.OauthToken> ToDomainModels(this IEnumerable<DataServicesNET80.Models.OauthToken> sources)
    {
        if (sources == null)
            return Enumerable.Empty<denModels.Persistent.OauthToken>();

        // Jeśli kolekcja wejściowa implementuje ICollection, możemy zoptymalizować alokację
        if (sources is ICollection<DataServicesNET80.Models.OauthToken> collection)
        {
            if (collection.Count == 0)
                return Array.Empty<denModels.Persistent.OauthToken>();

            // Dla małych kolekcji (do 1000 elementów) używamy ArrayPool
            if (collection.Count <= 1000)
            {
                var array = ArrayPool<denModels.Persistent.OauthToken>.Shared.Rent(collection.Count);
                try
                {
                    int index = 0;
                    foreach (var source in collection)
                    {
                        array[index++] = source.ToDomainModel();
                    }
                    // Zwracamy tylko wykorzystaną część tablicy
                    return new ArraySegment<denModels.Persistent.OauthToken>(array, 0, collection.Count);
                }
                catch
                {
                    // Zawsze zwracamy tablicę do puli, nawet w przypadku wyjątku
                    ArrayPool<denModels.Persistent.OauthToken>.Shared.Return(array);
                    throw;
                }
            }
        }

        // Dla większych kolekcji lub gdy nie możemy określić rozmiaru z góry
        // używamy standardowego LINQ, który i tak będzie wydajniejszy niż
        // tworzenie pośrednich list
        return sources.Select(s => s.ToDomainModel());
    }

    /// <summary>
    /// Mapuje pojedynczy token OAuth z denModels.Persistent do DataService.Models
    /// Metoda jest zoptymalizowana pod kątem wydajności
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DataServicesNET80.Models.OauthToken ToDataModel(this denModels.Persistent.OauthToken source)
    {
        if (source == null) 
            return null;

        return new DataServicesNET80.Models.OauthToken
        {
            OauthTokenId = source.OauthTokenId,
            LocationId = source.LocationId,
            ServiceId = source.ServiceId,
            AccessToken = source.AccessToken,
            RefreshToken = source.RefreshToken,
            ClientId = source.ClientId,
            ClientSecret = source.ClientSecret,
            AppId = source.AppId,
            CertId = source.CertId,
            DevId = source.DevId,
            AdditionalData = source.AdditionalData,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    /// <summary>
    /// Mapuje kolekcję tokenów OAuth z denModels.Persistent do DataService.Models
    /// Wykorzystuje podobne optymalizacje jak metoda ToDomainModels
    /// </summary>
    public static IEnumerable<DataServicesNET80.Models.OauthToken> ToDataModels(this IEnumerable<denModels.Persistent.OauthToken> sources)
    {
        if (sources == null)
            return Enumerable.Empty<DataServicesNET80.Models.OauthToken>();

        if (sources is ICollection<denModels.Persistent.OauthToken> collection)
        {
            if (collection.Count == 0)
                return Array.Empty<DataServicesNET80.Models.OauthToken>();

            if (collection.Count <= 1000)
            {
                var array = ArrayPool<DataServicesNET80.Models.OauthToken>.Shared.Rent(collection.Count);
                try
                {
                    int index = 0;
                    foreach (var source in collection)
                    {
                        array[index++] = source.ToDataModel();
                    }
                    return new ArraySegment<DataServicesNET80.Models.OauthToken>(array, 0, collection.Count);
                }
                catch
                {
                    ArrayPool<DataServicesNET80.Models.OauthToken>.Shared.Return(array);
                    throw;
                }
            }
        }

        return sources.Select(s => s.ToDataModel());
    }
}