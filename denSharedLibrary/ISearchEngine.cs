namespace denSharedLibrary;

public interface ISearchEngine
{
    /// <summary>
    /// Inicjalizuje strukturę danych wyszukiwarki na podstawie dostarczonego słownika.
    /// </summary>
    /// <param name="data">Słownik do inicjalizacji.</param>
    void Initialize(Dictionary<int, string> data);

    /// <summary>
    /// Wyszukuje identyfikatory na podstawie podanego ciągu wyszukiwania.
    /// </summary>
    /// <param name="searchTerm">Ciąg wyszukiwania.</param>
    /// <returns>Lista identyfikatorów pasujących do kryteriów wyszukiwania.</returns>
    List<int> Find(string searchTerm);

    /// <summary>
    /// Aktualizuje wpis w strukturze danych wyszukiwarki.
    /// </summary>
    /// <param name="dictionaryEntry">Wpis do aktualizacji (klucz i wartość).</param>
    void Refresh(KeyValuePair<int, string> dictionaryEntry);

    /// <summary>
    /// Dodaje nowy wpis do struktury danych wyszukiwarki.
    /// </summary>
    /// <param name="dictionaryEntry">Nowy wpis do dodania (klucz i wartość).</param>
    void Add(KeyValuePair<int, string> dictionaryEntry);
}