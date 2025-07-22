using DataServicesNET80.Models;
using EntityEvents;

namespace DataServicesNET80.DatabaseAccessLayer;

public partial class DatabaseAccessLayer
{
    private readonly IEntityEventsService _entityEventsService;
    public DatabaseAccessLayerState DALState { get; set; }
    public List<bodyinthebox> BodyInTheBoxes { get; set; }

    public int colourProperty { get; set; }

    public Dictionary<int, AssociatedData> items { get; set; }
}